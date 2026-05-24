# Unify Wizard Page State Around Draft-Backed Base Properties

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This repository includes `.agents/PLANS.md`, and this document must be maintained in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

After this change, prop wizard pages will use the draft as the single source of truth for all wizard-editable prop data, including shared base fields such as `Name`, `LightSize`, and `StringType`. The user-visible effect is that edit/create flows load existing values without special seeding code, field changes immediately affect preview input through the draft, and new shared fields no longer require brittle synchronization hooks.

A user should be able to open the Tree and PolyLine wizards, see existing values already selected, change shared fields like light size and string type, and have those changes persist and appear correctly in summaries and preview generation without any page-local sync helpers.

## Progress

- [x] (2025-02-14 00:00Z) Define shared draft capability interfaces for common wizard fields.
- [x] (2025-02-14 00:00Z) Refactor base wizard page classes to delegate shared properties to the draft instead of Catel-owned duplicate state.
- [x] (2025-02-14 00:00Z) Update `TreePropWizardPage` and `PolyLinePropWizardPage` to inherit from the new draft-backed base types.
- [x] (2025-02-14 00:00Z) Remove setup-time wizard seeding for shared fields from `TreePropSetup` and `PolyLinePropSetup`.
- [x] (2025-02-14 00:00Z) Add or update tests proving edit-flow loading and draft mutation for shared fields.
- [x] (2025-02-14 00:00Z) Run targeted tests for Tree and PolyLine wizard/draft behavior.

## Surprises & Discoveries

- Observation: `TreePropWizardPage` currently mixes draft-backed properties with base-class Catel property storage, which forces both constructor-time sync and `PropertyChanged` back-sync.
  Evidence: `Props.Runtime/Tree/Wizard/Pages/TreePropWizardPage.cs` contains both `SyncDraftToParent()` and `OnParentPropertyChanged(...)`.

- Observation: `TreePropSetup` and `PolyLinePropSetup` both seed page state separately from draft creation, so initialization responsibility is split.
  Evidence: `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` each call `PopulateWizardFromDraft(...)` to copy `Name` and `LightSize` into the page.

- Observation: `LightPropWizardPage` owns `StringType`, but `ILightPropWizardPage` does not expose it, so the abstraction is already inconsistent.
  Evidence: `Props.Runtime/Wizards/Core/Pages/LightPropWizardPage.cs` defines `StringType`; `Props.Runtime/Wizards/Core/Pages/ILightPropWizardPage.cs` does not.

- Observation: `PolyLinePropDraft` already carried `Name` and `LightSize`, but not `StringType`, even though `PolyLineProp` inherits `StringType` from `BaseLightProp<TModel>`.
  Evidence: `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs` lacked `StringType`, while `Props.Runtime/PolyLine/PolyLineProp.cs` derives from `BaseLightProp<PolyLinePropVisualModel>`.

- Observation: Converting the base pages to generic draft-backed types requires immediate constructor/base-type updates in derived pages even before their duplicate sync logic is removed.
  Evidence: `TreePropWizardPage` and `PolyLinePropWizardPage` must now call `base(draft)` because `PropWizardPageBase<TDraft>` and `LightPropWizardPage<TDraft>` require a draft instance in the constructor.

- Observation: Once shared properties are draft-backed at the base-page layer, the old page-level `PropertyChanged` mirrors become dead code rather than safety nets.
  Evidence: `TreePropWizardPage` and `PolyLinePropWizardPage` no longer need constructor sync or `OnParentPropertyChanged(...)` to keep `Name`, `LightSize`, or `StringType` current.

- Observation: After the page refactor, the setup helpers no longer needed access to wizard page instances at all; their remaining job was only feature-mapper initialization.
  Evidence: `PopulateWizardFromDraft(...)` in both setup classes reduced to `mapper.PopulateFrom(prop)` calls and was removed.

- Observation: `StringType` remains a protected base-page property, so direct regression coverage for it must either go through the view model or use reflection in tests.
  Evidence: This was true during the initial test pass design, but the property was later made public on `ILightPropWizardPage` and `LightPropWizardPage<TDraft>`, so the final regression tests use direct property access.

- Observation: The targeted test suite passed after the refactor, but the build reports pre-existing nullable warnings in `Props.OpenGlCommon` and member-hiding warnings for `Draft` on the concrete wizard pages.
  Evidence: `dotnet test PropCentric.Tests\PropCentric.Tests.csproj --filter "Tree|PolyLine|DraftBackedWizardPageTests"` passed with 36 tests; warnings included `CS0108` for `TreePropWizardPage.Draft` and `PolyLinePropWizardPage.Draft`.

## Decision Log

- Decision: Use the draft as the single source of truth for wizard-editable prop state instead of keeping duplicated state in page base classes.
  Rationale: This removes brittle synchronization paths and aligns with the architecture docs stating wizard pages should work against draft state rather than prop state.
  Date/Author: 2025-02-14 / Codex

- Decision: Introduce small draft capability interfaces for shared wizard fields instead of coupling base pages directly to concrete drafts.
  Rationale: This preserves reuse across prop types and keeps the base page abstractions explicit and testable.
  Date/Author: 2025-02-14 / Codex

- Decision: Refactor incrementally by keeping the current view model structure and changing page/property ownership first.
  Rationale: The main defect is state duplication in wizard pages and setup flow. Preserving the view model layer reduces migration risk.
  Date/Author: 2025-02-14 / Codex

- Decision: Add `StringType` to `PolyLinePropDraft` and its draft mapper during the interface-introduction step.
  Rationale: `IHasLightSettingsDraft` must mean the same thing across props, and the draft must be able to round-trip all shared light settings before the base pages can rely on it.
  Date/Author: 2025-02-14 / Codex

- Decision: Keep `LightSizeMinimum` and `LightSizeMaximum` as Catel-owned page state while moving `Name`, `LightSize`, and `StringType` to draft-backed storage.
  Rationale: The min/max values are page behavior constraints rather than persisted prop setup data, so they do not belong on the draft.
  Date/Author: 2025-02-14 / Codex

- Decision: Remove the old page-level sync hooks entirely instead of leaving them in place as redundant observers.
  Rationale: Keeping them would obscure the single-source-of-truth design and make future maintenance harder by implying two active synchronization paths.
  Date/Author: 2025-02-14 / Codex

- Decision: Inline feature-mapper initialization in setup flows instead of keeping a renamed helper method.
  Rationale: Once page seeding was removed, the helper no longer clarified intent; the direct loop is shorter and makes the remaining responsibility obvious.
  Date/Author: 2025-02-14 / Codex

- Decision: Add focused wizard-page tests that verify shared fields read from and write to the draft directly, using reflection to cover `StringType`.
  Rationale: This keeps the regression surface tight around the new single-source-of-truth behavior without broadening the test scope to unrelated WPF view concerns. After `StringType` became public, the tests were simplified to use direct access.
  Date/Author: 2025-02-14 / Codex

Revision note: Added draft capability interfaces, updated Tree and PolyLine drafts to implement them, extended `PolyLinePropDraftMapper` to round-trip `StringType`, converted the core wizard page base classes to generic draft-backed forms, removed redundant page-level sync hooks from Tree and PolyLine wizard pages, deleted setup-time shared-field seeding from both prop setup flows, added wizard-page regression tests for shared draft-backed fields, made `StringType` public on the light wizard page abstraction, and ran targeted tests successfully.

## Outcomes & Retrospective

This section will be updated after implementation milestones complete. Success means there is no remaining need for constructor sync helpers, `OnParentPropertyChanged` mirrors, or setup-time seeding of shared wizard fields.

## Context and Orientation

A “draft” in this repository is a wizard-owned temporary data object used during create/edit flows. The prop is the runtime/domain object, while the draft is the editable setup state. A wizard page should edit draft state, and the preview coordinator should read that same draft state to build visuals.

Relevant files today:

- `Props.Runtime/Tree/Setup/TreePropDraft.cs`
  Holds wizard-owned editable state for `TreeProp`, including `Name`, `LightSize`, and `StringType`.

- `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs`
  Holds wizard-owned editable state for `PolyLineProp`. This file must be checked to confirm whether it already exposes `Name`, `LightSize`, and any other shared fields.

- `Props.Runtime/Wizards/Core/Pages/PropWizardPageBase.cs`
  Current base wizard page with Catel-owned `Name`.

- `Props.Runtime/Wizards/Core/Pages/LightPropWizardPage.cs`
  Current light-specific base wizard page with Catel-owned `LightSize` and `StringType`.

- `Props.Runtime/Tree/Wizard/Pages/TreePropWizardPage.cs`
  Currently mixes draft-backed tree-specific properties with duplicated base-property sync logic.

- `Props.Runtime/PolyLine/Wizard/Pages/PolyLinePropWizardPage.cs`
  Uses the same mirrored-state pattern for `Name` and `LightSize`.

- `Props.Runtime/Wizards/Core/ViewModels/PropBaseWizardPageViewModel.cs`
- `Props.Runtime/Wizards/Core/ViewModels/LightWizardPageViewModel.cs`
  These view models use Catel `ViewModelToModel` bindings against wizard page properties. The refactor should preserve property names and notifications so these view models continue to work.

- `Props.Runtime/Tree/TreePropSetup.cs`
- `Props.Runtime/PolyLine/PolyLinePropSetup.cs`
  These setup wrappers currently populate drafts, initialize feature pages, build wizards, and then explicitly seed page-level shared fields.

The architecture intent in `Docs/poc-system-overview.md` and `Docs/core-design-goals.md` is that wizard pages work against draft state and do not need separate synchronization against the prop.

## Plan of Work

First, introduce shared draft capability interfaces in a stable abstractions location. Create an interface for prop name and an interface for light-related setup fields. The intent is that any draft used by a generic base wizard page can guarantee those members exist without knowing the concrete prop type.

A likely shape is:

    IHasNameDraft
    IHasLightSettingsDraft : IHasNameDraft

`IHasNameDraft` should expose `string Name { get; set; }`. `IHasLightSettingsDraft` should expose `int LightSize { get; set; }` and `StringTypes StringType { get; set; }`. Place them with setup abstractions so they are clearly draft-only contracts rather than runtime prop feature interfaces.

Next, refactor the base wizard page classes to be generic over the draft type and to delegate shared properties directly to the draft. `PropWizardPageBase` should become something like `PropWizardPageBase<TDraft>` and store a `Draft` reference constrained to `IHasNameDraft`. `LightPropWizardPage<TDraft>` should inherit from that and constrain `TDraft` to `IHasLightSettingsDraft`.

These base page properties should no longer use Catel `RegisterProperty` for `Name`, `LightSize`, or `StringType`. Instead, they should proxy directly into the draft and call `RaisePropertyChanged(...)` in setters. Keep property names unchanged so existing XAML and view model bindings continue to work.

Then update `TreePropDraft` and `PolyLinePropDraft` to implement the new interfaces. Confirm `PolyLinePropDraft` has the required members; if any are missing, add them in a way consistent with the current prop/draft mapper pattern.

After that, update `TreePropWizardPage` and `PolyLinePropWizardPage` to inherit from the generic draft-backed base pages. Their constructors should pass the draft to the base class. Remove `SyncDraftToParent()`, `OnParentPropertyChanged(...)`, and any comments describing mirrored state. Tree-specific and polyline-specific properties should continue to read/write the draft directly, matching the new base-class pattern.

Next, simplify setup flow. In `TreePropSetup` and `PolyLinePropSetup`, remove explicit page seeding for `Name`, `LightSize`, and any other shared fields now owned entirely by the draft-backed base page. If `PopulateWizardFromDraft(...)` becomes only a feature-mapper initialization step, either shrink it to just that responsibility or inline it if clarity improves.

Then validate the view model layer. Because `PropBaseWizardPageViewModel` and `LightWizardPageViewModel` rely on `ViewModelToModel`, the page properties must still raise change notifications correctly. If any property stops updating through the current view model mapping, adjust the page property setters or binding mode before changing the view model design. Do not broaden the refactor to replace the view model layer unless testing proves it is necessary.

Finally, add regression tests. Cover at least these cases: a draft-loaded Tree wizard exposes existing `Name`, `LightSize`, and `StringType` without explicit seeding; changing those properties on the page updates the draft immediately; the same behavior holds for PolyLine; and no setup wrapper test relies on page seeding anymore.

## Concrete Steps

All commands run from `C:\Dev\PropCentric`.

Inspect the current draft and setup files before editing:

    Get-Content Props.Runtime\PolyLine\Setup\PolyLinePropDraft.cs
    Get-Content Props.Runtime\Tree\Setup\TreePropDraft.cs
    Get-Content Props.Runtime\Wizards\Core\Pages\PropWizardPageBase.cs
    Get-Content Props.Runtime\Wizards\Core\Pages\LightPropWizardPage.cs
    Get-Content Props.Runtime\Tree\Wizard\Pages\TreePropWizardPage.cs
    Get-Content Props.Runtime\PolyLine\Wizard\Pages\PolyLinePropWizardPage.cs
    Get-Content Props.Runtime\Tree\TreePropSetup.cs
    Get-Content Props.Runtime\PolyLine\PolyLinePropSetup.cs

Implement the abstractions and page refactor, then run targeted searches to confirm sync helpers are removed:

    Get-ChildItem -Recurse -Include *.cs | Select-String -Pattern 'SyncDraftToParent|OnParentPropertyChanged|PopulateWizardFromDraft\('

Run the relevant tests:

    dotnet test PropCentric.Tests/PropCentric.Tests.csproj

If needed, run targeted tests once the exact test names are known:

    dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter Tree
    dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter PolyLine

Expected high-level result:
- tests pass
- no page-local sync helper remains for shared base fields
- setup wrappers no longer seed `Name` / `LightSize` into wizard pages

## Validation and Acceptance

Acceptance is behavioral, not just structural.

For code-level acceptance:
- `TreePropWizardPage` no longer contains `SyncDraftToParent()` or `OnParentPropertyChanged(...)`.
- `PolyLinePropWizardPage` no longer contains `OnParentPropertyChanged(...)`.
- `TreePropSetup` and `PolyLinePropSetup` no longer copy shared fields from draft into page instances before showing the wizard.

For test acceptance:
- run `dotnet test PropCentric.Tests/PropCentric.Tests.csproj`
- expect all existing tests to pass
- add new tests that would fail before this refactor:
  - a page created with a pre-populated draft exposes the same `Name`, `LightSize`, and `StringType`
  - changing those page properties mutates the draft immediately
  - Tree and PolyLine setups no longer require separate page seeding for those values

For manual acceptance in the harness app:
- open a Tree prop in edit mode with existing values
- observe that `Light Type`, `Name`, and `Light Size` already display the stored values
- change them and proceed to summary
- observe that the summary reflects the new values and they persist when the wizard completes

## Idempotence and Recovery

This refactor should be done in small, safe steps. Introduce interfaces first, then migrate one base page layer, then derived pages, then remove old sync code. If a step breaks binding behavior, temporarily keep the old setup seeding until the draft-backed property notifications are corrected; do not keep both systems permanently once tests pass.

Avoid changing unrelated wizard feature pages during this work. If a feature page depends on the old mirrored pattern, document that in `Surprises & Discoveries` and decide whether to adapt it or isolate it.

## Artifacts and Notes

Important code artifacts to preserve in the final diff:

- new draft capability interfaces in the setup abstractions area
- generic draft-backed versions of:
  - `Props.Runtime/Wizards/Core/Pages/PropWizardPageBase.cs`
  - `Props.Runtime/Wizards/Core/Pages/LightPropWizardPage.cs`
- simplified page classes:
  - `Props.Runtime/Tree/Wizard/Pages/TreePropWizardPage.cs`
  - `Props.Runtime/PolyLine/Wizard/Pages/PolyLinePropWizardPage.cs`
- simplified setup wrappers:
  - `Props.Runtime/Tree/TreePropSetup.cs`
  - `Props.Runtime/PolyLine/PolyLinePropSetup.cs`
- regression tests for draft-backed shared fields

Expected post-refactor pattern in prose:

    Wizard page property setter
      -> writes directly to Draft
      -> raises PropertyChanged
      -> existing view model binding continues to update UI
      -> preview coordinator reads the same Draft instance

## Interfaces and Dependencies

Define draft-only shared contracts in the setup abstractions layer. Use stable names and keep them narrowly scoped:

    public interface IHasNameDraft
    {
        string Name { get; set; }
    }

    public interface IHasLightSettingsDraft : IHasNameDraft
    {
        int LightSize { get; set; }
        StringTypes StringType { get; set; }
    }

Refactor base page signatures so the type system guarantees required draft members exist:

    public abstract class PropWizardPageBase<TDraft> : WizardPageBase, IPropWizardPageBase
        where TDraft : class, IHasNameDraft

    public abstract class LightPropWizardPage<TDraft> : PropWizardPageBase<TDraft>, ILightPropWizardPage
        where TDraft : class, IHasLightSettingsDraft

Update concrete pages accordingly:

    public sealed class TreePropWizardPage : LightPropWizardPage<TreePropDraft>
    public sealed class PolyLinePropWizardPage : LightPropWizardPage<PolyLinePropDraft>

Preserve existing property names (`Name`, `LightSize`, `StringType`) to avoid unnecessary XAML and view model churn.
