# Review Fixture Gaps Against The PropCentric POC

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

After this work, a contributor should be able to answer a specific question with evidence instead of opinion: what is missing, incompatible, or risky if the current PropCentric POC patterns are applied to intelligent fixtures from the Vixen `feature/VIX-3693` codebase? The outcome is not "fixture support is implemented." The outcome is a design review that names the real gaps, shows them in the code, and recommends the smallest credible path to close them.

The user-visible proof of success is a checked-in design review update that compares the current POC architecture with the existing fixture implementation, identifies the required new abstractions and workflow changes, and, if necessary, includes a narrow prototype that demonstrates the hardest unresolved point. A follow-up implementer should be able to start fixture work directly from that review and this plan without having to rediscover the same architecture questions.

## Progress

- [x] (2026-05-22 10:05 -05:00) Read `Docs/fixtures-gap-design-review.md`, `.agents/PLANS.md`, and the core POC docs needed to understand the current architecture and planning format.
- [x] (2026-05-22 10:12 -05:00) Inspected the local Vixen fixture reference area under `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture` and `Vixen/src/Vixen.Modules/Editor/FixtureWizard` to confirm the referenced code is available in this workspace.
- [x] (2026-05-25 14:49 -05:00) Re-ran Concrete Step 1 by re-reading `Docs/fixtures-gap-design-review.md`, `Docs/poc-system-overview.md`, `Docs/core-design-goals.md`, `Docs/feature-wizards-requirements.md`, and `Docs/naming-conventions.md` before continuing the fixture inventory work.
- [x] (2026-05-25 14:52 -05:00) Completed the runtime-type portion of the fixture inventory by reading `IntelligentFixtureProp.cs`, `IntelligentFixtureModel.cs`, and `YesNo.cs` under `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture`, confirming the persisted state shape and the prop-to-model wrapper pattern used by the current fixture implementation.
- [x] (2026-05-25 14:55 -05:00) Completed the wizard-shape portion of the Vixen fixture inventory by reading `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/IntelligentFixtureWizard.cs` and enumerating the `Models`, `ViewModels`, and `Views` folders to map the current page sequence and supporting wizard types.
- [x] (2026-05-25 15:00 -05:00) Completed the coupling/state-flow and external-dependency portion of the Vixen fixture inventory by searching `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard` for sibling-page lookups and opening the key view models that own profile selection, profile editing, function editing, color support, automation, dimming curves, and grouping behavior.
- [x] (2026-05-25 15:08 -05:00) Completed the POC seam inventory by reading `Props.Registry/PropsServiceCollectionExtensions.cs`, `Props.Registry/PropFeatureInferrer.cs`, the `Tree` and `PolyLine` prop/setup/draft/mapper/preview files, and the current reusable feature wizard pages under `Props.Runtime/Wizards/Features`.
- [x] (2026-05-25 15:13 -05:00) Wrote the gap-analysis classification in working form by mapping the major fixture concerns to `already supported`, `supported with adaptation`, and `missing architecture` based on the completed Vixen and POC inventories.
- [x] (2026-05-25 15:16 -05:00) Decided that no code spike is required for this review because the highest-risk questions are now answerable from source evidence: the POC already proves the setup/draft/preview seams, and the remaining fixture blockers are missing contract boundaries rather than uncertain library or rendering feasibility.
- [x] (2026-05-25 15:07 -05:00) Added documentation clarifications to `Docs/poc-system-overview.md`, `Docs/feature-wizards-requirements.md`, and `Docs/core-design-goals.md` stating that props may own their full wizard page flow and should use feature pages only when those pages add real value.
- [x] (2026-05-25 15:20 -05:00) Wrote the completed review to `Docs/Reviews/fixtures-gap-design-review-results.md`, incorporating the final gap classifications, the prop-owned wizard-flow clarification, validation notes explaining why no spike was needed, and a sequenced follow-up implementation plan while preserving `Docs/fixtures-gap-design-review.md` as the original prompt file.

## Surprises & Discoveries

- Observation: the fixture reference code is already present in the local repository under `Vixen/`, so the design review can rely on concrete source inspection without needing network access.
  Evidence: `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureProp.cs` and `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/IntelligentFixtureWizard.cs` are available locally.

- Observation: the current Vixen intelligent fixture wizard appears to coordinate state through page-to-page coupling and shared mutable objects rather than through a single draft model.
  Evidence: `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/AutomationWizardPageViewModel.cs` and `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileWizardPageViewModel.cs` query other wizard pages directly via `Wizard.Pages.Single(...)`.

- Observation: the current POC already reserves a fixture feature flag, but there is no concrete fixture pipeline in `Props.Runtime` yet.
  Evidence: `Props.Abstractions/Features/PropFeatureFlags.cs` defines `Fixture = 8`, while a repository search across `Props.Runtime` did not reveal any fixture prop implementation.

- Observation: the later feature-wizard cleanup removed mapper-backed feature pages from the supported architecture, which simplifies the fixture review by eliminating a second feature-page backing model.
  Evidence: `Docs/Plans/feature-wizard-draft-unification-execplan.md` records deletion of `IFeatureWizardDataMapper`, removal of `GetMappersFor(...)`, and convergence on `FeatureWizardContext` plus shared draft-backed feature pages.

- Observation: the current fixture runtime prop is a thin property-grid wrapper over `IntelligentFixtureModel`; most persisted state lives on the model and is exposed through simple forwarding properties on the prop.
  Evidence: `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureProp.cs` constructs `new IntelligentFixtureModel()` in the default constructor and each public fixture setting simply reads/writes a matching `PropModel` property such as `BeamLength`, `PanStartPosition`, and `MountingPosition`.

- Observation: the current fixture model mixes multiple state categories in one persisted object: beam rendering settings, motion constraints, automation timing, legend/display settings, and mounting/inversion options.
  Evidence: `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureModel.cs` stores `BeamLength`, `BeamTransparency`, `BeamWidthMultiplier`, `PanStartPosition`, `PanStopPosition`, `TiltStartPosition`, `TiltStopPosition`, `MaxPanTravelTime`, `MaxTiltTravelTime`, `StrobeRateMinimum`, `StrobeRateMaximum`, `MaximumStrobeDuration`, `MinColorWheelRotationSpeed`, `MaxColorWheelRotationSpeed`, `ShowLegend`, `ZoomNarrowToWide`, `InvertPanDirection`, `InvertTiltDirection`, and `MountingPosition` together.

- Observation: the runtime fixture code already shows maintenance risk through inconsistencies and likely defects that the review should not preserve when designing the POC fit.
  Evidence: `IntelligentFixtureProp.cs` lives in namespace `VixenModules.App.Props.Models.Arch`, while `IntelligentFixtureModel.cs` uses `VixenModules.App.Props.Models.IntellligentFixture` with a triple-`l` spelling; the model constructor also assigns `TiltStartPosition = DefaultTiltStopPosition` instead of initializing `TiltStopPosition`.

- Observation: the current fixture wizard is a fixed, hand-composed page flow rather than a feature-resolved composition model.
  Evidence: `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/IntelligentFixtureWizard.cs` adds pages explicitly in the constructor in this order: `SelectProfileWizardPage`, `EditProfileFunctionsWizardPage`, `EditProfileWizardPage`, `ColorSupportWizardPage`, `AutomationWizardPage`, `DimmingCurveWizardPage`, `GroupingWizardPage`, and `SummaryWizardPage`.

- Observation: the fixture wizard already separates responsibilities into clearly named page families, which makes the later comparison against POC prop-specific pages versus reusable feature pages more concrete.
  Evidence: the `Wizard/Models`, `Wizard/ViewModels`, and `Wizard/Views` folders contain distinct artifacts for profile selection, profile editing, color support, automation, dimming curves, and grouping, plus shared base types such as `FixtureWizardPageBase`, `SummaryWizardPageBase`, `EditWizardPageViewModelBase`, and `IIntelligentFixtureWizardPageViewModel`.

- Observation: fixture setup pages should be evaluated primarily as fixture-specific setup flow, not as candidate reusable feature pages, even when they touch concerns like automation, grouping, or profile editing.
  Evidence: user clarification on 2026-05-25 states that the fixture setup pages are intentionally specific to fixtures and that it is acceptable for a prop to own multiple setup pages exclusively, unlike broadly reusable pages such as dimming or color.

- Observation: navigation policy is partly delegated to page view models instead of living only in wizard metadata.
  Evidence: `IntelligentFixtureWizard.cs` overrides `CanMoveBack` and `CanMoveForward` and consults `IIntelligentFixtureWizardPageViewModel.CanMoveBack()` and `CanMoveNext()` on the current page view model before allowing navigation.

- Observation: the fixture wizard coordinates most state through sibling-page lookups against `Wizard.Pages.Single(...)` instead of through a single shared draft object.
  Evidence: `SelectProfileWizardPageViewModel.cs`, `EditProfileWizardPageViewModel.cs`, `EditProfileFunctionsWizardPageViewModel.cs`, `ColorSupportWizardPageViewModel.cs`, `AutomationWizardPageViewModel.cs`, `DimmingCurveWizardPageViewModel.cs`, and `GroupingWizardPageViewModel.cs` all read from or write to other pages through `Wizard.Pages.Single(...)`.

- Observation: `SelectProfileWizardPageViewModel` acts as the effective shared state hub for the current wizard by owning both the selected profile name and the mutable `FixtureSpecification` being edited.
  Evidence: in `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/SelectProfileWizardPageViewModel.cs`, choosing an existing profile clones it from `FixtureSpecificationManager.Instance().FixtureSpecifications`, choosing a new profile creates `new FixtureSpecification()` plus `InitializeBuiltInFunctions()`, and later pages read or overwrite `selectProfilePage.Fixture` and `selectProfilePage.SelectedFixture`.

- Observation: profile editing and function editing mutate the same shared `FixtureSpecification` through embedded editor view models rather than through an explicit mapper layer.
  Evidence: `EditProfileWizardPageViewModel.cs` passes `selectProfilePage.Fixture` into a `FixturePropertyEditorViewModel`, then saves by writing the returned specification back onto `selectProfilePage.Fixture`; `EditProfileFunctionsWizardPageViewModel.cs` passes `selectProfilePage.Fixture.FunctionDefinitions` into a `FunctionTypeViewModel` and saves by reassigning `selectProfilePage.Fixture.FunctionDefinitions = childVM.GetFunctionData()`.

- Observation: several later pages derive defaults from earlier pages instead of from a centralized state contract, which creates direct page-order coupling.
  Evidence: `ColorSupportWizardPageViewModel.cs` classifies color support from `selectProfilePage.Fixture`; `AutomationWizardPageViewModel.cs` combines `ColorSupportWizardPage` flags with `selectProfilePage.Fixture.SupportsFunction(...)`; `DimmingCurveWizardPageViewModel.cs` enables per-color curves from `colorSupportPage.ColorMixing`; `GroupingWizardPageViewModel.cs` seeds `ElementPrefix` from `selectProfilePage.ProfileName`.

- Observation: the current fixture wizard depends on external singleton/services and non-POC editor controls that would need explicit abstraction boundaries in the draft-based POC architecture.
  Evidence: `SelectProfileWizardPageViewModel.cs` calls `FixtureSpecificationManager.Instance()` directly; `EditProfileWizardPageViewModel.cs` and `EditProfileFunctionsWizardPageViewModel.cs` depend on `VixenModules.Editor.FixturePropertyEditor.ViewModels`; `EditProfileWizardPageView.xaml` hosts `FixturePropertyEditorView`; `DimmingCurveWizardPageViewModel.cs` launches a WinForms `CurveEditor` via `ShowDialog()`.

- Observation: the current POC already has the exact extension seams fixture support would need for discovery, setup orchestration, draft mapping, visual-input projection, visual-model building, and preview coordination.
  Evidence: `Props.Registry/PropsServiceCollectionExtensions.cs` auto-registers implementations of `IVisualInputMapper<,>`, `IPropVisualModelBuilder<,>`, `IPropDraftMapper<,>`, and `IWizardPreviewCoordinator<>`; `Props.Registry/PropFeatureInferrer.cs` derives feature flags from feature interfaces marked with `PropFeatureAttribute`.

- Observation: the setup-wrapper pattern already supports props owning multiple prop-specific pages while still composing in reusable feature pages through one shared draft and preview session.
  Evidence: `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` each create a prop-specific wizard page, create one shared draft, create `WizardPreviewSession<TDraft>`, initialize applicable feature pages with `FeatureWizardContext`, and then commit through the prop draft mapper if the wizard is accepted.

- Observation: the POC treats preview-driving input as an explicit snapshot-based transfer contract, not as the prop or draft itself.
  Evidence: `TreePropToVisualInputMapper.cs`, `TreeDraftToVisualInputMapper.cs`, `PolyLinePropToVisualInputMapper.cs`, and `PolyLineDraftToVisualInputMapper.cs` all project prop or draft state into dedicated visual-input records and explicitly snapshot mutable rendering data such as axis rotations or segment lists before preview/build steps.

- Observation: the current preview coordinators already encode the value-based caching and serialized rebuild behavior that fixture preview would need to join.
  Evidence: `TreeWizardPreviewCoordinator.cs` and `PolyLineWizardPreviewCoordinator.cs` map drafts to visual input, guard rebuilds with `SemaphoreSlim`, and reuse the previous visual model when the new input compares equal to the last input.

- Observation: reusable feature pages in the POC are shared-draft participants rather than alternate state owners, which reinforces that fixture-specific pages can remain exclusive as long as they edit one shared draft cleanly.
  Evidence: `ColorFeatureWizardPage.cs`, `DimmingFeatureWizardPage.cs`, `RotationFeatureWizardPage.cs`, and `SegmentsFeatureWizardPage.cs` all implement `IFeatureWizardDraftPage`, require the shared draft to implement the corresponding setup-only draft contract, and then read or mutate draft-owned state directly rather than querying sibling pages.

- Observation: the current concrete props still underexercise `IPropGroup` and multi-create behavior, which means fixture support would likely be the first real forcing function for that contract.
  Evidence: `TreePropSetup.cs` returns a `PropGroup` containing one prop and carries a TODO about grouping page results; `PolyLinePropSetup.cs` also returns a single-prop `PropGroup` after commit.

- Observation: the POC seam inventory also exposed small repository drift in file layout relative to the ExecPlan commands, but the architectural pattern remains intact.
  Evidence: the current setup-wrapper files live at `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` rather than under `Setup/`, while their draft mappers and draft types still live under `Setup/`.

- Observation: after the full comparison, the strongest conclusion is that fixture support is blocked more by missing fixture-specific contracts than by missing generic POC infrastructure.
  Evidence: discovery, setup orchestration, draft mapping, visual-input projection, and preview coordination all already exist in `Props.Registry/` and the `Tree`/`PolyLine` pipelines, while no current POC contract family represents fixture profile state, fixture automation state, fixture motion constraints, or fixture-specific multi-create/grouping semantics.

- Observation: some fixture concerns are clearly classifiable as "supported with adaptation" rather than "missing architecture", especially where the POC already has the right orchestration shape but not the fixture-specific data model.
  Evidence: prop-specific multi-page setup is already normal in `TreePropSetup.cs` and `PolyLinePropSetup.cs`; color and dimming are already modeled as shared-draft reusable feature participation in `ColorFeatureWizardPage.cs` and `DimmingFeatureWizardPage.cs`; the missing work is fitting fixture-specific data into those same seams without sibling-page coupling.

- Observation: the remaining open questions are design-boundary questions, not feasibility questions that require a prototype to answer.
  Evidence: the current repository already demonstrates all generic mechanics needed for a new prop slice—automatic registration, setup wrappers, draft mappers, feature-page initialization, visual-input projection, and preview rebuilds—while the unresolved fixture work is about defining the right fixture-specific contracts and adapters for profile, automation, motion, and grouping concerns.

## Decision Log

- Decision: this task will be treated as a design-and-feasibility review, not as a direct fixture implementation effort.
  Rationale: `Docs/fixtures-gap-design-review.md` asks for discovery of gaps and impediments before trying to clone a larger portion of the Vixen fixture code into the POC.
  Date/Author: 2026-05-22 / Codex

- Decision: the local `Vixen/` source tree will be treated as the primary reference for current fixture behavior instead of relying on the GitHub links in the request.
  Rationale: the required code is available in the workspace, which keeps the review reproducible and avoids drift from external branch changes during the analysis.
  Date/Author: 2026-05-22 / Codex

- Decision: the review will explicitly compare fixture behavior against the POC's established draft/mapping/preview pipeline rather than evaluating fixture code in isolation.
  Rationale: the task is about whether the POC pattern can be incorporated into the Vixen feature branch with minimal disruption while preserving the current design goals.
  Date/Author: 2026-05-22 / Codex

- Decision: the fixture review should treat reusable feature pages as draft-backed only and should not spend effort evaluating any mapper-backed feature-page migration path.
  Rationale: that migration path no longer exists in production code, so retaining it in the fixture analysis would create false optionality and overstate the solution space.
  Date/Author: 2026-05-25 / Codex

- Decision: the runtime inventory will treat `IntelligentFixtureProp`, `IntelligentFixtureModel`, and `YesNoType` as the authoritative persisted fixture-state slice for the Vixen side, with editor and wizard classes analyzed separately in the next inventory step.
  Rationale: those files define the concrete persisted settings and reveal the current ownership boundary between prop wrappers and underlying model state, which is the essential baseline before evaluating wizard flow and external dependencies.
  Date/Author: 2026-05-25 / Codex

- Decision: the wizard inventory will treat `IntelligentFixtureWizard.cs` as the authoritative source for the fixture flow order and use the folder inventories under `Wizard/Models`, `Wizard/ViewModels`, and `Wizard/Views` to classify which responsibilities are fixture-specific versus potentially reusable.
  Rationale: the constructor-level page sequence shows the intended user journey directly, while the folder structure identifies the supporting types that will matter when comparing this flow to the POC's prop-page plus feature-page composition model.
  Date/Author: 2026-05-25 / Codex

- Decision: the completed Vixen-side inventory will treat `SelectProfileWizardPageViewModel` as the current de facto draft/state owner for comparison purposes, even though it is implemented as a wizard page model plus shared mutable `FixtureSpecification` rather than as an explicit draft contract.
  Rationale: the key architectural question is how current fixture state moves through the wizard. The source inspection now shows that most other pages either initialize from or write back into `SelectProfileWizardPage`, so modeling that page as the current state hub makes the later POC comparison more precise.
  Date/Author: 2026-05-25 / Codex

- Decision: the gap analysis will not treat fixture-specific wizard pages as a design problem simply because they are not reusable across other prop types.
  Rationale: the architectural requirement is shared-draft and mapper-based setup flow, not universal page reuse. The user clarified that fixture setup is expected to own several pages that remain exclusive to fixtures, while only truly cross-cutting concerns such as generic dimming or color should be evaluated for reusable feature-page fit.
  Date/Author: 2026-05-25 / Codex

- Decision: the POC comparison baseline will use `TreePropSetup` and `PolyLinePropSetup` as the primary orchestration references, with the feature pages under `Props.Runtime/Wizards/Features` used only to define what “shared-draft reusable page” means in the current architecture.
  Rationale: the user clarified that fixture setup can legitimately own multiple fixture-specific pages, so the key comparison is whether fixtures can plug into the POC setup/draft/preview contracts, not whether the fixture flow can be collapsed into the reusable feature-page subset.
  Date/Author: 2026-05-25 / Codex

- Decision: the gap analysis will classify fixture concerns according to the smallest credible change needed to fit the current POC architecture: `already supported` if the seam exists and only concrete implementation is missing, `supported with adaptation` if the seam exists but the fixture state model or workflow must be reshaped to use it, and `missing architecture` if no current POC contract cleanly represents the concern.
  Rationale: the review goal is to recommend the minimum architecture expansion needed for fixtures without overstating either compatibility or incompatibility. This classification rule keeps the review outcome concrete and implementation-oriented.
  Date/Author: 2026-05-25 / Codex

- Decision: this design review will not add a prototype or code spike.
  Rationale: the hardest questions originally identified—whether fixture setup can participate in the shared-draft flow, whether fixture preview can fit the visual-input/builder pipeline, and whether fixture-specific multi-page setup is acceptable—are now answered by repository evidence plus user clarification. What remains unresolved is the naming and placement of new fixture-specific contracts, which is better handled in the review document and a follow-up implementation plan than in a narrow spike.
  Date/Author: 2026-05-25 / Codex

- Decision: the follow-up documentation should explicitly state that a prop may own its full wizard flow and that feature pages are optional composition tools, not required architectural participation points.
  Rationale: the user clarified that fixture setup pages are intentionally fixture-specific and that props should not be forced to adopt reusable feature pages when those pages do not add value. The architecture requirement is clean setup orchestration around shared draft state, not mandatory feature-page composition.
  Date/Author: 2026-05-25 / Codex

## Outcomes & Retrospective

At plan creation time, the review itself has not been written yet, but the initial evidence is already useful. The local workspace contains both the POC and the current Vixen intelligent-fixture implementation, so the task can be grounded in real code instead of inferred behavior. Early inspection also shows a likely fault line: the POC prefers a shared wizard draft and explicit mapping layers, while the current fixture wizard appears to use page-to-page coordination and direct shared object mutation.

The later feature-wizard unification work reduces ambiguity in that comparison. Fixture analysis no longer needs to ask whether special-case feature pages should be mapper-backed or draft-backed inside the POC. The answer is now fixed: reusable feature pages participate through shared draft state initialized with `FeatureWizardContext`, and persistence flows back through prop draft mappers.

The runtime inventory added a second concrete takeaway. The current Vixen fixture runtime does not yet separate persisted concerns into distinct layers; it stores visual-beam settings, motion limits, automation timing, and editor-facing display toggles on one `IntelligentFixtureModel`, then exposes those values on `IntelligentFixtureProp` primarily for property-grid editing. That confirms the review needs to distinguish which of those settings belong on a future POC prop, which belong only in setup draft state, and which will eventually need a runtime rendered-state concept.

The wizard-shape inventory adds a matching UI-side takeaway. The existing fixture flow is not a small prop wizard with one or two reusable add-on pages; it is an eight-step, fixture-specific flow whose major responsibilities are profile selection, profile editing, function editing, capability classification, automation choices, dimming-curve configuration, grouping, and summary. That means the later gap analysis needs to judge each of those steps independently instead of treating "fixture wizard" as one indivisible block.

The coupling inventory makes the central mismatch explicit. The current fixture wizard already behaves as though it has shared working state, but that state is informal: it lives on `SelectProfileWizardPage` as a mutable `FixtureSpecification` plus scattered page-owned booleans and curve/grouping settings, and pages discover it by directly querying sibling pages. The POC comparison should therefore focus less on whether fixtures can use a shared draft at all and more on how to replace this implicit, page-coupled shared state with an explicit `FixturePropDraft`, mapper layer, and injected service boundaries.

User guidance also sharpened the review boundary: fixture pages do not need to become generic feature pages to fit the POC. The real requirement is that fixture-specific pages participate cleanly in the same setup architecture as other props. That shifts the review emphasis toward state ownership, service boundaries, preview contracts, and commit flow rather than toward forced UI reuse.

The POC seam inventory now shows that most of the needed architecture already exists in the right places. Discovery is interface-driven and automatic. Setup wrappers already own prop-specific wizard composition. Draft mappers already isolate prop-versus-wizard state. Preview coordinators already rebuild from snapshot-style visual input. The remaining work is therefore unlikely to be a registry or DI problem; it is much more likely to be a contract-shape and state-boundary problem around fixture profiles, motion, automation, and multi-create semantics.

The classification work sharpens that further:

- Already supported:
  discovery and DI registration through `Props.Registry/PropsServiceCollectionExtensions.cs`;
  feature-flag inference via `Props.Registry/PropFeatureInferrer.cs`;
  prop-specific multi-page setup orchestration through the `IPropSetup` pattern in `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs`;
  draft-to-preview rebuild flow through `IWizardPreviewCoordinator<>` and `WizardPreviewSession<TDraft>`.

- Supported with adaptation:
  fixture-specific pages can fit the current setup-wrapper architecture, but their current page-to-page state sharing must be replaced with one shared draft;
  color and dimming concepts may partially reuse existing feature-page patterns, but only after fixture-specific draft state is projected onto the shared draft contracts cleanly;
  preview generation can use the existing visual-input plus builder pipeline, but it needs a fixture-specific visual input shape that separates baseline definition data from runtime-like motion concerns.

- Missing architecture:
  no current contract family models fixture profile ownership, fixture function definitions, or fixture automation settings;
  no current POC abstraction cleanly represents fixture motion constraints as a concept distinct from baseline `AxisRotations`;
  `IPropGroup` exists, but the real grouping/cloning semantics expected by the fixture wizard are not yet exercised by any current prop implementation;
  no adapter boundary currently exists for the fixture profile catalog/editor stack that the Vixen wizard reaches through `FixtureSpecificationManager`, `FixturePropertyEditorViewModel`, `FunctionTypeViewModel`, and `CurveEditor`.

That means the likely implementation sequence should start with fixture contracts, a fixture draft, and fixture visual-input design rather than with changes to scanning or wizard composition.

The spike decision follows from that. A prototype would be most valuable if there were uncertainty about whether the current POC mechanics could host fixture setup at all. The inventories now show that they can: prop-specific pages are allowed, shared drafts are already the norm, preview coordinators already rebuild from draft snapshots, and discovery already auto-registers new pipelines. The unresolved work is architectural specification, not proof of capability. The best next move is therefore to write the final review to `Docs/Reviews/fixtures-gap-design-review-results.md` with concrete contract recommendations and an ordered implementation path while preserving `Docs/fixtures-gap-design-review.md` as the original prompt.

One clarification still needs to be published into the docs set: props are allowed to own and control their full wizard page flow. Reusable feature pages are available where they help, but they are not mandatory. That clarification matters for fixtures because it prevents the review from treating lack of feature-page reuse as an architectural defect.

The final review document now captures that clarification directly and uses it to narrow the real blockers. The completed result is a design review that points future implementation work toward fixture contracts, fixture draft ownership, profile/editor service boundaries, fixture preview input design, and group-creation semantics without asking contributors to revisit the already-settled discovery and wizard-composition questions.

The completed outcome should be a design review that either shows fixture support can fit the current architecture with bounded additions or proves that one or more core seams must be expanded first. The plan intentionally leaves room for a small prototype because the hardest risks are likely to be around preview state, runtime motion state, and profile-driven configuration rather than around assembly discovery.

## Context and Orientation

In this repository, a "prop" is the runtime object that owns configuration and eventually produces a visual model for preview and rendering. A "draft" is temporary wizard-owned state used during create and edit flows. A "visual input" is a small transfer record that contains only the data needed to build a visual model. A "feature page" is a reusable wizard page that participates in setup when a prop supports a corresponding feature interface.

The current POC pattern is documented in `Docs/poc-system-overview.md`, `Docs/core-design-goals.md`, `Docs/feature-wizards-requirements.md`, and `Docs/naming-conventions.md`. The important rule is that wizard pages should not edit props directly. Instead, setup wrappers create a draft, wizard pages edit draft-owned state, reusable feature pages initialize from `FeatureWizardContext`, prop draft mappers move data between prop and draft, and visual builders generate preview models from transfer objects.

The fixture reference implementation lives under the local `Vixen/` tree. The core runtime type is `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureProp.cs`. Its model lives in `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureModel.cs`. The current fixture wizard lives under `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/` and is composed from pages such as `SelectProfileWizardPage`, `EditProfileWizardPage`, `ColorSupportWizardPage`, `AutomationWizardPage`, `DimmingCurveWizardPage`, and `GroupingWizardPage`.

The review needs to answer at least five concrete questions.

First, what fixture state is truly prop-owned and persisted, and what state is only setup-time or editor-time helper state? Second, which parts of the current fixture wizard flow are reusable feature-style pages versus fixture-specific pages? Third, how much of the current fixture behavior depends on direct mutable sharing between pages or editor controls, which conflicts with the POC's draft-first design? Fourth, what should count as the fixture equivalent of the POC's visual-input and visual-model pipeline, especially for moving-head geometry and runtime pan/tilt style motion? Fifth, does the current POC need new feature interfaces or draft contracts to represent fixture capabilities without confusing setup-time baseline geometry with runtime rendered motion.

The key repository areas to compare are:

- `Props.Abstractions/`, which defines prop, feature, draft, setup, and visual-model contracts.
- `Props.Registry/`, which defines startup discovery, feature inference, and dependency-injection registration.
- `Props.Runtime/Tree/` and `Props.Runtime/PolyLine/`, which are the reference prop pipelines in the current POC.
- `Props.Runtime/Wizards/Features/`, which shows the current reusable feature-page patterns.
- `Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/`, which shows the current intelligent-fixture runtime state shape.
- `Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/`, which shows the current intelligent-fixture setup flow, view models, and page interactions.

## Plan of Work

Begin by inventorying the fixture reference implementation in the local `Vixen/` tree. Read `IntelligentFixtureProp`, `IntelligentFixtureModel`, and the fixture wizard page/view-model classes closely enough to document the state they own, how that state moves through the wizard, and where the existing implementation depends on services or UI controls outside the POC. The review must note whether each fixture concept is prop-owned, page-owned, or effectively shared global editor state.

Next, inventory the POC seams that fixture support would need to use. Use `Tree` and `PolyLine` as the baseline examples because together they show the current rules for discovery, setup wrappers, drafts, draft mappers, visual-input mappers, visual-model builders, shared preview sessions, `FeatureWizardContext`, and reusable draft-backed feature pages. The point here is not to restate the docs; it is to identify exactly where fixture support already has a home and where no home exists yet.

With both inventories in hand, write a comparison section that maps each major fixture concern onto the POC architecture. At minimum, compare fixture profile selection, profile editing, grouping and cloning, dimming curves, color support, automation options, patching- or channel-related metadata, preview requirements, and runtime motion concepts such as pan and tilt. For each concern, classify it as one of three categories: already supported by the current POC pattern, supported with modest adaptation, or blocked by a missing abstraction or design conflict.

Then resolve the hardest open questions. The most likely ones are whether fixture setup can be represented by a single shared draft, how fixture runtime motion should be kept separate from baseline prop-definition rotations, and whether the current POC visual-model contracts can represent fixture body geometry plus emitted beam or legend overlays without leaking runtime editor concerns into prop setup. If any of those cannot be answered confidently from source reading alone, add a narrow spike. A good spike would be one of the following: a draft-only `FixtureProfileDraft` experiment, a minimal fixture prop that exercises discovery and preview without full wizard parity, or a proof-of-concept visual-input shape that separates fixture definition from runtime motion state. Do not spend spike effort on alternate feature-page persistence paths unless the fixture analysis proves the current shared-draft model itself is insufficient.

After the comparison and any spike are complete, write the durable review deliverable to `Docs/Reviews/fixtures-gap-design-review-results.md`. That results document should summarize the current fixture flow, name the specific gaps, cite the repository files that prove those gaps, recommend the next implementation slices in order, and clearly state which existing POC rules must remain unchanged. Keep `Docs/fixtures-gap-design-review.md` as the original prompt/reference input.

Throughout the review, apply the repository's review standards instead of only feature-parity thinking. Use the `dotnet-design-pattern-review` lens when assessing whether fixture responsibilities are cleanly separated, use the `dotnet-best-practices` lens when judging whether proposed abstractions fit the current solution shape, and use the `catel-mvvm` lens when evaluating wizard/view-model coupling and whether the current fixture UI behavior can be translated into the POC's Catel-based draft-backed pattern.

## Milestones

### Milestone 1: Build The Two Architecture Inventories

At the end of this milestone, the review should have two concrete maps: one for how intelligent fixtures work today in `Vixen/`, and one for the exact POC seams available in `Props.*`. This milestone is complete when a new contributor can point to a fixture behavior and immediately find both its current implementation file and the likely POC destination for that responsibility.

Run the source-inspection commands listed below, then capture the results in working notes or directly in the review document. The acceptance signal is that the inventory covers runtime state, wizard pages, shared services, preview concerns, and output artifacts such as grouping or node-generation behavior.

### Milestone 2: Classify Gaps And Design Conflicts

At the end of this milestone, the review should explain not just that differences exist, but whether they are superficial or architectural. This is the milestone where the categories "already supported", "supported with adaptation", and "missing architecture" are assigned. The acceptance signal is that every major fixture concern has one category, a rationale, and at least one file reference from both the POC and the Vixen fixture implementation.

### Milestone 3: Prove Or Retire The Hardest Unknown

At the end of this milestone, the review should no longer contain hand-wavy risk language around the single hardest design question. If a spike is needed, implement only enough code to prove feasibility or expose the real blocker, then validate it with focused build and test commands. If a spike is not needed, the review must explicitly say why source analysis was sufficient.

### Milestone 4: Publish The Final Review And Follow-Up Plan

At the end of this milestone, `Docs/Reviews/fixtures-gap-design-review-results.md` should read like a completed design review. It should include the findings, recommendations, validation evidence, and a sequenced follow-up plan that could become one or more implementation ExecPlans. The acceptance signal is that another contributor can read the document and know what to build first, what to avoid, and what evidence supports those choices.

## Concrete Steps

Work from `C:\Dev\PropCentric`.

1. Re-read the fixture task prompt and POC architecture docs:

       Get-Content Docs/fixtures-gap-design-review.md
       Get-Content Docs/poc-system-overview.md
       Get-Content Docs/core-design-goals.md
       Get-Content Docs/feature-wizards-requirements.md
       Get-Content Docs/naming-conventions.md

2. Inventory the current fixture runtime types:

       Get-Content Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureProp.cs
       Get-Content Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureModel.cs
       Get-ChildItem Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture

3. Inventory the current fixture wizard shape:

       Get-Content Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/IntelligentFixtureWizard.cs
       Get-ChildItem Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models
       Get-ChildItem Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels
       Get-ChildItem Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/Views

4. Search for coupling points and state flow inside the fixture wizard:

       Get-ChildItem Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard -Recurse | Select-String -Pattern "Wizard.Pages.Single|Fixture = |FunctionDefinitions|SupportsFunction|ShowDialog|PropertyEditor"

5. Inventory the current POC extension seams:

       Get-Content Props.Registry/PropsServiceCollectionExtensions.cs
       Get-Content Props.Registry/PropFeatureInferrer.cs
       Get-Content Props.Runtime/Tree/TreeProp.cs
       Get-Content Props.Runtime/Tree/Setup/TreePropSetup.cs
       Get-Content Props.Runtime/Tree/Setup/TreePropDraft.cs
       Get-Content Props.Runtime/Tree/Visuals/TreeVisualModelBuilder.cs
       Get-Content Props.Runtime/PolyLine/PolyLineProp.cs
       Get-Content Props.Runtime/PolyLine/Setup/PolyLinePropSetup.cs
       Get-ChildItem Props.Runtime/Wizards/Features -Recurse

6. Write a comparison matrix directly into `Docs/Reviews/fixtures-gap-design-review-results.md` or in temporary working notes, covering:

       runtime prop state
       wizard state ownership
       profile editing
       grouping/cloning
       color and dimming configuration
       preview generation
       runtime motion versus setup-time baseline geometry
       external service dependencies
       discovery and registration implications

7. If one unresolved question remains high-risk, create a spike branch of work in the POC codebase and validate it with focused commands such as:

       dotnet build PropCentric.sln
       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "<focused test filter>"

8. Write `Docs/Reviews/fixtures-gap-design-review-results.md` as the final review document, then run:

       dotnet build PropCentric.sln
       dotnet test PropCentric.Tests/PropCentric.Tests.csproj

9. If the spike touched runtime or wizard code, manually exercise the harness:

       dotnet run --project PropCentric/PropCentric.csproj

## Validation and Acceptance

This task is complete when all of the following are true.

First, `Docs/Reviews/fixtures-gap-design-review-results.md` is the completed design review deliverable. It must describe the current fixture design, the relevant POC patterns, the precise gaps between them, and a recommended path forward. `Docs/fixtures-gap-design-review.md` remains the original prompt/reference input.

Second, every major finding in that review is backed by repository evidence. A reader should be able to trace each conclusion to at least one POC file and one Vixen fixture file.

Third, the review must explicitly answer the core architecture questions rather than leaving them as vague concerns. It must state whether fixture setup can use the draft/mapping pipeline, how fixture runtime motion differs from setup-time `AxisRotations`, whether existing draft-backed feature-page patterns cover any fixture pages, and what new contracts are required if they do not.

Fourth, if a spike was needed, the review must include its outcome and the exact commands that proved success or exposed the blocker. Those commands must run from `C:\Dev\PropCentric`, and the solution must still build and the tests must still pass after the spike or after the spike is removed.

Fifth, the final section of the review must include a sequenced follow-up implementation plan. That sequence should identify the minimum first slice, such as adding fixture-specific abstractions, introducing a fixture draft, or defining fixture visual-input contracts, before attempting full wizard parity.

## Idempotence and Recovery

Most of this task is analysis and documentation, so it is naturally repeatable. Re-reading the source and revising the review document is safe. If a spike is introduced, keep it narrow and additive so it can either be promoted into a future implementation plan or removed cleanly after the finding is recorded.

If the analysis starts to sprawl, recover by forcing every note back into one of the three comparison categories: already supported, supported with adaptation, or missing architecture. If a proposed finding cannot be tied to repository evidence, it is not ready for the final review.

If a spike causes the solution to fail, either complete the spike until the build and tests pass again or revert only the spike-specific changes before finishing the documentation. Do not leave speculative code behind without an explicit recommendation to keep it.

## Artifacts and Notes

The review should aim to produce an evidence trail shaped like this:

    Fixture concern: profile editing
    Current Vixen implementation:
        Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileWizardPageViewModel.cs
        Vixen/src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileFunctionsWizardPageViewModel.cs
    Current POC analogue:
        Props.Runtime/*/Setup/*PropDraft.cs
        Props.Runtime/*/Setup/*PropDraftMapper.cs
    Expected finding:
        Current fixture flow edits shared mutable specification objects across pages.
        POC expects a single draft plus explicit mapping.
        Gap is real and likely needs a dedicated fixture draft plus mapper layer.

    Fixture concern: runtime motion versus baseline geometry
    Current Vixen implementation:
        Vixen/src/Vixen.Modules/App/Props/Models/IntelligentFixture/IntelligentFixtureModel.cs
    Current POC analogue:
        Docs/core-design-goals.md
        Props.Abstractions/Features/ICanAxisRotate.cs
    Expected finding:
        Fixture pan and tilt should not be modeled as setup-time baseline axis rotations.
        A separate rendered-state concept or fixture-specific runtime contract is likely required.

    Fixture concern: reusable feature-page opportunities
    Current Vixen implementation:
        ColorSupportWizardPage
        DimmingCurveWizardPage
        GroupingWizardPage
    Current POC analogue:
        Props.Runtime/Wizards/Features/Color/
        Props.Runtime/Wizards/Features/Dimming/
    Expected finding:
        Some fixture pages may map onto existing reusable draft-backed feature pages, but profile-driven fixture editing will remain fixture-specific.

## Interfaces and Dependencies

This review is expected to inspect, and potentially recommend additions to, the following existing interfaces and contract families:

- `Props.Abstractions.Props.IProp` and `BaseProp<TModel>`, which define persisted prop ownership.
- `Props.Abstractions.Setup.IPropSetup`, `IPropDraft`, and `IPropDraftMapper<TDraft, TProp>`, which define the setup orchestration and draft mapping pattern.
- `Props.Abstractions.Visual.IVisualInputMapper<TSource, TVisualInput>`, `IPropVisualModelBuilder<TVisualInput, TVisualModel>`, and `IWizardPreviewCoordinator<TDraft>`, which define the current preview and visual-generation pipeline.
- `Props.Abstractions.Features.*`, especially `PropFeatureFlags`, `FeatureWizardContext`, existing feature interfaces, and any future fixture-specific capability contracts that may be needed.
- `Props.Runtime.Wizards.Features.*`, which provide the existing reusable draft-backed feature-page patterns for comparison.

If the review recommends new contracts, it must name them concretely and place them in the current naming system. Example recommendation shapes that may prove necessary are:

    IHasFixtureProfile
    IHasFixtureMotion
    FixturePropDraft
    FixtureVisualInput
    FixtureWizardPreviewCoordinator

Those names are placeholders at plan-creation time. The completed review must either confirm them, replace them with better names, or explain why no new contracts are required.

Revision note: updated on 2026-05-25 after the feature-wizard unification work removed mapper-backed feature pages from the supported POC architecture. The fixture-gap review should now evaluate fixture setup against the shared-draft plus `FeatureWizardContext` model only.
Revision note: updated on 2026-05-25 to record completion of Concrete Step 1 again after re-reading the current fixture review and core POC docs before resuming the inventory work.
Revision note: updated on 2026-05-25 to record completion of the fixture runtime-type inventory and capture the main persisted-state findings before moving on to wizard-flow analysis.
Revision note: updated on 2026-05-25 to record completion of the fixture wizard-shape inventory, including the explicit page order and the supporting model/view-model/view structure.
Revision note: updated on 2026-05-25 to record completion of the coupling and dependency inventory across the fixture wizard view models, including the main sibling-page and external-editor dependencies.
Revision note: updated on 2026-05-25 after user clarification that fixture setup may legitimately own multiple fixture-specific pages, so the review should not force reuse where the behavior is inherently fixture-only.
Revision note: updated on 2026-05-25 to record completion of the POC seam inventory, including discovery, setup-wrapper, mapper, visual-input, preview, and reusable feature-page reference points.
Revision note: updated on 2026-05-25 to record the first complete gap classification and the rule used to distinguish `already supported`, `supported with adaptation`, and `missing architecture`.
Revision note: updated on 2026-05-25 to record the decision that no spike is required because the remaining uncertainties are contract-design questions, not unproven POC mechanics.
Revision note: updated on 2026-05-25 to add a follow-up documentation step clarifying that props may own their full wizard flow and only use feature pages when those pages provide value.
Revision note: updated on 2026-05-25 after publishing the wizard-flow clarification into the core POC docs so the final review can reference repository guidance instead of only plan notes.
Revision note: updated on 2026-05-25 after moving the durable review deliverable to `Docs/Reviews/fixtures-gap-design-review-results.md` and restoring `Docs/fixtures-gap-design-review.md` as the original prompt file.
