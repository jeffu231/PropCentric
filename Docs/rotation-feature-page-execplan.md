# Add A Reusable Rotation Feature Page Backed By `ICanRotate`

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

After this change, wizard-editable prop rotations will be modeled as a first-class reusable feature page instead of ad hoc state embedded in common prop-page bases. A new feature contract named `ICanRotate` will identify props that support wizard-editable rotations. `TreeProp` will be the first concrete example that implements `ICanRotate` and receives the reusable rotation page automatically. `PolyLineProp` will be the first concrete example that does not implement `ICanRotate` and therefore receives no rotation page.

The user-visible behavior should be easy to verify. When the user opens the Tree wizard, there will be a reusable Rotation page whose edits update the live preview immediately and persist when the wizard is accepted. When the user opens the PolyLine wizard, there will be no Rotation page and no rotation-specific wizard state.

This plan is intentionally separate from `IHasOrientation`. Orientation remains its own feature and must not be used as the gate for rotation editing. The new gate is `ICanRotate`.

## Progress

- [x] (2026-05-21 15:31 -05:00) Read `.agents/PLANS.md`, `Docs/poc-system-overview.md`, `Docs/core-design-goals.md`, and the current rotation-related wizard, draft, and visual-mapping code.
- [x] (2026-05-21 15:31 -05:00) Corrected the design direction so rotations remain independent from `IHasOrientation`.
- [x] (2026-05-21 15:31 -05:00) Refined the target shape to use a reusable feature page aligned with the existing feature-page discovery and draft-backed preview design.
- [x] (2026-05-21 15:31 -05:00) Chosen concrete naming so the new capability matches repository conventions: `PropFeatureFlags.Rotation`, `ICanRotate`, `IHasRotationsDraft`, and `RotationFeatureWizardPage`.
- [x] (2026-05-21 16:23 -05:00) Introduced `ICanRotate`, added `PropFeatureFlags.Rotation`, added `IHasRotationsDraft`, and added a focused discovery test proving the new feature interface maps to the new flag.
- [x] (2026-05-21 16:23 -05:00) Migrated the runtime draft and visual-input pipeline so `TreeProp` implements `ICanRotate` and remains rotation-enabled while `PolyLinePropDraft`, polyline visual input, polyline visual mapping, and polyline visual-model generation no longer carry rotation state.
- [x] (2026-05-21 16:23 -05:00) Ran focused validation with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~PropDiscoveryTests|FullyQualifiedName~TreeDraftMappingTests|FullyQualifiedName~PolyLineDraftMappingTests|FullyQualifiedName~PolyLineVisualInputMappingTests|FullyQualifiedName~PolyLineVisualModelBuilderTests|FullyQualifiedName~PolyLineWizardPreviewCoordinatorTests"` and confirmed the affected tests pass.
- [x] (2026-05-21 17:18 -05:00) Implemented `RotationFeatureWizardPage`, `RotationFeatureWizardPageViewModel`, and `RotationFeatureWizardPageView` under `Props.Runtime/Wizards/Features/Rotation`, with shared-draft wrapper items and preview-session-based rebuilds.
- [x] (2026-05-21 17:18 -05:00) Removed the old shared `Rotations` state from `PropWizardPageBase` and `PropBaseWizardPageViewModel`, removed the Tree preview-time copy-back workaround, removed the old Tree rotation UI block, and simplified the PolyLine page view model to a draft-only preview build path.
- [x] (2026-05-21 17:18 -05:00) Added focused discovery and page-behavior tests for the rotation feature page and reran validation with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~PropDiscoveryTests|FullyQualifiedName~RotationFeatureWizardPageTests|FullyQualifiedName~SegmentsFeatureWizardPageTests|FullyQualifiedName~TreeDraftMappingTests|FullyQualifiedName~PolyLineDraftMappingTests|FullyQualifiedName~PolyLineVisualInputMappingTests|FullyQualifiedName~PolyLineVisualModelBuilderTests|FullyQualifiedName~PolyLineWizardPreviewCoordinatorTests"`.
- [x] (2026-05-21 17:31 -05:00) Confirmed a simple UI check for the new Rotation page flow, then ran `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` and `dotnet build PropCentric.sln` successfully. One initial parallel build attempt hit a transient file-lock on `PropCentric.Tests.runtimeconfig.json`; rerunning the build sequentially succeeded.

## Surprises & Discoveries

- Observation: the current implementation already uses the feature-page pattern for reusable wizard slices, and the Segments page shows the draft-backed live-preview design that rotations should follow.
  Evidence: `Props.Runtime/Wizards/Features/Segments/Pages/SegmentsFeatureWizardPage.cs` implements `IFeatureWizardDraftPage`, and `Props.Runtime/Wizards/Features/Segments/ViewModels/SegmentsFeatureWizardPageViewModel.cs` schedules preview rebuilds without copy-back sync.

- Observation: the current rotation problem is mostly a state-ownership problem, not a rendering problem.
  Evidence: `Props.Runtime/Tree/Wizard/ViewModels/TreePropWizardPageViewModel.cs` copies `Rotations` into `wizardPage.Draft.AxisRotations` in `PreviewBuilder`, which indicates the preview pipeline only works correctly after an explicit synchronization step.

- Observation: rotation UI is currently forced onto every prop page through the common base classes.
  Evidence: `Props.Runtime/Wizards/Core/Pages/PropWizardPageBase.cs` constructs `Rotations` for every prop page, and `Props.Runtime/Wizards/Core/ViewModels/PropBaseWizardPageViewModel.cs` owns collection and item change handling for that shared property.

- Observation: the reusable feature-page path slots into the existing discovery pipeline without any prop-specific registration code.
  Evidence: `RotationFeatureWizardPage` is discovered via `FeatureWizardPageAttribute`, and `PropCentric.Tests/Discovery/PropDiscoveryTests.cs` now verifies both scanner discovery and resolver behavior for Tree versus PolyLine.

- Observation: PolyLine is already acting like a non-rotation prop in spirit, but the code still carries rotation state through the draft and visual pipeline.
  Evidence: `Props.Runtime/PolyLine/Wizard/ViewModels/PolyLinePropWizardPageViewModel.cs` comments out the rotation-to-draft sync line, while `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs` and `Props.Runtime/PolyLine/Visuals/PolyLineVisualInput.cs` still carry rotation data.

- Observation: the repository already has the discovery and feature-flag infrastructure needed for a new rotation capability.
  Evidence: startup discovery already resolves feature pages from feature interfaces as documented in `Docs/poc-system-overview.md`, and existing feature interfaces such as `IHasSegments` are decorated with `PropFeatureAttribute`.

- Observation: existing `PropFeatureFlags` values are noun-style capability names such as `Color`, `Segments`, `Dimming`, and `Orientation`.
  Evidence: `Props.Abstractions/Features/PropFeatureFlags.cs` uses noun-style labels, so `Rotation` fits better than `Rotate` for the new enum value.

## Decision Log

- Decision: introduce a new feature contract named `ICanRotate` for wizard-editable prop rotations.
  Rationale: the user explicitly requested that rotations align with the feature-page design, while also keeping them separate from `IHasOrientation`.
  Date/Author: 2026-05-21 / Codex

- Decision: use `PropFeatureFlags.Rotation` as the new feature-flag value.
  Rationale: the existing enum uses noun-style capability labels, and `Rotation` is both clear and distinct from `Orientation`.
  Date/Author: 2026-05-21 / Codex

- Decision: implement rotations as a reusable `RotationFeatureWizardPage` discovered from `ICanRotate`.
  Rationale: this removes rotation UI from the common prop-page base, matches the existing feature-page architecture, and makes Tree and future props opt in cleanly.
  Date/Author: 2026-05-21 / Codex

- Decision: keep shared wizard drafts as the single source of truth for rotation edits.
  Rationale: the duplicate state between common page/view-model bases and prop drafts is the direct cause of the brittleness. The Segments page already demonstrates the preferred draft-backed pattern.
  Date/Author: 2026-05-21 / Codex

- Decision: use Tree and PolyLine as the first supported and unsupported examples for `ICanRotate`.
  Rationale: this forces the implementation to prove both paths work: feature page present when supported, absent when unsupported.
  Date/Author: 2026-05-21 / Codex

## Outcomes & Retrospective

This plan records the corrected and refined implementation direction before code changes begin. The desired outcome is a reusable rotation feature page, a clean `ICanRotate` capability boundary, and a simpler draft-backed synchronization model.

The feature-page migration slice is now complete. Rotation editing lives in `Props.Runtime/Wizards/Features/Rotation`, Tree resolves that page through `ICanRotate`, and the common prop wizard base no longer owns rotation state. Focused automated validation now covers feature inference, feature-page discovery and resolution, shared-draft rotation editing, and the Tree/PolyLine split.

The broader validation step is now also complete: the full test suite passed, the full solution build passed, and the user confirmed a simple UI check for the rotation page flow. The only issue encountered during validation was a transient file lock caused by running build and test in parallel against the same test output; that was environmental and not a product-code failure.

## Context and Orientation

In this repository, a "draft" is the wizard-owned temporary state object used during prop create and edit flows. The setup wrapper populates the draft from the prop, the wizard edits that draft, and the draft mapper copies the accepted values back into the prop. The current examples are `Props.Runtime/Tree/Setup/TreePropDraft.cs`, `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs`, `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs`, and `Props.Runtime/PolyLine/Setup/PolyLinePropDraftMapper.cs`.

In this plan, "rotations" means the `AxisRotations` values that are edited in setup and then applied to the visual-model generation path for props that support them. This is not the same thing as `IHasOrientation`. The new rotation feature must stand on its own as `ICanRotate`.

Feature pages in this repository are reusable wizard pages discovered from feature interfaces. They live under `Props.Runtime/Wizards/Features/{Feature}` and are resolved automatically for props that implement the corresponding feature interface. Some feature pages still use mapper-backed prop synchronization, while newer ones, such as Segments, use the draft-backed pattern through `IFeatureWizardDraftPage` and `IWizardPreviewSession`.

The current rotation flow does not follow that architecture. `Props.Runtime/Wizards/Core/Pages/PropWizardPageBase.cs` creates a page-owned `Rotations` collection for every prop page. `Props.Runtime/Wizards/Core/ViewModels/PropBaseWizardPageViewModel.cs` subscribes to that collection and schedules preview rebuilds. At the same time, the drafts also own `AxisRotations`, and `TreePropWizardPageViewModel` explicitly copies the page collection into the draft in `PreviewBuilder`. That means rotations are neither a proper reusable feature page nor a clean draft-backed state path.

The target end state is:

1. `ICanRotate` marks props that support wizard-editable rotations.
2. a reusable `RotationFeatureWizardPage` is discovered for `ICanRotate`.
3. a draft-facing contract exposes shared rotation state to that page.
4. the page edits shared draft rotation state directly through thin wrapper items.
5. preview rebuilds read the shared draft without any extra copy-back code.
6. common prop-page base classes no longer own any rotation-specific state.

## Milestones

### Milestone 1: Add `ICanRotate` and explicit draft-backed rotation contracts

At the end of this milestone, rotation support will be explicit in both the prop feature model and the wizard draft model. `TreeProp` will implement `ICanRotate`, and `TreePropDraft` will implement a draft-facing rotation contract. `PolyLineProp` and `PolyLinePropDraft` will not. The preview-input pipeline will still work for Tree, and PolyLine will stop carrying dead rotation state through its draft and visual-input layers.

The proof for this milestone is a focused test run that shows Tree draft mapping still preserves rotations and PolyLine draft mapping no longer includes them. Feature-flag inference should also show Tree has the new rotation feature and PolyLine does not.

### Milestone 2: Implement the reusable Rotation feature page

At the end of this milestone, the repository will have a reusable rotation feature page under `Props.Runtime/Wizards/Features/Rotation` that is discovered from `ICanRotate`. The page will edit shared draft-backed rotation state and rebuild preview immediately when the user changes a rotation value.

The proof for this milestone is a harness check. Open the Tree wizard and confirm the Rotation feature page appears, editing a rotation updates preview immediately, and navigating away and back preserves the values because the shared draft is the source of truth.

### Milestone 3: Remove common-base rotation state and finish migration

At the end of this milestone, the old `Rotations` property and handler logic will be gone from `PropWizardPageBase` and `PropBaseWizardPageViewModel`. Tree will rely on the new feature page. PolyLine will have no rotation page and no rotation-specific wizard or preview code. Docs and tests will be aligned with the final design.

The proof for this milestone is that targeted tests pass, Tree behaves correctly in the harness, PolyLine does not expose rotations, and the docs explain the `ICanRotate` feature-page pattern.

## Plan of Work

Start in `Props.Abstractions/Features` by adding a new feature interface:

    [PropFeature(PropFeatureFlags.Rotation)]
    public interface ICanRotate
    {
    }

Add a new `PropFeatureFlags.Rotation` value and update any related feature-flag tests. This new feature is the discovery gate for the reusable rotation page. Do not reuse `IHasOrientation`.

Next add a draft-facing rotation contract in `Props.Abstractions/Features`, named `IHasRotationsDraft`, that exposes `ObservableCollection<AxisRotationModel> AxisRotations`. This contract is not a discovered prop feature; it is only the shared draft shape used by the feature page and preview flow.

Then make the prop and draft ownership explicit. `TreeProp` should implement `ICanRotate` and own rotation state. `TreePropDraft` should implement `IHasRotationsDraft`. `PolyLineProp` and `PolyLinePropDraft` should not participate. If practical, move `AxisRotations` out of `Props.Abstractions/Props/BaseProp.cs` so it is no longer universal runtime state. If that step is too invasive for the first slice, isolate the wizard and preview pipeline first, but the final state of this plan is that PolyLine must not carry rotation data in draft, visual input, or wizard UI.

Update the draft mappers and visual-input mappers. `TreePropDraftMapper`, `TreeDraftToVisualInputMapper`, and `TreePropToVisualInputMapper` should continue to populate and snapshot rotations. `PolyLinePropDraftMapper`, `PolyLineDraftToVisualInputMapper`, `PolyLinePropToVisualInputMapper`, `PolyLineVisualInput`, and `PolyLineVisualModelBuilder` should be simplified so they no longer carry or apply rotations.

Implement the reusable feature page under the documented feature structure:

1. `Props.Runtime/Wizards/Features/Rotation/Pages/RotationFeatureWizardPage.cs`
2. `Props.Runtime/Wizards/Features/Rotation/ViewModels/RotationFeatureWizardPageViewModel.cs`
3. `Props.Runtime/Wizards/Features/Rotation/Views/RotationFeatureWizardPageView.xaml`
4. any small wrapper model type needed for page-facing rotation items

The page should be decorated with `FeatureWizardPageAttribute` for `ICanRotate` and should implement `IFeatureWizardDraftPage`. In `Initialize(...)`, validate that the draft implements `IHasRotationsDraft`, store the preview session, and create wrapper items over the shared draft rotation objects. If the UI still needs `RotationAngleDefault`, axis display labels, or slider-specific state, keep that as wrapper or control-level state, not persisted prop or draft state.

The rotation feature view model should follow the same pattern as `SegmentsFeatureWizardPageViewModel`. It should set `PreviewBuilder` to `featureWizardPage.PreviewSession.BuildPreview()`, subscribe to rotation wrapper item changes, and call `SchedulePreviewRebuild()` when a rotation value changes. It must not convert an entire collection back into the draft inside `PreviewBuilder`.

Once the feature page exists, remove rotations from the common base classes. Delete the `Rotations` property and default collection construction from `Props.Runtime/Wizards/Core/Pages/PropWizardPageBase.cs`. Delete the rotation-specific collection and item handler logic from `Props.Runtime/Wizards/Core/ViewModels/PropBaseWizardPageViewModel.cs`. Then remove the old Tree-specific workaround from `Props.Runtime/Tree/Wizard/ViewModels/TreePropWizardPageViewModel.cs` and the `nameof(Rotations)` branch from `Props.Runtime/Tree/Wizard/Pages/TreePropWizardPage.cs`.

Finally, verify setup-wrapper wiring. Because feature pages are already resolved through `FeatureWizardPageResolver` and initialized through `InitializePages(...)`, the new rotation page should join the existing feature-page pipeline naturally once Tree implements `ICanRotate`. Confirm that no prop-specific manual registration path is introduced.

## Concrete Steps

Work from `C:\Dev\PropCentric`.

1. Re-read the current feature-page pattern and the old rotation path:

       Get-Content Props.Abstractions/Features/IHasSegments.cs
       Get-Content Props.Runtime/Wizards/Features/Segments/Pages/SegmentsFeatureWizardPage.cs
       Get-Content Props.Runtime/Wizards/Features/Segments/ViewModels/SegmentsFeatureWizardPageViewModel.cs
       Get-Content Props.Runtime/Wizards/Core/Pages/PropWizardPageBase.cs
       Get-Content Props.Runtime/Wizards/Core/ViewModels/PropBaseWizardPageViewModel.cs
       Get-Content Props.Runtime/Tree/Wizard/ViewModels/TreePropWizardPageViewModel.cs

2. Add `ICanRotate`, add `PropFeatureFlags.Rotation`, and add `IHasRotationsDraft`.

3. Update Tree and PolyLine prop, draft, mapper, visual-input, and builder code so Tree supports rotations and PolyLine does not.

4. Create `RotationFeatureWizardPage`, its view model, its view, and any wrapper items under `Props.Runtime/Wizards/Features/Rotation`.

5. Remove the old shared-base rotation state and Tree-specific copy-back workaround.

6. Add tests for feature inference, page resolution, draft-backed rotation editing, and Tree versus PolyLine behavior.

7. Run:

       dotnet build PropCentric.sln
       dotnet test PropCentric.Tests/PropCentric.Tests.csproj
       dotnet run --project PropCentric/PropCentric.csproj

8. Manually verify:

   1. Open the Tree wizard.
   2. Navigate to the Rotation page.
   3. Change one or more rotation values.
   4. Observe the preview update immediately.
   5. Finish the wizard, reopen Tree, and confirm the values persisted.
   6. Open the PolyLine wizard and confirm there is no Rotation page.

## Validation and Acceptance

Automated acceptance should cover the feature model, the draft-backed page behavior, and the supported versus unsupported prop paths.

Add or update feature inference tests so Tree resolves the `ICanRotate` flag and PolyLine does not. Add or update feature-page resolver tests so `RotationFeatureWizardPage` is returned for `TreeProp` but not for `PolyLineProp`.

Add or update Tree-focused tests so that `TreePropDraftMapper` preserves rotation values, `TreeDraftToVisualInputMapper` includes rotation snapshots, and `TreeWizardPreviewCoordinator` still reacts to changed rotation input. Add or update PolyLine-focused tests so that its draft mapper, visual-input mapper, and visual model builder no longer depend on rotations.

Add tests for the new rotation feature page similar to the Segments feature page tests. The essential assertion is that changing a wrapper item's angle updates the shared draft immediately and that preview rebuild scheduling happens from item changes rather than collection replacement.

Manual acceptance requires both Tree and PolyLine scenarios. Tree must expose the reusable Rotation page and live preview behavior. PolyLine must not expose the page and must not carry unused rotation editing state.

## Idempotence and Recovery

This work can be performed incrementally. The safest order is:

1. add `ICanRotate` and draft-facing rotation contracts
2. clean Tree and PolyLine draft/preview pipelines
3. add the new reusable rotation page
4. remove the old common-base rotation path

That order lets the repository compile with a temporary overlap while the new feature page is being wired.

If moving `AxisRotations` out of `BaseProp<TModel>` causes too much fallout in one step, keep the runtime property temporarily but still complete the wizard and preview migration first. The important recovery rule is that the final accepted state must have PolyLine removed from the wizard-editable rotation path and Tree using the reusable `ICanRotate` page.

If the rotation feature page is not fully ready yet, it is acceptable to keep Tree rotation editing on the prop-specific page for one short migration commit, but only if the shared draft has already become the single source of truth and the common base has stopped imposing rotation state on all props.

## Artifacts and Notes

The intended end-state relationships are:

    Rotation-supporting prop path:
        TreeProp implements ICanRotate
        TreePropDraft implements IHasRotationsDraft
        RotationFeatureWizardPage edits the shared draft directly
        Tree preview coordinator reads rotations from the shared draft
        Tree draft mapper applies rotations back to TreeProp on accept

    Non-rotation prop path:
        PolyLineProp does not implement ICanRotate
        PolyLinePropDraft does not implement IHasRotationsDraft
        RotationFeatureWizardPage is not resolved for PolyLine
        PolyLine visual input and builder do not carry rotation state

    Common wizard base path:
        PropWizardPageBase does not create or own rotation collections
        PropBaseWizardPageViewModel does not contain rotation-specific sync logic
        rotation-specific mutable state lives with the reusable rotation feature page

The key design constraint is that UI-only concerns, such as slider reset defaults, must not force a second persisted rotation model into the draft or prop. Keep those concerns local to the wrapper item or control.

## Interfaces and Dependencies

In `Props.Abstractions/Features/ICanRotate.cs`, define:

    [PropFeature(PropFeatureFlags.Rotation)]
    public interface ICanRotate
    {
    }

The enum name is intentionally `Rotation` rather than `Rotate` because `PropFeatureFlags` already uses noun-style capability labels such as `Color`, `Segments`, `Dimming`, and `Orientation`.

In `Props.Abstractions/Features/IHasRotationsDraft.cs`, define:

    public interface IHasRotationsDraft
    {
        ObservableCollection<AxisRotationModel> AxisRotations { get; }
    }

In `Props.Runtime/Wizards/Features/Rotation/Pages/RotationFeatureWizardPage.cs`, implement a feature page discovered for `ICanRotate` and also implementing `IFeatureWizardDraftPage`.

In `Props.Runtime/Wizards/Features/Rotation/ViewModels/RotationFeatureWizardPageViewModel.cs`, derive from `GraphicsWizardPageViewModelBase<RotationFeatureWizardPage>` and set:

    PreviewBuilder = () => featureWizardPage.PreviewSession?.BuildPreview()
        ?? throw new InvalidOperationException("Rotation preview session has not been initialized.");

The view model should subscribe to wrapper item changes and call `SchedulePreviewRebuild()`. It should not perform model conversion or copy-back synchronization.

At the end of this plan, no common wizard base class should expose a `Rotations` property, Tree should receive the reusable `RotationFeatureWizardPage` through `ICanRotate`, PolyLine should not, and no part of the implementation should use `IHasOrientation` to decide whether rotations are available.

Revision note: created on 2026-05-21 to refine the earlier draft-backed rotation plan into a reusable feature-page design driven by the new `ICanRotate` capability.
Revision note: updated on 2026-05-21 after the runtime draft and visual-input slice landed. The plan now records that Tree implements `ICanRotate`, PolyLine no longer carries rotation state in its draft or visual-input path, and the next major step is the reusable `RotationFeatureWizardPage`.
Revision note: updated on 2026-05-21 after the first implementation slice landed in `Props.Abstractions` and `PropCentric.Tests/Discovery`. The plan now records that `ICanRotate`, `PropFeatureFlags.Rotation`, and `IHasRotationsDraft` already exist, and that the next slice begins the runtime migration for Tree and PolyLine.
