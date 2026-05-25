# Consolidate Feature Wizards Around Shared Draft State

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This repository includes `.agents/PLANS.md`, and this document must be maintained in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

After this change, every feature wizard page will be backed by the shared wizard draft for the current prop setup flow, regardless of whether the page hosts the OpenGL viewer. A user will be able to move through Tree and PolyLine wizards knowing that every editable value in every page lives in one shared state object, so edits reopen correctly, preview-driving state updates consistently, and new feature pages follow one obvious pattern instead of choosing between legacy mapper-backed and newer draft-backed approaches.

The observable architectural result is that `DimmingFeatureWizardPage`, `ColorFeatureWizardPage`, `RotationFeatureWizardPage`, and `SegmentsFeatureWizardPage` all participate in the wizard through the same backing-state contract. Setup wrappers stop performing separate mapper population and apply loops, and the feature-page resolver becomes responsible only for resolving and initializing feature pages against the current wizard context.

## Progress

- [x] (2026-05-25 10:47-05:00) Reviewed the relevant docs and current feature-page implementations.
- [x] (2026-05-25 10:47-05:00) Confirmed that only dimming still uses the legacy mapper-backed pattern.
- [x] (2026-05-25 10:47-05:00) Chosen the target direction: consolidate all feature pages on shared-draft backing and retire feature data mappers.
- [x] (2026-05-25 10:47-05:00) Chosen the `FeatureWizardContext` object as the final per-page initialization contract for feature pages.
- [x] (2026-05-25 13:57-05:00) Implemented `FeatureWizardContext` in `Props.Abstractions`, updated resolver/setup initialization to use it, and migrated Color, Rotation, and Segments pages plus their tests.
- [x] (2026-05-25 14:18-05:00) Added `IHasDimmingSettingsDraft`, implemented it on Tree and PolyLine drafts, and updated both prop draft mappers plus mapping tests to round-trip brightness and gamma through shared drafts.
- [x] (2026-05-25 14:15-05:00) Migrated `DimmingFeatureWizardPage` to the shared-draft pattern, removed its mapper declaration, and added focused page, discovery, and setup round-trip tests.
- [x] (2026-05-25 14:19-05:00) Removed `IFeatureWizardDataMapper`, deleted the unused dimming mapper, simplified `FeatureWizardPageAttribute`, `FeatureWizardPageDescriptor`, `IFeatureWizardPageResolver`, `FeatureWizardPageResolver`, `TreePropSetup`, and `PolyLinePropSetup`, and updated tests to the draft-only path.
- [ ] Align repository docs with the now draft-only feature wizard pattern.
- [ ] Run focused and full test validation.

## Surprises & Discoveries

- Observation: the architecture docs already favor shared-draft state strongly; the dual pattern mainly survives in code for dimming and in resolver/setup abstractions.
  Evidence: `Docs/feature-wizards-requirements.md` calls `DimmingFeatureWizardPage` the legacy mapper-backed example while Color, Rotation, and Segments are the reference draft-backed pages.

- Observation: `TreePropSetup` and `PolyLinePropSetup` still contain duplicate orchestration loops solely because mapper-backed feature pages remain supported.
  Evidence: both setup wrappers call `InitializePages(...)`, then separately build mapper lists, populate mappers before showing the wizard, and apply mappers after acceptance.

- Observation: preview hosting is already orthogonal to backing state.
  Evidence: `SegmentsFeatureWizardPage` and `RotationFeatureWizardPage` host the shared preview path, while `ColorFeatureWizardPage` uses the same draft/preview initialization path without hosting the viewer.

- Observation: even among draft-backed pages, there are two implementation styles: direct wrappers over mutable draft child objects versus page-local projection synchronized back into the draft.
  Evidence: `SegmentsFeatureWizardPage` and `RotationFeatureWizardPage` wrap draft-owned items directly, while `ColorFeatureWizardPage` maintains helper state and writes snapshots back into `draft.ColorConfiguration`.

- Observation: introducing the context object was a low-risk first slice because the existing draft-backed pages and test doubles already behaved as if draft and preview session were one wizard-scoped unit.
  Evidence: after replacing the two-parameter initialization calls with `FeatureWizardContext`, the focused test set for Color, Rotation, Segments, Tree setup, and PolyLine setup passed unchanged in behavior.

- Observation: keeping `Brightness` as `double` on the draft preserved the prop-domain shape while still allowing the legacy dimming page UI to remain integer-based during the migration.
  Evidence: Tree and PolyLine draft-mapping tests now round-trip fractional brightness values through `TreePropDraftMapper` and `PolyLinePropDraftMapper` without loss before the page layer is touched.

- Observation: the dimming page could become draft-backed without any view-model redesign because its existing Catel `ViewModelToModel` bindings only depend on page properties, not on where those properties store canonical state.
  Evidence: `DimmingFeatureWizardPageViewModel` remained unchanged while new focused tests passed for `DimmingFeatureWizardPage`, `TreePropSetup`, `PolyLinePropSetup`, and discovery after the page switched from mapper-backed state to draft-backed state.

- Observation: once the last mapper-backed page was migrated, the resolver and setup cleanup was mechanically simple because no production code still depended on mapper metadata or mapper populate/apply loops.
  Evidence: deleting `IFeatureWizardDataMapper`, removing `GetMappersFor(...)`, and simplifying both prop setup wrappers required only direct removal of dead paths plus small test-double updates; the focused discovery and setup test slice still passed immediately after the cleanup.

## Decision Log

- Decision: converge on shared draft state as the only backing model for feature wizard pages.
  Rationale: this matches the architecture documents, removes duplicate setup orchestration, and gives every wizard page the same source-of-truth model.
  Date/Author: 2026-05-25 / Codex

- Decision: do not create separate patterns for viewer-hosting pages and non-viewer pages.
  Rationale: the current code already proves that preview participation is optional behavior layered on top of the same shared draft and preview-session initialization path.
  Date/Author: 2026-05-25 / Codex

- Decision: migrate dimming into the draft-backed model rather than preserving mapper support indefinitely.
  Rationale: dimming is the only remaining fully legacy feature page and is the cleanest migration target to complete the architectural transition.
  Date/Author: 2026-05-25 / Codex

- Decision: permit page-local helper models when UI complexity requires them, but require the shared draft to remain the only canonical state store.
  Rationale: `ColorFeatureWizardPage` shows that some editing experiences need local projection state, yet those helpers do not require a second architecture pattern as long as accepted edits flow immediately into the shared draft.
  Date/Author: 2026-05-25 / Codex

- Decision: use a `FeatureWizardContext` object rather than a multi-parameter feature-page initialization method.
  Rationale: the context object keeps wizard-scoped dependencies together, avoids future breaking changes when new wizard-scoped services are added, and makes the “one context per wizard instance” model explicit.
  Date/Author: 2026-05-25 / Codex

## Outcomes & Retrospective

The core migration is complete. The repository now has a stable `FeatureWizardContext` abstraction in `Props.Abstractions`, all current feature pages are draft-backed, dimming data plus dimming-page editing both flow through shared drafts for Tree and PolyLine, and the mapper-oriented abstractions plus setup orchestration have been removed from production code. The remaining work is documentation alignment and broader validation, not another architectural transition.

## Context and Orientation

A “draft” in this repository is the wizard-owned temporary state object used during prop create and edit flows. It is distinct from the committed prop. The prop remains the runtime source of truth after the wizard completes, but while the wizard is open the shared draft is the only state object that pages should treat as canonical.

A “feature wizard page” is a reusable Orc.Wizard page discovered by `FeatureWizardPageAttribute` and inserted into a prop setup flow when the prop type implements the corresponding feature interface. Examples live under `Props.Runtime/Wizards/Features`.

The current split is this:

- `Props.Runtime/Wizards/Features/Dimming/Pages/DimmingFeatureWizardPage.cs` is mapper-backed. Its state lives on the page itself. `Props.Runtime/Wizards/Features/Dimming/Mappers/DimmingFeatureWizardDataMapper.cs` copies data between the page and the prop.
- `Props.Runtime/Wizards/Features/Color/Pages/ColorFeatureWizardPage.cs`, `Props.Runtime/Wizards/Features/Rotation/Pages/RotationFeatureWizardPage.cs`, and `Props.Runtime/Wizards/Features/Segments/Pages/SegmentsFeatureWizardPage.cs` are draft-backed. They currently receive the shared `IPropDraft` and `IWizardPreviewSession` for the current wizard instance and should be migrated first to a `FeatureWizardContext`.
- `Props.Registry/FeatureWizardPageResolver.cs` and `Props.Abstractions/Features/IFeatureWizardPageResolver.cs` still support both patterns.
- `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` still orchestrate both patterns.

The target state is that feature pages use one common backing model across the repository:

- the shared prop draft holds all wizard-editable feature state
- feature pages are initialized once per wizard instance against that shared draft
- feature pages may use the shared preview session, but preview hosting is optional and does not change the backing-state pattern
- prop draft mappers, not feature data mappers, handle draft -> prop persistence at wizard acceptance time

## Plan of Work

First, implement the chosen feature-page initialization shape. Introduce a wizard-scoped context type, `FeatureWizardContext`, that contains `IPropDraft Draft` and `IWizardPreviewSession PreviewSession`. Update `IFeatureWizardDraftPage` so feature pages initialize from that single object. Then update `IFeatureWizardPageResolver` and `FeatureWizardPageResolver` so resolver initialization also uses the same context object. This establishes the long-term contract before dimming migration begins.

Second, add setup-only draft contracts for dimming under `Props.Abstractions/Setup/Drafts`. The repository already uses narrow setup-only interfaces such as `IHasColorSettingsDraft`, `IHasAxisRotationsDraft`, and `IHasSegmentsDraft`. Dimming should follow the same pattern. This repository now chooses `IHasDimmingSettingsDraft` with `double Brightness` and `double Gamma` so draft state preserves the prop-domain representation without lossy round-tripping.

Third, update the concrete drafts and draft mappers. `TreePropDraft` and `PolyLinePropDraft` should implement the new dimming draft contract because those props already support `IHasDimming`. Their prop draft mappers must populate dimming values from the prop into the draft before editing and apply draft values back into the prop after acceptance. This is the key step that moves dimming persistence into the main draft pipeline.

Fourth, refactor `DimmingFeatureWizardPage` to become draft-backed. Remove the mapper type from its `FeatureWizardPageAttribute`. Implement the shared feature-page initialization contract. Bind the page to the shared dimming draft state. If the UI remains simple, the page can proxy directly into the draft; if validation or formatting needs helper state, keep helpers but synchronize into the draft immediately so the draft remains canonical. The page should accept the shared preview session even if it does not host the OpenGL viewer.

Fifth, remove the legacy feature data mapper path from the abstractions and setup orchestration. Delete `IFeatureWizardDataMapper` once no feature pages use it. Remove `GetMappersFor(...)` from `IFeatureWizardPageResolver`. Remove mapper creation from `FeatureWizardPageResolver`. Remove `mapperType` usage from `FeatureWizardPageAttribute` and the scanner/descriptor path if that metadata is no longer needed. Then simplify `TreePropSetup` and `PolyLinePropSetup` so they resolve feature pages, initialize them against the shared draft context, show the wizard, and rely on the prop draft mapper for persistence.

Sixth, normalize tests around the unified model. Existing tests already cover draft-backed page initialization for Color, Rotation, and Segments. Add dimming coverage that proves:

- the page initializes from shared draft state
- edits update the shared draft immediately
- Tree and PolyLine setup/edit flows round-trip dimming values through the draft without any feature data mapper
- feature-page resolver and prop setup tests no longer depend on `GetMappersFor(...)`

Finally, remove dead code and refresh docs. Update `Docs/feature-wizards-requirements.md`, `Docs/poc-system-overview.md`, and any plan or review notes that still present mapper-backed pages as a supported long-term pattern. After the migration, those docs should describe mapper-backed pages as historical context only, or omit them entirely.

## Concrete Steps

All commands run from `C:\Dev\PropCentric`.

Inspect the current feature wizard and setup files before editing:

    Get-Content Props.Abstractions\Features\IFeatureWizardPageResolver.cs
    Get-Content Props.Abstractions\Features\IFeatureWizardDraftPage.cs
    Get-Content Props.Abstractions\Features\FeatureWizardContext.cs
    Get-Content Props.Abstractions\Features\IFeatureWizardDataMapper.cs
    Get-Content Props.Registry\FeatureWizardPageResolver.cs
    Get-Content Props.Runtime\Wizards\Features\Dimming\Pages\DimmingFeatureWizardPage.cs
    Get-Content Props.Runtime\Wizards\Features\Dimming\Mappers\DimmingFeatureWizardDataMapper.cs
    Get-Content Props.Runtime\Tree\Setup\TreePropDraft.cs
    Get-Content Props.Runtime\Tree\Setup\TreePropDraftMapper.cs
    Get-Content Props.Runtime\PolyLine\Setup\PolyLinePropDraft.cs
    Get-Content Props.Runtime\PolyLine\Setup\PolyLinePropDraftMapper.cs
    Get-Content Props.Runtime\Tree\TreePropSetup.cs
    Get-Content Props.Runtime\PolyLine\PolyLinePropSetup.cs

After migrating dimming and removing mapper support, confirm no feature mapper references remain in source files:

    Get-ChildItem -Recurse -Include *.cs | Select-String -Pattern 'IFeatureWizardDataMapper|GetMappersFor\(|mapperType: typeof\(|DimmingFeatureWizardDataMapper'

Run focused tests during the migration:

    dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~Dimming|FullyQualifiedName~TreePropSetupTests|FullyQualifiedName~PolyLinePropSetupTests|FullyQualifiedName~DraftBackedWizardPageTests"

Run the full test suite and solution build at the end:

    dotnet test PropCentric.Tests/PropCentric.Tests.csproj
    dotnet build PropCentric.sln

Expected high-level result:

- all feature wizard pages initialize from the shared draft
- no setup wrapper performs feature mapper population or apply loops
- no feature-page discovery metadata declares a mapper type
- dimming round-trips through the same draft pipeline used by color, rotation, and segments

## Validation and Acceptance

Acceptance is behavioral and architectural.

For code-level acceptance:

- `Props.Runtime/Wizards/Features/Dimming/Pages/DimmingFeatureWizardPage.cs` implements the shared draft-backed feature-page contract.
- `Props.Runtime/Wizards/Features/Dimming/Mappers/DimmingFeatureWizardDataMapper.cs` is deleted.
- `Props.Abstractions/Features/IFeatureWizardDataMapper.cs` is deleted.
- `Props.Abstractions/Features/IFeatureWizardPageResolver.cs` no longer exposes `GetMappersFor(...)`.
- `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` no longer create, populate, or apply feature data mappers.

For test acceptance:

- run `dotnet test PropCentric.Tests/PropCentric.Tests.csproj`
- expect the existing feature-page and setup tests to pass after being updated to the new contract
- add tests that would fail before the change:
  - dimming page initializes from a draft implementing the dimming setup contract
  - changing dimming values on the page updates the draft immediately
  - Tree and PolyLine edit flows persist dimming changes without feature mappers

For manual acceptance in the harness app:

- open a Tree prop in edit mode
- navigate to the Dimming page
- observe that existing brightness and gamma values are already loaded
- change the values, complete the wizard, reopen edit, and observe the same values are shown again
- if preview behavior is wired to dimming in the future, confirm the page already participates through the shared preview session without another architecture change

## Idempotence and Recovery

Perform the migration in additive steps. First add dimming draft contracts and draft-mapper support, then convert the dimming page to read from the draft, then remove feature data mapper support only after all tests pass with the new path. This keeps rollback simple because the old mapper path can remain temporarily until dimming is proven to round-trip correctly through drafts.

If the repository needs a shorter transition, it is acceptable to keep `IFeatureWizardDataMapper` temporarily while `DimmingFeatureWizardPage` is moved to the new pattern, but the end of the plan must delete the old contract and remove the duplicate setup orchestration. Do not stop at a halfway state where both patterns remain first-class.

## Artifacts and Notes

Important files likely to change during implementation:

- `Props.Abstractions/Features/IFeatureWizardDraftPage.cs`
- `Props.Abstractions/Features/IFeatureWizardPageResolver.cs`
- `Props.Abstractions/Features/FeatureWizardPageAttribute.cs`
- `Props.Abstractions/Setup/Drafts/IHasDimmingSettingsDraft.cs` (new)
- `Props.Registry/FeatureWizardPageResolver.cs`
- `Props.Registry/FeatureWizardPageScanner.cs`
- `Props.Registry/FeatureWizardPageDescriptor.cs`
- `Props.Runtime/Wizards/Features/Dimming/Pages/DimmingFeatureWizardPage.cs`
- `Props.Runtime/Tree/Setup/TreePropDraft.cs`
- `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs`
- `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs`
- `Props.Runtime/PolyLine/Setup/PolyLinePropDraftMapper.cs`
- `Props.Runtime/Tree/TreePropSetup.cs`
- `Props.Runtime/PolyLine/PolyLinePropSetup.cs`
- `PropCentric.Tests/*` focused on feature pages and prop setup wrappers

Target end-state flow in prose:

    setup wrapper creates or loads prop
      -> prop draft mapper populates shared draft from prop
      -> feature page resolver returns applicable pages
      -> resolver initializes each page with shared draft context
      -> wizard pages edit shared draft-backed state
      -> optional preview session rebuilds from the same draft
      -> user accepts wizard
      -> prop draft mapper applies draft to prop
      -> prop commits

There should be no separate feature mapper populate/apply phase in that flow.

## Interfaces and Dependencies

Use setup-only draft contracts to keep wizard editing concerns separate from runtime feature discovery. Follow the established pattern under `Props.Abstractions/Setup/Drafts`.

A likely contract is:

    public interface IHasDimmingSettingsDraft
    {
        int Brightness { get; set; }
        double Gamma { get; set; }
    }

If the implementation chooses a normalized double for brightness instead, keep the page API and summary formatting consistent and document the conversion rules in XML docs and tests.

The feature-page resolver should end in a simpler shape:

    public interface IFeatureWizardPageResolver
    {
        IReadOnlyList<IWizardPage> GetPagesFor(Type propType);
        void InitializePages(IReadOnlyList<IWizardPage> pages, FeatureWizardContext context);
    }

`DimmingFeatureWizardPage` should end in one of these shapes:

    public sealed class DimmingFeatureWizardPage : WizardPageBase, IFeatureWizardDraftPage

where the page stores the typed dimming draft reference obtained during initialization and treats it as the canonical backing state.

Revision note: Updated this ExecPlan after removing the mapper-oriented feature wizard infrastructure from production code and validating the simplified resolver/setup path with focused tests. The plan now records documentation alignment and broader validation as the remaining work.
