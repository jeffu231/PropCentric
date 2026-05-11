# Segmentable Props

## Summary

Segmentable props represent an ordered path made of one or more logical segments. In this POC, the first concrete implementation is `PolyLineProp`, an open-only light prop whose rendered geometry is defined by normalized model-space segments.

Core rules for the current slice:

- segment order is fixed by capture order
- segment geometry is stored on the prop in normalized `0..1` model space
- world-space capture transforms remain outside the prop
- shared corners between adjacent segments represent a single light
- wizard feature pages may edit per-segment `PointCount`, but not capture geometry
- preview-driving segment edits live in the shared wizard draft during setup/edit flows
- element generation remains out of scope for this POC and is currently a no-op

## Captured Geometry Flow

Segment capture starts outside the prop system in a world-view workflow. The capture result is passed into setup as `SegmentCaptureSetupContext`, which contains:

- `CapturedWorldSegment` values in world coordinates
- a `WorldToModelTransform` describing the bounds used to normalize those coordinates

`ISegmentCaptureNormalizer` converts that captured input into ordered `Segment` records:

```csharp
public sealed record Segment(
    Vector2 Start,
    Vector2 End,
    int PointCount);
```

Normalization responsibilities:

- reject empty capture sets
- reject non-positive `PointCount`
- reject zero-width or zero-height transforms
- reject zero-length segments
- enforce open-polyline continuity
- preserve capture order

The prop stores only the normalized segments. It does not persist the world-space transform or viewer-specific state.

## Current Prop Pattern

The implemented segmentable prop pipeline is:

- `PolyLineProp`
- `PolyLinePropSetup`
- `PolyLinePropDraft`
- `PolyLinePropDraftMapper`
- `PolyLineVisualInput`
- `PolyLinePropToVisualInputMapper`
- `PolyLineDraftToVisualInputMapper`
- `PolyLineVisualModelBuilder`
- `PolyLineWizardPreviewCoordinator`

`PolyLineProp` implements both `IHasLights` and `IHasSegments`. Segment state is replaced atomically through `ReplaceSegments(IReadOnlyList<Segment> segments)`.

The draft-backed feature-page support for this slice adds:

- `IHasSegmentsDraft`
- `SegmentDraftState`
- `SegmentsFeatureWizardPage`

`PolyLinePropDraft` implements `IHasSegmentsDraft`, so the Segments page can edit shared draft segments without depending on `PolyLinePropDraft` specifically.

## Wizard Responsibilities

The setup wrapper accepts optional external setup input through `IPropSetupContext`.

For segmentable props, that means:

- create can seed geometry from `SegmentCaptureSetupContext`
- edit can reuse persisted prop segments when no context is supplied
- edit can replace persisted geometry before the wizard opens when recapture input is supplied

Wizard state is draft-owned, not prop-owned. The prop-specific page handles general prop settings and preview. The reusable `SegmentsFeatureWizardPage` is responsible only for:

- displaying ordered segments
- showing read-only start and end coordinates
- editing `PointCount`
- showing total point counts
- showing the OpenGL preview and rebuilding it from the shared draft when `PointCount` changes

It is explicitly not a geometry editor.

## Draft-Backed Segments Flow

The Segments page now follows the draft-backed feature-page pattern:

1. `PolyLinePropSetup` populates `PolyLinePropDraft` from the prop.
2. The setup wrapper creates an `IWizardPreviewSession<PolyLinePropDraft>`.
3. `FeatureWizardPageResolver.InitializePages(...)` passes the shared draft and preview session to any page implementing `IFeatureWizardDraftPage`.
4. `SegmentsFeatureWizardPage` projects `IHasSegmentsDraft.Segments` into `SegmentFeatureWizardItem` wrappers.
5. Changing `SegmentFeatureWizardItem.PointCount` updates the underlying `SegmentDraftState` immediately.
6. `SegmentsFeatureWizardPageViewModel` schedules a preview rebuild through `IWizardPreviewSession.BuildPreview()`.
7. When the wizard is accepted, `PolyLinePropDraftMapper.ApplyDraft(...)` copies the final draft segment counts back onto the prop.

This removes the previous duplicate page-owned segment state path.

## Visual Behavior

`PolyLineVisualModelBuilder` creates one `LightSegment` per logical segment and interpolates lights across each one. Adjacent segments deduplicate the shared corner light by omitting the duplicated first light on later connected segments.

Rendered geometry is built from normalized model-space segments and then transformed by any configured axis rotations.

Because the Segments page now rebuilds preview from the shared draft, changing `PointCount` on that page immediately changes the previewed light distribution without waiting for wizard completion.

## Future Scope

Closed segment shapes such as rectangles or polygons are not part of this first slice. If those are added later, they should be modeled as a separate capability with explicit closure rules rather than changing the current open-polyline assumptions.
