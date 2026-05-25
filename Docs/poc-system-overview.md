# PropCentric POC System Overview

This document is intended as a presentation-oriented summary of how the PropCentric POC works today, what the main architectural patterns are, and how to add a new Prop that follows the same design.

## Goal of the POC

The POC explores a cleaner architecture for Vixen's prop-centric workflow.

The key idea is:

- a `Prop` owns its configuration and feature state
- wizard pages collect user data without directly editing the prop
- mapping layers move data between wizard state, prop state, and visual-model input
- visual models are generated from simple transfer objects rather than acting as the prop's data model
- discovery and DI registration happen at startup so new props can plug into the system with minimal manual wiring

## High-Level Architecture

The solution is split into a few main layers.

### `Props.Abstractions`

Defines the core contracts and shared base types:

- `IProp`, `BaseProp<TModel>`, `BaseLightProp<TModel>`
- feature interfaces such as `IHasLights`, `IHasDimming`, `IHasColor`, `IHasSegments`, and `ICanAxisRotate`
- setup-draft contracts such as `IHasColorSettingsDraft`, `IHasDimmingSettingsDraft`, `IHasSegmentsDraft`, and `IHasAxisRotationsDraft` under `Props.Abstractions.Setup.Drafts`
- setup contracts such as `IPropSetup`, `IPropSetupContext`, `IPropDraftMapper<,>`, and `ISegmentCaptureNormalizer`
- visual contracts such as `IVisualInputMapper<,>`, `IPropVisualModelBuilder<,>`, `IWizardPreviewCoordinator<>`, and `IWizardPreviewSession`
- visual element types such as `LightPoint`, `LightPointCloud`, and `IPropVisualModel`

This project defines the shape of the system without hard-coding a specific prop type.

### `Props.Registry`

Owns startup discovery and registration:

- scans assemblies for props decorated with `PropDescriptorAttribute`
- infers prop features from implemented feature interfaces
- discovers feature wizard pages
- registers prop support services into DI
- provides runtime registries and factories for resolving props and setup flows

This is the plugin/discovery backbone of the POC.

### `Props.Runtime`

Contains concrete prop implementations and their pipelines.

For the current POC, the main examples are `TreeProp` and `PolyLineProp`, each with its own setup, mapping, preview, and visual-model pipeline.

### `Props.OpenGlCommon`

Contains the drawing engine and OpenGL integration used by preview surfaces.

### `PropCentric`

A harness application used to drive and exercise the POC.

## How the System Works

There are three main flows:

1. startup discovery
2. prop setup/edit flow
3. visual-model generation and preview

## 1. Startup Discovery

Startup begins in:

- [PropSystemBootstrap.cs](C:/Dev/PropCentric/PropCentric/PropSystemBootstrap.cs)

That calls:

- `services.AddPropSystem(path);`

### What `AddPropSystem(...)` does

The registry bootstrapping code:

- loads assemblies from the plugin directory
- filters to assemblies starting with `Props`
- scans for concrete prop types decorated with `PropDescriptorAttribute`
- scans for feature wizard pages decorated with `FeatureWizardPageAttribute`
- registers the registries and factories
- registers discovered prop and setup types
- registers discovered support services by scanning generic contracts

Relevant file:

- [PropsServiceCollectionExtensions.cs](C:/Dev/PropCentric/Props.Registry/PropsServiceCollectionExtensions.cs)

### What is discovered automatically

The current startup discovery automatically registers:

- prop types
- setup wrapper types
- feature wizard pages
- `IVisualInputMapper<,>` implementations
- `IPropVisualModelBuilder<,>` implementations
- `IPropDraftMapper<,>` implementations
- `IWizardPreviewCoordinator<>` implementations

This means a prop support pipeline no longer needs a manual registration method like `AddTreePropServices()` in order to participate in the system.

## 2. Prop Discovery Pattern

A prop becomes discoverable when:

1. it implements `IProp`
2. it is a concrete class
3. it is decorated with `PropDescriptorAttribute`
4. the attribute points to a valid `IPropSetup` implementation

Examples:

- [TreeProp.cs](C:/Dev/PropCentric/Props.Runtime/Tree/TreeProp.cs)
- [PolyLineProp.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/PolyLineProp.cs)

The descriptor provides:

- prop id
- display name
- optional icon
- setup wrapper type

### Feature discovery

Features are inferred from implemented interfaces, not from manual flags on the prop.

Example:

- `TreeProp : ... , IHasLights`
- `BaseLightProp<TModel> : ... , IHasDimming`
- `TreeProp : ... , ICanAxisRotate`

At startup, feature flags are inferred by inspecting interfaces marked with `PropFeatureAttribute`.

Relevant file:

- [PropFeatureInferrer.cs](C:/Dev/PropCentric/Props.Registry/PropFeatureInferrer.cs)

This is important because it keeps feature support declarative and type-driven.

## 3. Setup and Edit Flow

Each prop has a setup wrapper that orchestrates create and edit operations.

Examples:

- [TreePropSetup.cs](C:/Dev/PropCentric/Props.Runtime/Tree/TreePropSetup.cs)
- [PolyLinePropSetup.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/PolyLinePropSetup.cs)

### Responsibilities of the setup wrapper

The setup wrapper:

- creates or accepts a prop instance
- can accept optional external setup input through `IPropSetupContext`
- creates a wizard draft model
- populates the draft from the prop for edit flows
- resolves feature wizard pages for the prop type
- initializes feature pages for the current wizard instance from the shared `FeatureWizardContext`
- creates the prop-specific wizard
- applies draft data back into the prop when the wizard is accepted
- commits the prop

### Why the wizard does not edit the prop directly

This is a core design pattern in the POC.

Wizard pages work against draft/page state, not the prop itself.

That gives the system:

- a safer edit experience
- a cleaner separation between UI state and domain state
- a consistent mapping pipeline
- better testability

For `PolyLineProp`, setup can also normalize captured world-space geometry before the wizard opens. The prop persists only normalized model-space `Segment` values, while the capture transform remains external setup data.

## 4. Wizard and Feature Page Composition

There are two categories of wizard pages:

- prop-specific pages
- feature-specific pages

### Prop-specific pages

These are provided directly by the prop implementation.

For the tree example:

- [TreePropWizardPage.cs](C:/Dev/PropCentric/Props.Runtime/Tree/Wizard/Pages/TreePropWizardPage.cs)

For the polyline example:

- [PolyLinePropWizardPage.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/Wizard/Pages/PolyLinePropWizardPage.cs)

### Feature-specific pages

These are discovered independently and inserted based on prop feature support.

For example, `DimmingFeatureWizardPage`, `ColorFeatureWizardPage`, `RotationFeatureWizardPage`, and `SegmentsFeatureWizardPage` are discovered through:

- `FeatureWizardPageAttribute`

Then `FeatureWizardPageResolver` determines which pages apply to a given prop type and initializes any page that implements `IFeatureWizardDraftPage`.

Relevant files:

- [FeatureWizardPageScanner.cs](C:/Dev/PropCentric/Props.Registry/FeatureWizardPageScanner.cs)
- [FeatureWizardPageResolver.cs](C:/Dev/PropCentric/Props.Registry/FeatureWizardPageResolver.cs)

### Feature page pattern

Feature wizard pages now follow one shared pattern.

Feature pages implement `IFeatureWizardDraftPage` and are initialized with a `FeatureWizardContext` containing:

- the shared wizard draft
- the shared `IWizardPreviewSession` for that wizard instance

This lets a feature page participate in live preview without duplicating preview-driving state in a page-local model, while still allowing non-viewer pages to use the same backing contract.

Current implemented examples:

- `DimmingFeatureWizardPage` is a reusable dimming page for `IHasDimming` props. It edits shared draft `Brightness` and `Gamma` values directly.
- `ColorFeatureWizardPage` is a draft-backed reusable color-settings page for `IHasColor` props. It edits the shared `LightColorConfiguration` draft state directly and exposes color-specific preview state in-page rather than hosting the shared OpenGL prop viewer.
- `RotationFeatureWizardPage` is a draft-backed reusable rotation page for `ICanAxisRotate` props. It edits shared draft `AxisRotations` values directly and rebuilds preview immediately.
- `SegmentsFeatureWizardPage` is a draft-backed reusable segments page for `IHasSegments` props. It hosts the shared OpenGL viewer and reflects `PointCount` edits immediately.

## 5. Draft and Mapping Pattern

Each prop setup flow uses an explicit draft type.

For the tree example:

- [TreePropDraft.cs](C:/Dev/PropCentric/Props.Runtime/Tree/Setup/TreePropDraft.cs)
- [PolyLinePropDraft.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs)

The draft holds wizard-owned values during setup/edit.

### Draft mapper

Each prop also has a draft mapper:

- [TreePropDraftMapper.cs](C:/Dev/PropCentric/Props.Runtime/Tree/Setup/TreePropDraftMapper.cs)
- [PolyLinePropDraftMapper.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/Setup/PolyLinePropDraftMapper.cs)

The mapper is responsible for:

- copying prop state into the draft before a wizard opens
- copying draft state back into the prop after the user confirms

This pattern isolates the translation between UI/setup state and domain state.

### Draft-backed feature edits

Feature pages edit shared draft state directly instead of holding disconnected page copies.

For the color flow:

- `TreePropDraft` and `PolyLinePropDraft` implement `IHasColorSettingsDraft`
- `IHasColorSettingsDraft` exposes the shared `LightColorConfiguration`
- `ColorFeatureWizardPage` edits that shared draft configuration directly for single-color, multiple-discrete-color, and full-color modes

For the polyline flow:

- `PolyLinePropDraft` implements `IHasSegmentsDraft`
- `IHasSegmentsDraft` exposes `ObservableCollection<SegmentDraftState>`
- `SegmentsFeatureWizardPage` wraps those segment draft items for display and editing

For the tree flow:

- `TreePropDraft` implements `IHasAxisRotationsDraft`
- `IHasAxisRotationsDraft` exposes `ObservableCollection<AxisRotationModel>`
- `RotationFeatureWizardPage` wraps those draft rotation items for display and editing

For dimming:

- `TreePropDraft` and `PolyLinePropDraft` implement `IHasDimmingSettingsDraft`
- `IHasDimmingSettingsDraft` exposes shared `Brightness` and `Gamma`
- `DimmingFeatureWizardPage` reads and writes those shared draft values directly

This is important because the preview coordinator already reads from the shared prop draft. When a feature page edits that same draft state, preview can rebuild immediately without waiting for wizard completion.

Important design boundary:

- `AxisRotations` are a setup-time and baseline prop-definition capability
- they define the default orientation of the prop geometry itself
- they are not the same thing as runtime rendered motion such as fixture pan, tilt, elevation, or other animated state
- runtime rendered motion belongs in a separate render-state concept in the real application and should not be modeled as baseline prop-definition `AxisRotations`

## 6. Visual Model Generation Pattern

The prop itself owns its configuration, but the visual model is generated from a transfer object.

### Prop -> visual input mapper

Each prop has a mapper that projects prop state into a visual input record.

Important:

- if the prop contains mutable reference-type state that affects rendering, the mapper must project that state as a value snapshot, not as a shared object reference
- otherwise a later in-place edit can change the underlying data without changing the visual input record identity or equality result
- axis rotation collections are the current concrete example: map the axis/angle values, not the original mutable collection and item instances
- those axis rotations are baseline setup definition data, not live rendered motion state

Example:

- [TreePropToVisualInputMapper.cs](C:/Dev/PropCentric/Props.Runtime/Tree/Visuals/TreePropToVisualInputMapper.cs)
- [PolyLinePropToVisualInputMapper.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/Visuals/PolyLinePropToVisualInputMapper.cs)

### Draft -> visual input mapper

Each wizard preview flow has a mapper that projects draft state into the same visual input shape.

Important:

- the draft-to-visual-input mapper has the same snapshot requirement as the prop-to-visual-input mapper
- wizard preview often mutates draft-owned objects in place, so passing mutable references through to the visual input record breaks unchanged-input detection in preview caching

Example:

- [TreeDraftToVisualInputMapper.cs](C:/Dev/PropCentric/Props.Runtime/Tree/Visuals/TreeDraftToVisualInputMapper.cs)
- [PolyLineDraftToVisualInputMapper.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/Visuals/PolyLineDraftToVisualInputMapper.cs)

### Visual input record

The visual input record contains only the subset of state needed to generate the visual model.

Example:

- [TreeVisualInput.cs](C:/Dev/PropCentric/Props.Runtime/Tree/Visuals/TreeVisualInput.cs)
- [PolyLineVisualInput.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/Visuals/PolyLineVisualInput.cs)

Important design point:

- a prop may have additional data that does not directly affect the visual model
- that data still belongs on the prop and can still be collected in setup
- only the rendering-relevant subset is mapped into the visual input transfer object
- if preview caching compares visual input records for equality, the record must behave like a value object for every rendering-relevant field
- mutable reference members inside the record must therefore either be converted to immutable/value-like snapshots before construction or be compared structurally by their contents
- do not assume record equality is sufficient if the record contains mutable reference types such as collections of mutable models

### Visual model builder

The builder consumes the visual input and produces the visual model.

Example:

- [TreeVisualModelBuilder.cs](C:/Dev/PropCentric/Props.Runtime/Tree/Visuals/TreeVisualModelBuilder.cs)
- [PolyLineVisualModelBuilder.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/Visuals/PolyLineVisualModelBuilder.cs)

This is the single place where tree geometry is defined.
For `PolyLineProp`, the builder creates one `LightSegment` per logical segment and deduplicates shared-corner lights between adjacent segments.

## 7. Preview Flow

Wizard preview uses the same general rendering pattern as runtime visual generation, but the source data is the wizard draft instead of the committed prop.

### Preview coordinator

The preview coordinator:

- maps draft state to visual input
- optionally reuses the last built preview if the input is unchanged
- returns the visual model used by the wizard drawing engine
- exposes an async `BuildPreviewAsync(...)` API so preview generation can run without blocking the wizard UI

Important:

- "unchanged" must mean value-equal rendering input, not "the same mutable objects are still referenced"
- if the coordinator caches based on `TVisualInput` equality, the mapper and record design must guarantee that an in-place change to rendering data produces a different equality result
- otherwise preview can incorrectly reuse stale geometry after edits such as changing a rotation angle inside an existing collection item
- current coordinators also serialize rebuild work so overlapping requests do not race cached preview state

Example:

- [TreeWizardPreviewCoordinator.cs](C:/Dev/PropCentric/Props.Runtime/Tree/Visuals/TreeWizardPreviewCoordinator.cs)
- [PolyLineWizardPreviewCoordinator.cs](C:/Dev/PropCentric/Props.Runtime/PolyLine/Visuals/PolyLineWizardPreviewCoordinator.cs)

This gives a dedicated place for preview-specific concerns without changing the prop model directly.

### Preview session

Feature pages that need live preview do not talk to a preview coordinator directly. Instead, the setup wrapper creates an `IWizardPreviewSession<TDraft>` once per wizard instance.

The preview session:

- exposes the shared draft
- delegates preview generation to the existing preview coordinator
- exposes an async `BuildPreviewAsync(...)` API used by prop pages and preview-capable feature pages
- gives prop-specific pages and feature pages a common way to request preview rebuilds

`SegmentsFeatureWizardPageViewModel` and `RotationFeatureWizardPageViewModel` use `IWizardPreviewSession.BuildPreviewAsync(...)` so their previews stay in sync with draft changes made on those same pages.

`ColorFeatureWizardPageViewModel` also requests async preview rebuilds when color configuration changes so the shared preview pipeline stays current, even though the Color page itself does not host the OpenGL prop viewer.

### Async preview rebuilds

Wizard preview rebuild scheduling is now fully async:

- page view models debounce rebuild requests
- debounced rebuilds propagate cancellation tokens
- preview generation runs through `BuildPreviewAsync(...)`
- stale preview results are ignored if a newer rebuild request has already superseded them

This matters because feature pages and prop pages can now trigger rebuilds frequently during editing without forcing the geometry build path to run synchronously on the UI thread.

## 8. Commit Flow

After the wizard is accepted:

1. the draft mapper applies prop and feature data from the shared draft
2. `CommitAsync()` finalizes the prop

For feature pages such as Dimming, Color, Rotation, and Segments, the feature-specific edits are already in the shared draft, so the prop draft mapper applies them as part of the normal draft-to-prop step.

For light props, commit typically does two things:

- generate or update backing element/node structures
- build and store the visual model

This behavior comes from:

- `BaseProp<TModel>`
- `BaseLightProp<TModel>`

## 9. Pattern to Add a New Prop

This is the repeatable pattern the POC is trying to prove.

### Step 1: Create the prop class

Add a concrete prop type that:

- derives from an appropriate base class
- implements feature interfaces that describe its capabilities
- is decorated with `PropDescriptorAttribute`

Example pattern:

```csharp
[PropDescriptor("...", "My Prop", typeof(MyPropSetup))]
public sealed class MyProp : BaseLightProp<MyPropVisualModel>, IHasLights, IHasDimming
{
}
```

### Step 2: Add prop-owned state

Add the properties that define the prop's configuration.

Examples:

- counts
- dimensions
- coverage
- patching-related metadata
- optional non-visual configuration

The prop remains the source of truth for its state.

### Step 3: Add the setup wrapper

Create an `IPropSetup` implementation that:

- creates/edits the prop
- creates the draft
- resolves feature pages
- initializes feature pages from the shared `FeatureWizardContext`
- maps draft data back into the prop
- commits the prop

### Step 4: Add the draft type

Create an `IPropDraft` implementation that holds the wizard's working state.

If a feature page must edit preview-driving state directly, expose that state through a feature-specific draft contract such as `IHasSegmentsDraft` or `IHasAxisRotationsDraft`.

If a feature page edits reusable color configuration, expose that through `IHasColorSettingsDraft` so the shared draft remains the single source of truth for light-type and color-mode selections.

Keep these contracts under `Props.Abstractions.Setup.Drafts` because they are setup-only shapes, not runtime prop feature interfaces.

### Step 5: Add the draft mapper

Create `IPropDraftMapper<TDraft, TProp>` to move state:

- prop -> draft
- draft -> prop

### Step 6: Add the visual input record

Create a record that contains only the subset of state required to generate the visual model.

This is the rendering contract for the prop.

If the record contains reference-type members that affect rendering, define them so equality still reflects value changes. Do not rely on default record equality over mutable shared references.

### Step 7: Add the visual input mappers

Create:

- `IVisualInputMapper<TProp, TVisualInput>`
- `IVisualInputMapper<TDraft, TVisualInput>`

These let both runtime and wizard preview use the same builder contract.

When mapping mutable rendering state such as rotation collections, project a snapshot by value instead of passing the original mutable objects through unchanged.

### Step 8: Add the visual model builder

Create:

- `IPropVisualModelBuilder<TVisualInput, TVisualModel>`

This type owns the geometry logic for the prop.

### Step 9: Add the preview coordinator

Create:

- `IWizardPreviewCoordinator<TDraft>`

This handles wizard preview rebuild behavior.

If it reuses the previous preview when the visual input is unchanged, "unchanged" must be based on value-equal rendering data, not reference identity of mutable child objects.

The current contract is async. Implement `BuildPreviewAsync(...)` so geometry generation can be awaited and canceled when newer rebuild requests supersede older ones.

### Step 10: Add the prop-specific wizard page(s)

Create the core wizard page(s) needed to collect prop-specific data.

### Step 11: Reuse feature wizard pages where possible

If the prop implements existing features such as dimming, the corresponding feature wizard pages can be discovered and inserted automatically.

If a feature page should rebuild preview immediately while the user edits it, have that page implement `IFeatureWizardDraftPage` and update shared draft state directly.

If the feature page is color-focused rather than geometry-focused, it should still edit the shared draft directly, but it does not need to host the OpenGL prop viewer.

### Step 12: Let discovery register everything

If all types live in a scanned `Props*` assembly and implement the expected contracts, startup discovery will now register:

- the prop
- its setup wrapper
- its visual mappers
- its visual model builder
- its draft mapper
- its preview coordinator
- any applicable feature wizard pages

That means adding a new prop should not require a dedicated manual DI extension method.

## Minimal Checklist for a New Prop

- [ ] Create the prop class and decorate it with `PropDescriptorAttribute`
- [ ] Implement the correct feature interfaces
- [ ] Create the setup wrapper
- [ ] Create the draft type
- [ ] Create the draft mapper
- [ ] Create the visual input record
- [ ] Create the prop-to-visual-input mapper
- [ ] Create the draft-to-visual-input mapper
- [ ] Create the visual model builder
- [ ] Create the wizard preview coordinator
- [ ] Create the prop-specific wizard page(s)
- [ ] Add tests for discovery, mapping, and visual generation

## Segmentable Prop Notes

The implemented segmentable-prop slice establishes these rules:

- props persist normalized model-space segment geometry
- world-space capture transforms remain external setup data
- `PolyLineProp` is open-only in this first slice
- segment feature pages edit `PointCount`, not capture geometry
- continuity is validated during normalization before rendering
- the Segments feature page uses the shared wizard draft and preview session
- color feature pages edit `LightColorConfiguration` in the shared draft and reuse the same feature-page discovery path as other features
- dimming, color, rotation, and segments all use the same shared-draft feature-page model

## Why This Pattern Matters

The value of the pattern is not just that a tree can be created.

It is that each concern is isolated:

- discovery is startup-only
- props own state
- setup wrappers orchestrate flow
- drafts isolate wizard state
- mappers isolate translation
- builders isolate geometry logic
- feature pages are reusable
- dimming, color, rotation, and segments now all follow the shared draft-backed feature-page pattern
- live preview can be shared across prop pages and feature pages
- async preview rebuilds keep frequent edit-time updates off the synchronous UI path
- DI resolves the full pipeline automatically

That is the main architectural result of the POC and the part most worth carrying forward into the real Vixen refactor.
