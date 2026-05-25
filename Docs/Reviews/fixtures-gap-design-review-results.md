# Fixture Design Gap Review

## Purpose

This review answers the question posed by the original task prompt: what is missing, incompatible, or risky if the current PropCentric POC patterns are applied to intelligent fixtures from the Vixen `feature/VIX-3693` branch?

The short answer is that fixture support is feasible within the POC direction, but not as a straight copy of the current Vixen fixture wizard. The biggest gap is not discovery or dependency injection. The biggest gap is architectural: the current POC assumes a single shared draft, explicit mapping layers, and preview built from value-like visual input records, while the current Vixen fixture flow coordinates behavior through page-to-page coupling, shared mutable `FixtureSpecification` state, singleton repositories, and editor controls that live outside the POC pipeline.

## Executive Summary

The current POC already has several pieces that help fixture work:

- startup discovery can already discover new prop pipelines through `Props.Registry/PropServiceCollectionExtensions.cs`
- feature flags already reserve `PropFeatureFlags.Fixture` in `Props.Abstractions/Features/PropFeatureFlags.cs`
- the setup wrapper pattern in `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` already separates wizard orchestration from the prop
- the draft, mapper, visual-input, visual-model-builder, and preview-coordinator patterns are established and working
- `Props.Abstractions/PropVisualModels/PropMesh.cs` already provides a mesh primitive that can represent fixture body geometry

Those pieces are necessary, but they are not sufficient. There is currently no fixture runtime contract, no fixture draft model, no fixture-specific visual pipeline, and no abstraction boundary for the fixture profile catalog/editor behavior that the Vixen wizard depends on.

The required conclusion is:

- fixture support should be added as a new prop pipeline that follows the POC draft/mapping/preview rules
- the current Vixen fixture wizard should be mined for responsibilities and user flow, not copied structurally
- fixture-specific pages are acceptable and expected; props may own their entire wizard flow when reusable feature pages do not add value
- runtime fixture motion such as pan and tilt must stay separate from setup-time baseline `AxisRotations`
- grouping/cloning needs a more explicit place in the POC because the current fixture wizard treats multi-create as a first-class outcome

## Findings

### 1. The current Vixen fixture flow is not draft-first

This is the main architectural mismatch.

The current POC design requires wizard pages to work against a shared draft and preview session rather than directly editing the prop. That pattern is visible in:

- `Props.Runtime/Tree/TreePropSetup.cs`
- `Props.Runtime/PolyLine/PolyLinePropSetup.cs`
- `Props.Runtime/Tree/Setup/TreePropDraft.cs`
- `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs`
- `Props.Runtime/Wizards/Features/Color/Pages/ColorFeatureWizardPage.cs`
- `Props.Runtime/Wizards/Features/Rotation/Pages/RotationFeatureWizardPage.cs`
- `Props.Runtime/Wizards/Features/Segments/Pages/SegmentsFeatureWizardPage.cs`

The current Vixen fixture wizard does not follow that pattern. Multiple pages reach into other pages through `Wizard.Pages.Single(...)`, and several pages directly mutate shared `FixtureSpecification` state:

- `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/SelectProfileWizardPageViewModel.cs`
- `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileWizardPageViewModel.cs`
- `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileFunctionsWizardPageViewModel.cs`
- `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/ColorSupportWizardPageViewModel.cs`
- `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/AutomationWizardPageViewModel.cs`
- `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/DimmingCurveWizardPageViewModel.cs`
- `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/GroupingWizardPageViewModel.cs`

Examples:

- `SelectProfileWizardPageViewModel` clones or creates a `FixtureSpecification` and stores it directly on the page model.
- `EditProfileWizardPageViewModel` pulls that object from `SelectProfileWizardPage`, passes it into a child editor, then writes the edited instance back into `SelectProfileWizardPage`.
- `EditProfileFunctionsWizardPageViewModel` reads and writes `FunctionDefinitions` through `SelectProfileWizardPage.Fixture`.
- `AutomationWizardPageViewModel` derives defaults by reading both `ColorSupportWizardPage` and `SelectProfileWizardPage`.

This coupling is workable in the old wizard, but it does not fit the POC's design goal that wizard pages should not know about props directly and should not depend on sibling page state.

Conclusion: fixture work needs a dedicated shared draft model and mapper layer before any attempt at page parity.

### 2. The POC has a fixture flag, but no fixture contract family yet

`Props.Abstractions/Features/PropFeatureFlags.cs` defines `Fixture = 8`, which means the architecture already expects fixture-like props to exist.

However, there is currently no corresponding runtime feature interface or draft contract family. A search through `Props.Abstractions/` found the flag, but no `IHasFixture...` or `ICan...Fixture...` contracts.

That leaves several fixture-specific concepts without a home:

- fixture profile identity and editable specification
- fixture function definitions
- automation settings such as shutter, prism, color wheel, and dimmer automation
- dimming-curve configuration
- beam/legend/body rendering configuration
- motion constraints such as pan and tilt ranges and travel times

Conclusion: fixture support needs new abstraction contracts, not just a new concrete prop.

### 3. The current POC visual pipeline can host fixture rendering, but the fixture shape is missing

The POC's visual architecture is general enough:

- `IVisualInputMapper<,>` can project prop or draft state into a visual-input record
- `IPropVisualModelBuilder<,>` can build geometry
- `IWizardPreviewCoordinator<>` can cache and rebuild previews
- `PropMesh` in `Props.Abstractions/PropVisualModels/PropMesh.cs` can represent fixture body meshes

That is promising. The missing piece is the fixture-specific visual input shape and builder strategy.

`TreeVisualInput` in `Props.Runtime/Tree/Visuals/TreeVisualInput.cs` shows the intended pattern: capture only rendering-relevant data in an immutable, value-comparable record. `TreeVisualModelBuilder` in `Props.Runtime/Tree/Visuals/TreeVisualModelBuilder.cs` then owns the geometry generation.

The current fixture code does not map cleanly into that shape yet. `IntelligentFixtureModel` in `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureModel.cs` mixes several categories of data:

- persisted baseline configuration such as beam length, transparency, width multiplier, mounting position, strobe rates, and pan/tilt ranges
- automation-related settings that are not inherently geometry
- constraints and behavior that matter to runtime rendering but are not setup-time geometry

Conclusion: fixture support needs an explicit `FixtureVisualInput` design that separates:

- body geometry and static rendering configuration
- baseline setup-time orientation
- runtime motion/render state such as live pan, tilt, shutter, prism, and wheel position

### 4. Runtime motion must stay separate from baseline `AxisRotations`

This is a direct design constraint from the POC docs and it matters more for fixtures than for trees or polylines.

`Docs/core-design-goals.md` already states that setup-time `AxisRotations` are baseline prop-definition state and must not be conflated with runtime rendered motion such as fixture pan, tilt, or elevation. The current fixture model in `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureModel.cs` contains pan/tilt range and timing data, which are clearly fixture-motion concepts rather than baseline setup rotations.

That means fixture support cannot simply reuse `ICanAxisRotate` to represent moving-head behavior. At most, a fixture prop might also support baseline `AxisRotations` for how the fixture is mounted in the scene, but that is a separate concern from live movement.

Conclusion: fixture support needs a separate motion-oriented contract or render-state concept. If that concept is not introduced now, the fixture draft and visual-input design must still reserve a clean boundary for it.

### 5. The current fixture wizard depends on infrastructure that is outside the POC seams

The Vixen fixture flow depends on several concrete services and editors that are not represented in the current POC:

- `FixtureSpecificationManager.Instance()` in `SelectProfileWizardPageViewModel`
- `FixturePropertyEditorViewModel` in `EditProfileWizardPageViewModel`
- `FunctionTypeViewModel` in `EditProfileFunctionsWizardPageViewModel`
- `CurveEditor` from WinForms in `DimmingCurveWizardPageViewModel`

This has two implications.

First, fixture support in the POC cannot be finished by only adding a prop and a few wizard pages. It also needs adapter boundaries for the external fixture-profile catalog and profile-editing behavior.

Second, the current fixture wizard is not a clean fit for the POC's Catel MVVM direction. From a `catel-mvvm` perspective, the heavy use of page-to-page lookups and singleton access is a smell. The POC should move these responsibilities behind injected services and draft mappers rather than carrying forward the same coupling.

Conclusion: introduce provider-style abstractions for profile catalog access and profile editing inputs before trying to mirror the full wizard.

### 6. Grouping and multi-create are under-modeled in the current POC

`IPropSetup.CreateAsync(...)` already returns `IPropGroup?`, so the POC clearly expects setup flows to be able to create more than one prop.

In practice, the current concrete implementations do not really exercise that capability:

- `Props.Runtime/Tree/TreePropSetup.cs` creates one `TreeProp` and contains a TODO for grouping
- `Props.Runtime/PolyLine/PolyLinePropSetup.cs` creates one `PolyLineProp`

The current Vixen fixture wizard treats multi-create and grouping as a real part of the flow:

- `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/GroupingWizardPageViewModel.cs`

That page models:

- fixture count
- element prefix
- optional grouping root
- preview tree of the created fixture set

Conclusion: fixture support is likely to be the first real consumer of `IPropGroup`, so the grouping/cloning outcome should be designed explicitly instead of left as a TODO.

### 7. Fixture-specific pages are acceptable; reusable feature pages help only where they add value

There is some reuse potential, but it is limited, and that is not a design failure.

The current docs now explicitly allow a prop to own and control its full wizard flow:

- `Docs/core-design-goals.md`
- `Docs/feature-wizards-requirements.md`
- `Docs/poc-system-overview.md`

That means fixture setup does not need to be judged by how many pages it can share with other props. The real question is whether fixture pages can participate in the same draft, mapper, preview, and commit architecture. On that question, the answer is yes, but only after the fixture-specific state model is formalized.

The current POC reusable feature pages cover:

- color via `Props.Runtime/Wizards/Features/Color/Pages/ColorFeatureWizardPage.cs`
- dimming via `Props.Runtime/Wizards/Features/Dimming/Pages/DimmingFeatureWizardPage.cs`
- segments via `Props.Runtime/Wizards/Features/Segments/Pages/SegmentsFeatureWizardPage.cs`
- baseline rotation via `Props.Runtime/Wizards/Features/Rotation/Pages/RotationFeatureWizardPage.cs`

The current fixture wizard pages fall into two groups.

Pages that might align with reusable feature patterns:

- color support
- some dimming behavior
- possibly grouping if grouping becomes a general prop-level capability

Pages that are fixture-specific:

- select or create profile
- edit profile channels
- edit profile functions
- automation settings tied to supported fixture functions

The color overlap is also not exact. `ColorSupportWizardPageViewModel` only classifies the fixture into color mixing, color wheel, or no color support. The POC color page edits a reusable `LightColorConfiguration`. Those are related, but not equivalent, concepts.

Conclusion: expect a hybrid solution. Reuse the POC feature-page pattern where it actually fits, but keep fixture profile editing as a fixture-specific pipeline.

## Comparison By Concern

### Concern: Discovery and DI registration

Current POC status:

- supported
- evidence: `Props.Registry/PropServiceCollectionExtensions.cs`, `Props.Registry/PropFeatureInferrer.cs`

Gap:

- only the fixture-specific contracts and implementations are missing

Recommendation:

- no discovery redesign is needed
- add a fixture prop pipeline under a scanned `Props*` assembly and let existing discovery register it

### Concern: Persisted runtime prop state

Current POC status:

- partially supported

Evidence:

- prop-owned state pattern exists in `TreeProp` and `PolyLineProp`
- no fixture-specific runtime contracts exist today

Gap:

- no canonical home for fixture profile, function, automation, or motion-constraint state

Recommendation:

- introduce fixture runtime contracts before implementing a concrete prop

### Concern: Wizard state ownership

Current POC status:

- supported with adaptation

Evidence:

- shared draft pattern in `TreePropSetup`, `PolyLinePropSetup`, and the draft-backed feature pages

Gap:

- current fixture wizard behavior is page-coupled rather than draft-backed

Recommendation:

- design a `FixturePropDraft` first
- do not port the current page coupling

### Concern: Prop-specific multi-page wizard flow

Current POC status:

- already supported

Evidence:

- `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` already let the setup wrapper assemble prop-specific page flow
- `Docs/core-design-goals.md`, `Docs/feature-wizards-requirements.md`, and `Docs/poc-system-overview.md` now explicitly state that props may own their full wizard flow and use feature pages only when those pages add value

Gap:

- no architectural gap; only fixture-specific implementation work remains

Recommendation:

- keep fixture pages fixture-specific where that produces the clearest flow
- use reusable feature pages selectively, not by default

### Concern: Preview and geometry generation

Current POC status:

- partially supported

Evidence:

- generic preview pipeline exists
- `PropMesh` exists for fixture-like mesh geometry

Gap:

- no fixture visual input, no fixture visual model builder, no preview coordinator

Recommendation:

- define a minimal fixture visual slice early, even if the first version only renders body orientation plus one emitter/beam placeholder

### Concern: Runtime motion

Current POC status:

- unsupported as a dedicated concept

Evidence:

- docs explicitly separate runtime motion from setup-time `AxisRotations`
- no fixture-motion abstraction currently exists

Gap:

- fixtures need motion semantics that are not the same as baseline rotation

Recommendation:

- define a separate fixture motion concept or reserve a clear adapter boundary for it

### Concern: Group creation

Current POC status:

- contract present, behavior mostly unimplemented

Evidence:

- `IPropGroup` return type exists
- current setup wrappers still create one prop each

Gap:

- fixture wizard expects real multi-create behavior

Recommendation:

- treat fixture implementation as the forcing function to finish the grouping/cloning path

## Recommended Implementation Order

### Step 1: Introduce fixture abstractions before any full wizard port

Add the contract family that the current POC does not have yet. The exact names can be refined during implementation, but the design needs explicit interfaces for:

- persisted fixture definition/profile state
- fixture automation configuration
- fixture motion constraints separate from baseline `AxisRotations`
- setup-only draft capabilities for fixture editing

Candidate names:

- `IHasFixtureProfile`
- `IHasFixtureAutomation`
- `IHasFixtureMotionConstraints`
- `IHasFixtureProfileDraft`

These names are recommendations, not finalized requirements.

### Step 2: Create a dedicated fixture draft and draft mapper

This is the first critical implementation slice.

The draft should centralize the state that the current Vixen wizard spreads across pages:

- selected or newly created profile
- editable profile metadata
- editable function definitions
- color support mode
- automation choices
- dimming-curve choices
- grouping/count metadata

The mapper should handle:

- prop -> draft for edit
- draft -> prop for commit

Without this step, the rest of the fixture work will reproduce the current page coupling and fight the POC architecture.

### Step 3: Introduce a provider boundary for fixture profiles and editor dependencies

Do not let the new POC fixture pages call `FixtureSpecificationManager.Instance()` or depend directly on editor-specific controls.

Instead, introduce injected abstractions such as:

- a profile catalog provider
- a profile cloning/creation service
- a function-definition editor adapter if the existing editor must be reused
- a dimming-curve editor abstraction if curve editing remains part of the flow

This keeps the setup wrapper and pages testable and avoids carrying singletons into the new design.

### Step 4: Build a minimal fixture visual slice early

Before trying to reach full wizard parity, implement a minimal fixture preview path:

- `FixtureVisualInput`
- `FixturePropToVisualInputMapper`
- `FixtureDraftToVisualInputMapper`
- `FixtureVisualModelBuilder`
- `FixtureWizardPreviewCoordinator`

The first version does not need full moving-head animation. It only needs enough rendering to prove the architecture:

- fixture body mesh or placeholder mesh
- emitter/light origin
- baseline setup orientation
- optional beam placeholder if that is cheap to model

This step de-risks the largest missing rendering seam.

### Step 5: Implement fixture-specific wizard pages, then reuse feature pages selectively

Fixture-specific pages should cover:

- profile selection/creation
- profile editing
- function editing
- automation setup
- grouping/multi-create

Reusable feature pages can then be reused where the concepts genuinely match:

- color, only if the fixture draft can project into `LightColorConfiguration` in a meaningful way
- dimming, only if generic dimming semantics are sufficient
- baseline rotation, only for mount orientation, not for live pan/tilt
- no feature page at all, when a fixture-specific page is clearer than mixed composition

### Step 6: Finish `IPropGroup` cloning semantics

The fixture flow is likely to be the first real case where `CreateAsync(...)` should return a group containing multiple same-shape props with different names. That logic should be implemented as part of the fixture slice rather than postponed again.

## What Does Not Need To Change

The review did not find any evidence that the following core POC decisions should be reversed:

- startup discovery through assembly scanning
- feature inference through interfaces
- setup wrappers as the orchestration boundary
- draft-backed wizard editing
- visual-input records plus builders
- preview coordinators and shared preview sessions

Those patterns still look correct. Fixture work should extend them, not bypass them.

## Validation Notes

This review did not require a code spike.

Source inspection was sufficient because the repository already proves the generic mechanics that a fixture slice would need:

- automatic discovery and DI registration in `Props.Registry/PropServiceCollectionExtensions.cs`
- feature inference in `Props.Registry/PropFeatureInferrer.cs`
- prop-specific wizard orchestration in `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs`
- shared draft mapping in `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs` and `Props.Runtime/PolyLine/Setup/PolyLinePropDraftMapper.cs`
- preview rebuild flow in `Props.Runtime/Tree/Visuals/TreeWizardPreviewCoordinator.cs` and `Props.Runtime/PolyLine/Visuals/PolyLineWizardPreviewCoordinator.cs`

The unresolved issues are contract-shape and responsibility-boundary questions, not feasibility questions. The review therefore recommends moving directly to a fixture implementation ExecPlan rather than spending time on a narrow prototype.

## Follow-Up Plan

1. Add fixture abstraction contracts under `Props.Abstractions` and setup-only fixture draft contracts under `Props.Abstractions.Setup.Drafts`.
2. Add a first `FixtureProp`, `FixturePropSetup`, `FixturePropDraft`, and `FixturePropDraftMapper` pipeline that proves prop creation and edit flow without full Vixen fixture parity.
3. Add provider/editor adapter abstractions for profile catalog access, profile creation/cloning, function-definition editing, and dimming-curve editing.
4. Add the first fixture preview slice with `FixtureVisualInput`, mappers, builder, and `FixtureWizardPreviewCoordinator`.
5. Implement fixture-specific wizard pages for profile, function, automation, and grouping workflows against the shared draft.
6. Decide case by case whether any existing feature pages should participate for color, dimming, or baseline rotation.
7. Complete `IPropGroup` multi-create behavior as part of the fixture implementation slice.

## Final Assessment

The current POC is close enough in architecture to support fixtures, but not close enough to accept the existing Vixen fixture wizard unchanged.

The main blockers are:

- missing fixture contract families
- missing fixture draft and mapper pipeline
- missing fixture visual pipeline
- missing abstraction boundary around existing fixture profile/editor dependencies
- unfinished group/clone behavior in `IPropGroup` flows

The good news is that none of those blockers require throwing away the current POC direction. They require extending it in the areas where fixtures are more stateful, more profile-driven, and more runtime-motion-aware than the tree and polyline examples.

That means the pattern is still viable for incorporation into the Vixen feature branch, but only if fixture support is treated as a first-class architectural slice instead of a direct wizard port.
