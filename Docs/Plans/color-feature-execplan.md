# Implement The Reusable `IHasColor` Feature And Color Editing Flow

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

After this change, props that support configurable light color behavior will expose that capability through the repository's reusable feature system instead of through ad hoc `StringType` fields embedded in base light-prop and prop-page code. A user will be able to open a prop wizard, land on a reusable Color feature page, choose between single-color, multiple discrete colors, and full-color modes, configure the corresponding options, and see those choices preserved when editing the prop again later.

The first user-visible proof should be in the existing harness flows for `TreeProp` and `PolyLineProp`. Both props already have lights and currently carry partial color state. After implementation, both should resolve a reusable Color feature page automatically, store color selections on the prop through the draft and setup pipeline, and show those selections in the summary and edit flows. The wizard preview should continue to build geometry normally; color-mode changes do not currently need to alter geometry because the setup preview renders lights with a default display color.

## Progress

- [x] (2026-05-24 11:10 -05:00) Read `Docs/color-feature-requirements.md`, `Docs/poc-system-overview.md`, `Docs/core-design-goals.md`, `Docs/naming-conventions.md`, and `.agents/PLANS.md`.
- [x] (2026-05-24 11:10 -05:00) Inspected the current light/color-related runtime and wizard code in `Props.Abstractions`, `Props.Runtime`, `Props.Registry`, and `PropCentric.Tests`.
- [x] (2026-05-24 11:10 -05:00) Identified the current architectural mismatch: `IHasColor` exists but is empty, while `BaseLightProp`, prop drafts, common wizard-page bases, and Tree-specific UI still own `StringType` and other color state directly.
- [x] (2026-05-24 11:10 -05:00) Chosen the implementation direction: move color state into explicit reusable color contracts, add a draft-backed `ColorFeatureWizardPage`, and remove color editing from common light prop pages.
- [x] (2026-05-24 11:34 -05:00) Implemented Milestone 1: replaced `StringTypes` with `LightType`, expanded `IHasColor`, introduced `LightColorConfiguration` and related value objects, and added `IHasColorSettingsDraft`.
- [x] (2026-05-24 11:34 -05:00) Migrated `TreeProp`, `PolyLineProp`, their drafts, draft mappers, shared light wizard bases, and Tree visual-input plumbing away from `StringType`.
- [x] (2026-05-24 11:34 -05:00) Updated focused discovery and draft-mapping tests, then validated Milestone 1 with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~PropDiscoveryTests|FullyQualifiedName~TreeDraftMappingTests|FullyQualifiedName~PolyLineDraftMappingTests|FullyQualifiedName~DraftBackedWizardPageTests|FullyQualifiedName~TreeVisualModelBuilderTests"` and `dotnet build PropCentric.sln`.
- [x] (2026-05-24 12:36 -05:00) Implemented Milestone 2: added `IColorConfigurationCatalog`, added the in-memory `InMemoryColorConfigurationCatalog`, seeded the required predefined discrete color sets and full-color orders, and registered the catalog through `AddPropSystem`.
- [x] (2026-05-24 12:36 -05:00) Added focused catalog tests for predefined contents, duplicate-name rejection, empty-set rejection, immediate custom-set availability, and DI registration in discovery tests.
- [x] (2026-05-24 12:36 -05:00) Validated Milestone 2 with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorConfigurationCatalogTests|FullyQualifiedName~PropDiscoveryTests"` and `dotnet build PropCentric.sln`.
- [ ] Implement the WPF color picker dialog, the inline multiple-discrete color editor, and the reusable `ColorFeatureWizardPage`.
- [ ] Update `TreeProp`, `PolyLineProp`, setup/draft/summary code, and tests to prove discovery, persistence, and editing behavior.

## Surprises & Discoveries

- Observation: the repository already has the exact reusable feature-page mechanism that color should use.
  Evidence: `Props.Registry/FeatureWizardPageResolver.cs` initializes `IFeatureWizardDraftPage` instances against a shared draft and preview session, and `Props.Runtime/Wizards/Features/Segments/Pages/SegmentsFeatureWizardPage.cs` demonstrates the preferred draft-backed pattern.

- Observation: `IHasColor` is already declared as a feature interface, but it currently carries no state and is not used by the runtime props.
  Evidence: `Props.Abstractions/Features/IHasColor.cs` is empty, while `Props.Runtime/Tree/TreeProp.cs` and `Props.Runtime/PolyLine/PolyLineProp.cs` do not implement `IHasColor`.

- Observation: current color state is spread across four different layers instead of being feature-owned.
  Evidence: `Props.Abstractions/Props/BaseLightProp.cs` owns `StringType`, `SingleColorOption`, and `SelectedColorSet`; `Props.Abstractions/Setup/Drafts/IHasLightSettingsDraft.cs` owns `StringType`; `Props.Runtime/Wizards/Core/Pages/LightPropWizardPage.cs` exposes `StringType`; and `Props.Runtime/Tree/Wizard/Views/TreePropWizardPageView.xaml` still renders the light-type combo box inline on the Tree page.

- Observation: current rendering does not require color-mode data to build geometry.
  Evidence: `Props.Runtime/Tree/Visuals/TreeVisualInput.cs` carries `StringType`, but the broader docs state wizard preview uses a default display color. This means color editing can be implemented as a feature without blocking on visual-geometry changes, though runtime color-handling integration should still be structured cleanly for later work.

- Observation: the existing `BaseLightProp` color summary logic is tightly tied to legacy names and incomplete behavior.
  Evidence: `Props.Abstractions/Props/BaseLightProp.cs` still formats summary text using `StringType`, only partially distinguishes modes, and leaves actual color-handling integration commented out.

- Observation: `StringType` was still flowing through `TreeVisualInput` even though the tree visual-model builder never used it.
  Evidence: `Props.Runtime/Tree/Visuals/TreeVisualInput.cs`, `Props.Runtime/Tree/Visuals/TreePropToVisualInputMapper.cs`, and `Props.Runtime/Tree/Visuals/TreeDraftToVisualInputMapper.cs` carried the field, but `Props.Runtime/Tree/Visuals/TreeVisualModelBuilder.cs` only reads geometry and rotation fields.

- Observation: C# records cannot declare a member named `Clone`.
  Evidence: the first Milestone 1 test run failed with `CS8859`, so the value-object copy helpers were renamed to `DeepClone`.

- Observation: putting a test class under the namespace `PropCentric.Tests.Color` caused unqualified `Color.White` references elsewhere in the test project to bind to the namespace name instead of `System.Drawing.Color`.
  Evidence: the first Milestone 2 test run failed with many `CS0234` errors such as "`White` does not exist in the namespace `PropCentric.Tests.Color`"; renaming the test namespace resolved it.

## Decision Log

- Decision: replace `StringTypes` with a new `LightType` enum and treat that rename as part of the feature migration, not a cosmetic follow-up.
  Rationale: the requirements explicitly say the current `StringType` terminology is wrong and belongs to the color feature. Keeping the old name would preserve the current architectural leak.
  Date/Author: 2026-05-24 / Codex

- Decision: keep `IHasLights` and `IHasColor` as separate features, but make current light props implement both.
  Rationale: the requirements say the two features are tightly coupled, but the repository's feature system is interface-driven. Separate interfaces preserve explicit feature discovery while still allowing Tree and PolyLine to opt into color cleanly.
  Date/Author: 2026-05-24 / Codex

- Decision: store color state on the prop as explicit value objects instead of only storing a selected set name.
  Rationale: the design goals say props own their configuration. If a prop stored only a foreign-key-like set name, later catalog edits could silently change existing prop behavior. The prop should persist the selected configuration snapshot it depends on.
  Date/Author: 2026-05-24 / Codex

- Decision: model multiple-discrete and full-color selections as different configuration shapes under one feature instead of flattening everything into a few loose strings.
  Rationale: the requirements distinguish between a list of concrete `System.Drawing.Color` values for multiple discrete colors and a channel-order name for full color. Treating them as distinct shapes makes validation and summary logic straightforward.
  Date/Author: 2026-05-24 / Codex

- Decision: implement the Color feature page as a draft-backed feature page, not as a mapper-backed page.
  Rationale: color configuration is prop-owned setup state, should participate in the shared wizard state, and may eventually affect preview-facing state. The draft-backed pattern already exists and fits better than page-local data plus a prop mapper.
  Date/Author: 2026-05-24 / Codex

- Decision: remove color editing from `LightPropWizardPage` and prop-specific pages once the reusable color feature page exists.
  Rationale: repository naming and architecture docs say reusable feature pages should own reusable feature editing. Leaving color fields in prop pages would duplicate state and undermine the feature system.
  Date/Author: 2026-05-24 / Codex

- Decision: remove color-mode data from `TreeVisualInput` during Milestone 1 instead of keeping a renamed placeholder field.
  Rationale: the current tree geometry builder does not consume color data, and carrying a renamed but unused field would preserve accidental coupling in the preview pipeline.
  Date/Author: 2026-05-24 / Codex

- Decision: use `DeepClone()` helpers on the new color value objects when copying between props and drafts.
  Rationale: the configuration contains nested read-only lists. Explicit cloning keeps props and drafts from sharing the same underlying list instances during wizard edits.
  Date/Author: 2026-05-24 / Codex

- Decision: place the in-memory catalog implementation in `Props.Registry` and register it explicitly in `AddPropSystem`.
  Rationale: `Props.Registry` already owns system bootstrapping, and putting the implementation there avoids introducing a `Props.Registry -> Props.Runtime` project dependency.
  Date/Author: 2026-05-24 / Codex

- Decision: treat duplicate discrete-color-set names as case-insensitive matches.
  Rationale: the requirements say user-defined names must be unique; case-insensitive comparison avoids near-duplicate names such as `RGB` versus `rgb`.
  Date/Author: 2026-05-24 / Codex

## Outcomes & Retrospective

Milestone 1 is now complete. The repository has a real color-domain contract layer: `LightType`, `LightColorChannel`, `DiscreteColorSetDefinition`, `FullColorOrderDefinition`, `LightColorConfiguration`, and `IHasColorSettingsDraft` now exist, and `IHasColor` is a real feature contract rather than an empty marker.

The current props and drafts now follow that contract. `TreeProp` and `PolyLineProp` both implement `IHasColor`, both drafts persist `ColorConfiguration`, and the shared light wizard base no longer owns color mode state. The Tree visual-input pipeline also no longer carries an unused color-mode field.

Milestone 1 validation passed: the focused tests for discovery, draft mapping, shared draft-backed page behavior, and tree visual generation all passed, and `dotnet build PropCentric.sln` succeeded.

Milestone 2 is now also complete. The repository has a shared color catalog contract plus an in-memory implementation that exposes the required predefined discrete color sets and full-color orders, persists custom discrete color sets for the current process, enforces unique names, and is available from DI through `AddPropSystem`.

Milestone 2 validation passed: the focused catalog and discovery tests passed, and `dotnet build PropCentric.sln` succeeded. The remaining work is now concentrated on the actual WPF editing surfaces and the reusable `ColorFeatureWizardPage`.

## Context and Orientation

In this repository, a "feature" is a capability a prop declares by implementing an interface marked with `PropFeatureAttribute`. Startup discovery in `Props.Registry` scans those interfaces and computes a `PropFeatureFlags` set for each prop type. Reusable feature wizard pages are discovered separately through `FeatureWizardPageAttribute` and injected into a prop wizard when the prop type implements the targeted feature interface. This is how `DimmingFeatureWizardPage`, `SegmentsFeatureWizardPage`, and `RotationFeatureWizardPage` are already resolved.

A "draft" is the wizard-owned temporary state object that exists during prop create and edit flows. Setup wrappers such as `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` populate the draft from the prop, build the wizard, initialize any draft-backed feature pages, and then copy accepted values back onto the prop before calling `CommitAsync()`.

Before Milestone 1, the color implementation did not match that architecture. `Props.Abstractions/Features/IHasColor.cs` existed but contained no members. Instead, `Props.Abstractions/Props/BaseLightProp.cs` stored three color-related properties directly: `StringType`, `SingleColorOption`, and `SelectedColorSet`. The drafts in `Props.Runtime/Tree/Setup/TreePropDraft.cs` and `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs` exposed `StringType` through `IHasLightSettingsDraft`. The common light page base in `Props.Runtime/Wizards/Core/Pages/LightPropWizardPage.cs` exposed `StringType`, and the Tree-specific view rendered the combo box inline in `Props.Runtime/Tree/Wizard/Views/TreePropWizardPageView.xaml`.

After Milestone 1, that legacy path is gone. Color state now lives in `LightColorConfiguration` on the prop and draft, `IHasLightSettingsDraft` only carries shared non-color light state, and the shared light page base no longer owns a color-mode property.

The requirements in `Docs/color-feature-requirements.md` establish the target behavior. A prop with color support must let the user choose one of three light types: single color, multiple discrete colors, or full color. Single color uses one concrete `System.Drawing.Color`. Multiple discrete colors uses a named list of colors and must support predefined and user-defined sets. Full color uses a named channel order such as `RGB` or `GRBW`, restricted to the channels Red, Green, Blue, and White.

The design in this plan uses explicit runtime value objects for those concepts. The prop remains the source of truth. Catalog services provide built-in and user-defined choices to the wizard, but the prop persists the actual chosen configuration snapshot so edit flows remain stable even if the catalog changes later.

## Proposed Design

The reusable feature contract should be expanded first. `Props.Abstractions/Features/IHasColor.cs` should stop being a marker-only interface and should expose the runtime color configuration that a prop owns. The cleanest shape is:

1. a `LightType` enum with the three required modes: `SingleColor`, `MultipleDiscreteColors`, and `FullColor`;
2. a `DiscreteColorSetDefinition` immutable record containing `string Name` and `IReadOnlyList<System.Drawing.Color> Colors`;
3. a `FullColorOrderDefinition` immutable record containing `string Name` and a validated ordered list of allowed color channels;
4. a `LightColorConfiguration` immutable record containing the selected `LightType`, the selected single color, the selected discrete color set snapshot, and the selected full-color order snapshot;
5. an expanded `IHasColor` contract that exposes `LightColorConfiguration ColorConfiguration { get; set; }`.

This design keeps the prop contract simple and value-oriented. Instead of spreading three loose properties across props, drafts, and pages, one configuration value becomes the feature state. The prop still exposes the exact configuration the wizard chose, and summary or runtime adapter code can branch on `ColorConfiguration.LightType`.

The existing `StringTypes` enum should be retired. The requirements explicitly call that name incorrect, and its current location under `Vixen.Sys.Props` reinforces the wrong ownership model. The replacement `LightType` enum should live in `Props.Abstractions/Features` or a nearby feature-focused namespace so both runtime props and wizard code can use it without routing color semantics through the old Vixen shim namespace.

Draft ownership should follow the same pattern as the modern feature pages. Add a setup-only interface such as `Props.Abstractions/Setup/Drafts/IHasColorSettingsDraft.cs` that exposes `LightColorConfiguration ColorConfiguration { get; set; }`. `TreePropDraft` and `PolyLinePropDraft` should implement it. `IHasLightSettingsDraft` should be narrowed so it only exposes shared non-color light state such as `LightSize`; `StringType` should be removed from it because color is no longer part of the generic light-page base.

`BaseLightProp` should keep only genuinely shared light-prop behavior. It can still provide helper methods and summary formatting for color if those methods operate on the new `ColorConfiguration` value, but it should stop being the place where feature identity leaks in through legacy property names. To preserve explicit feature opt-in, `BaseLightProp` should not itself implement `IHasColor`; instead, `TreeProp` and `PolyLineProp` should implement `IHasColor` directly while using the common property implementation inherited from `BaseLightProp`.

The reusable catalog surface should be added under `Props.Abstractions` and implemented in `Props.Runtime` or `Props.Registry` depending on where the team wants the concrete in-memory storage to live. The interface should be small and specific:

1. `GetDiscreteColorSets()` returns predefined sets plus any custom user-defined sets available in the current session.
2. `SaveDiscreteColorSet(DiscreteColorSetDefinition colorSet)` validates uniqueness and makes the new set available immediately.
3. `GetFullColorOrders()` returns the predefined channel orders required by the requirements.

For the POC, a process-local in-memory service is enough. It satisfies the "stub this function to provide minimal runtime behavior" requirement while still exercising the UI and setup flow realistically.

The reusable UI should be split into three pieces. The first is a `ColorFeatureWizardPage` under `Props.Runtime/Wizards/Features/Color` that is decorated with `[FeatureWizardPage(typeof(IHasColor), priority: ...)]` and implements `IFeatureWizardDraftPage`. The second is a reusable color picker dialog that edits a single `System.Drawing.Color` using HSV and RGB numeric entry, a spectrum panel, quick-select swatches, and old-versus-new preview rectangles. The third is an inline multiple-discrete color editor region that sits inside the Color feature page when `LightType.MultipleDiscreteColors` is selected. The requirements strongly prefer this inline editor over a second popup, so only the single-color picker should be modal.

The `ColorFeatureWizardPage` should use the shared draft as the source of truth. When initialized, it casts the shared draft to `IHasColorSettingsDraft`, binds to `draft.ColorConfiguration`, and projects that state into page-facing helper models as needed. It should show:

1. a combo box for `LightType`;
2. a single-color editor surface for `SingleColor`;
3. an inline multiple-discrete editor for choosing or creating a `DiscreteColorSetDefinition`;
4. a combo box for selecting a `FullColorOrderDefinition`;
5. summary text that matches the chosen mode.

The page can rebuild preview requests when the light type changes even though geometry is currently unaffected. That keeps the feature page aligned with the shared preview-session pattern and avoids future special cases if color ever affects wizard rendering.

Summary generation should move to the new value-object model. `BaseLightProp.GetColorSummary()` and the prop-specific summary text in `TreeProp` should describe the selected `LightType` and the relevant details from `ColorConfiguration` rather than referring to `StringType` or loose string fields. Edit flows must prove that an accepted prop reopens with the same saved selection, including a custom discrete color set.

Runtime color-handling integration should be structured but scoped appropriately. `BaseLightProp` already has commented placeholders for applying color-handling metadata to element nodes. This feature implementation should reintroduce a single internal method that converts `LightColorConfiguration` into the color-handling shape expected by the future adapter layer, but the POC does not need a full external persistence system or complete Vixen patching integration to be considered successful.

## Milestones

### Milestone 1: Refactor The Domain Model To A Real Color Feature

At the end of this milestone, the repository will no longer treat color mode as `StringType` owned by common light-page code. `LightType`, the runtime color configuration records, and `IHasColor` members will exist. `TreeProp` and `PolyLineProp` will implement `IHasColor`. Their drafts will implement `IHasColorSettingsDraft`. The old `StringTypes` enum and any direct `StringType` bindings in shared page bases will be removed or replaced.

The proof for this milestone is a focused test run showing that feature discovery now marks Tree and PolyLine with `PropFeatureFlags.Color`, that draft mappers persist `ColorConfiguration`, and that edit flows round-trip the new configuration values. The main work in this milestone is contract and mapping refactoring, not UI.

### Milestone 2: Add Catalog Services For Predefined And Custom Color Sets

At the end of this milestone, the repository will have catalog services that provide built-in multiple-discrete color sets such as `RGB`, `RGBW`, and `GRBW`, built-in full-color orders such as `RGB`, `RBG`, `GBR`, `GRB`, `BRG`, `BGR`, `RGBW`, and `GRWB`, and in-memory persistence for newly added custom multiple-discrete color sets.

The proof for this milestone is a focused test run that validates predefined catalog contents, uniqueness enforcement for user-defined multiple-discrete set names, and immediate availability of newly saved sets in subsequent queries. This milestone intentionally avoids wizard UI so the catalog behavior can be validated in isolation.

### Milestone 3: Implement The Reusable Color Picker Dialog

At the end of this milestone, the repository will contain a reusable WPF color picker dialog and supporting view model under a stable feature-oriented namespace and folder structure. The control will expose synchronized HSV and RGB numeric editing, quick-select swatches for white, red, green, and blue, and old-versus-new color previews. Validation will constrain RGB and HSV ranges to the supported numeric bounds.

The proof for this milestone is a focused test set for view-model behavior plus a simple manual harness check. The manual check is: open the color picker from a temporary host or from the in-progress color feature page, change RGB values, observe HSV fields update, click a preset swatch, and observe the old/new preview rectangles and numeric fields update consistently.

### Milestone 4: Implement The Draft-Backed `ColorFeatureWizardPage`

At the end of this milestone, the repository will have a reusable `ColorFeatureWizardPage`, `ColorFeatureWizardPageViewModel`, and `ColorFeatureWizardPageView` under `Props.Runtime/Wizards/Features/Color`. The page will resolve automatically for props implementing `IHasColor`, show the mode selector, show the correct dynamic editor region for the active mode, and update the shared draft directly. The multiple-discrete path will allow selection of a predefined set, creation of a new custom set with a unique name, and editing the set's individual colors with the reusable color picker dialog.

The proof for this milestone is a harness check using both `TreeProp` and `PolyLineProp`. Open each wizard and confirm the Color page is present. Switch among the three modes, configure the mode-specific settings, navigate away and back, and confirm the chosen state persists because the shared draft is the source of truth.

### Milestone 5: Remove Legacy Color UI And Finish Integration

At the end of this milestone, no prop-specific wizard page or common light-page base will own color-selection UI directly. Tree-specific inline `StringType` controls will be gone. Summary text will use the new color configuration model. Tests will cover discovery, draft mapping, feature-page discovery, catalog behavior, and at least one end-to-end setup round trip per current color-capable prop type.

The proof for this milestone is a full targeted validation pass plus one manual edit-flow check. Create a Tree prop with a custom multiple-discrete set, accept the wizard, reopen the prop for edit, and confirm the Color page still shows the same selected set and colors.

## Plan of Work

Start in `Props.Abstractions`. Add the new color-domain types and expand `IHasColor`. Introduce `IHasColorSettingsDraft` under `Props.Abstractions/Setup/Drafts`. Update or replace `IHasLightSettingsDraft` so it no longer carries color state. Rename the old `StringTypes` enum to `LightType` by creating the new enum in the abstraction layer and then replacing all references in runtime code and tests. Do not leave the old type as the system of record.

Update `Props.Abstractions/Props/BaseLightProp.cs` next. Replace `StringType`, `SingleColorOption`, and `SelectedColorSet` with a single `LightColorConfiguration ColorConfiguration` property plus helper accessors only if they materially simplify UI or summary code. Update `GetColorSummary()` to branch on the new configuration shape. Keep the runtime color-handling placeholder methods additive and non-destructive.

Update the concrete runtime props and drafts. `Props.Runtime/Tree/TreeProp.cs` and `Props.Runtime/PolyLine/PolyLineProp.cs` should implement `IHasColor`. `Props.Runtime/Tree/Setup/TreePropDraft.cs` and `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs` should implement `IHasColorSettingsDraft`. Their draft mappers should copy `ColorConfiguration` both directions. Remove any remaining `StringType` references from `Props.Runtime/Wizards/Core/Pages/ILightPropWizardPage.cs`, `Props.Runtime/Wizards/Core/Pages/LightPropWizardPage.cs`, `Props.Runtime/Wizards/Core/ViewModels/LightWizardPageViewModel.cs`, and prop-specific page code.

Add the catalog service contracts and implementation. Keep the contracts under `Props.Abstractions` and the first concrete in-memory implementation under a scanned `Props*` assembly so startup discovery or explicit DI registration can resolve it without prop-specific bootstrap code. Seed the service with the predefined multiple-discrete and full-color options from the requirements. Enforce uniqueness for custom multiple-discrete set names and reject invalid full-color orders that contain unsupported channels or duplicates.

Add the reusable color picker dialog and its supporting view model under `Props.Runtime/Wizards/Features/Color`. Keep the popup focused on editing one `System.Drawing.Color` value. Use Catel MVVM, numeric text entry, and explicit synchronization methods so RGB and HSV updates stay coherent. The picker does not need to persist state beyond the dialog result.

Add the `ColorFeatureWizardPage` last, after the contracts and dialog exist. Decorate it with `FeatureWizardPageAttribute` targeting `IHasColor`. Implement `IFeatureWizardDraftPage`. In `Initialize`, require `IHasColorSettingsDraft`. Bind the mode selector to `LightType`. Surface the dynamic region according to the active mode. Use the catalog service to populate selectable sets and save new custom multiple-discrete sets. Route all accepted edits back into `draft.ColorConfiguration`.

Finally, remove the old Tree inline light-type controls from `Props.Runtime/Tree/Wizard/Views/TreePropWizardPageView.xaml`, update summaries, and add tests. The test coverage should mirror existing patterns: discovery tests in `PropCentric.Tests/Discovery`, draft mapping tests alongside the current Tree and PolyLine mapping tests, and new feature-page behavior tests under `PropCentric.Tests/Wizards`.

## Concrete Steps

Run these commands from `C:\Dev\PropCentric`.

1. Inspect the current baseline before editing:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~PropDiscoveryTests|FullyQualifiedName~TreeDraftMappingTests|FullyQualifiedName~PolyLine"

   Expect the current tests to pass and to still reference `StringType`. This confirms the migration target is exercised by tests before any code is changed.

2. After Milestone 1, run focused refactor validation:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~PropDiscoveryTests|FullyQualifiedName~TreeDraftMappingTests|FullyQualifiedName~PolyLine"

   Expect updated assertions that check `PropFeatureFlags.Color`, `IHasColor`, `LightType`, and `ColorConfiguration` instead of `StringType`.

3. After Milestone 2, run catalog tests:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorSet|FullyQualifiedName~ColorCatalog"

   Expect tests proving predefined sets exist and newly saved custom sets can be queried immediately.

4. After Milestone 4, run feature-page tests:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorFeatureWizardPage|FullyQualifiedName~DraftBackedWizardPage|FullyQualifiedName~PropDiscoveryTests"

   Expect tests proving the page is discovered, bound to the shared draft, and persists mode-specific edits.

5. Before considering the feature complete, run the full suite and solution build:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj
       dotnet build PropCentric.sln

6. Perform a manual harness check:

       dotnet run --project PropCentric/PropCentric.csproj

   In the harness, create or edit a Tree and a PolyLine prop. Confirm the reusable Color page appears for both, configure each mode at least once, accept the wizard, reopen the prop, and confirm the saved values persist.

## Validation and Acceptance

Acceptance is behavioral, not just structural.

The implementation is acceptable only when all of the following are true:

1. `TreeProp` and `PolyLineProp` are discovered as supporting `PropFeatureFlags.Color`, and a discovery test proves that by checking `typeof(IHasColor).IsAssignableFrom(...)`.
2. The reusable `ColorFeatureWizardPage` is discovered automatically through `FeatureWizardPageAttribute` and resolved for color-capable props without prop-specific registration code.
3. The old inline `StringType` control is removed from `Props.Runtime/Tree/Wizard/Views/TreePropWizardPageView.xaml`, and no common wizard-page base owns color-selection UI state.
4. Single-color mode allows editing one `System.Drawing.Color`, and that exact color persists through create and edit flows.
5. Multiple-discrete mode allows choosing a predefined set, creating a new uniquely named set, editing its colors with the reusable color picker, and reopening the prop with the same selected set and colors.
6. Full-color mode allows selecting one of the predefined channel-order names and reopens with the same selection.
7. Summary output for Tree and PolyLine describes the selected color mode using the new configuration model.
8. `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` passes, and the new color-focused tests fail before the implementation and pass after it.
9. `dotnet build PropCentric.sln` passes.

## Idempotence and Recovery

The planned changes are additive and refactor-oriented. They can be implemented in small slices and rerun safely. If a milestone stalls midway, the recovery rule is to keep the repository coherent at the type-system level before proceeding. For example, do not leave both `StringTypes` and `LightType` as competing authoritative enums for longer than one short refactor slice. If a draft or prop contract changes, update its mapper and the affected tests in the same slice.

The custom color-set catalog is intentionally in-memory for the POC, so retrying wizard flows during development should not damage persistent state. If a test becomes brittle because the in-memory catalog retains values across test cases, give the service an explicit reset seam or use fresh service instances per test.

## Artifacts and Notes

Important current files to update or replace during implementation include:

- `Docs/color-feature-requirements.md`
- `Props.Abstractions/Features/IHasColor.cs`
- `Props.Abstractions/Props/BaseLightProp.cs`
- `Props.Abstractions/Setup/Drafts/IHasLightSettingsDraft.cs`
- `Props.Runtime/Tree/TreeProp.cs`
- `Props.Runtime/PolyLine/PolyLineProp.cs`
- `Props.Runtime/Tree/Setup/TreePropDraft.cs`
- `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs`
- `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs`
- `Props.Runtime/PolyLine/Setup/PolyLinePropDraftMapper.cs`
- `Props.Runtime/Wizards/Core/Pages/ILightPropWizardPage.cs`
- `Props.Runtime/Wizards/Core/Pages/LightPropWizardPage.cs`
- `Props.Runtime/Wizards/Core/ViewModels/LightWizardPageViewModel.cs`
- `Props.Runtime/Tree/Wizard/Pages/TreePropWizardPage.cs`
- `Props.Runtime/Tree/Wizard/Views/TreePropWizardPageView.xaml`
- `PropCentric.Tests/Discovery/PropDiscoveryTests.cs`

Expected discovery assertions after Milestone 1 should look conceptually like this:

    Assert.True(flags.HasFlag(PropFeatureFlags.Color));
    Assert.True(typeof(IHasColor).IsAssignableFrom(typeof(TreeProp)));

Expected draft-roundtrip assertions should look conceptually like this:

    Assert.Equal(LightType.MultipleDiscreteColors, draft.ColorConfiguration.LightType);
    Assert.Equal("RGBW", draft.ColorConfiguration.DiscreteColorSet!.Name);

## Interfaces and Dependencies

Define or update the following interfaces and types as part of the implementation:

In `Props.Abstractions/Features`, define:

    public enum LightType
    {
        SingleColor,
        MultipleDiscreteColors,
        FullColor
    }

    public interface IHasColor
    {
        LightColorConfiguration ColorConfiguration { get; set; }
    }

    public sealed record DiscreteColorSetDefinition(string Name, IReadOnlyList<System.Drawing.Color> Colors);

    public sealed record FullColorOrderDefinition(string Name, IReadOnlyList<LightColorChannel> Channels);

    public sealed record LightColorConfiguration(
        LightType LightType,
        System.Drawing.Color SingleColor,
        DiscreteColorSetDefinition? DiscreteColorSet,
        FullColorOrderDefinition? FullColorOrder);

    public enum LightColorChannel
    {
        Red,
        Green,
        Blue,
        White
    }

In `Props.Abstractions/Setup/Drafts`, define:

    public interface IHasColorSettingsDraft
    {
        LightColorConfiguration ColorConfiguration { get; set; }
    }

In a feature-oriented abstractions namespace, define a catalog interface similar to:

    public interface IColorConfigurationCatalog
    {
        IReadOnlyList<DiscreteColorSetDefinition> GetDiscreteColorSets();
        IReadOnlyList<FullColorOrderDefinition> GetFullColorOrders();
        void SaveDiscreteColorSet(DiscreteColorSetDefinition colorSet);
    }

In `Props.Runtime/Wizards/Features/Color/Pages`, define:

    [FeatureWizardPage(typeof(IHasColor), priority: 130)]
    public sealed class ColorFeatureWizardPage : WizardPageBase, IFeatureWizardDraftPage

In `Props.Runtime/Wizards/Features/Color/ViewModels`, define:

    public sealed class ColorFeatureWizardPageViewModel : WizardPageViewModelBase<ColorFeatureWizardPage>

In `Props.Runtime/Wizards/Features/Color/Views`, define:

    public partial class ColorFeatureWizardPageView

Keep the UI stack on Catel MVVM and WPF. Keep discovery automatic through the existing startup scanning in `Props.Registry`. Avoid adding prop-specific DI bootstrap methods; the solution already has the right registration pattern for reusable feature infrastructure.

Revision note: created this ExecPlan on 2026-05-24 because `Docs/color-feature-requirements.md` describes a significant feature migration, not a small isolated change, and the repository requires an ExecPlan for this scale of work.
