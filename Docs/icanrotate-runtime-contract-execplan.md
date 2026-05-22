# Make `ICanRotate` The Runtime Rotation Contract

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

After this change, a prop that supports rotations will declare and own its rotation state through `ICanRotate` itself. The rotation feature page, the draft mappers, and the visual-input mappers will all work against that interface instead of relying on `BaseProp<TModel>` to provide universal rotation storage. A user should still see the same Tree wizard behavior as today, but the code path will be cleaner: Tree supports rotations because it implements `ICanRotate`, and PolyLine does not because it does not implement `ICanRotate`.

The user-visible proof is straightforward. Open the Tree wizard and confirm the Rotation page still appears and persists values. Open the PolyLine wizard and confirm there is still no Rotation page. Then run the full solution build and tests and confirm that Tree rotation behavior still works while PolyLine no longer carries rotation state through the runtime model.

## Progress

- [x] (2026-05-21 17:54 -05:00) Reviewed the current post-migration state of `ICanRotate`, `BaseProp<TModel>`, Tree rotation mapping, and PolyLine test data to identify the remaining design mismatch.
- [x] (2026-05-21 17:54 -05:00) Chosen the implementation direction: `ICanRotate` will become the runtime rotation contract and a static helper/factory will provide default and clone behavior so rotation-capable props do not duplicate setup code.
- [x] (2026-05-21 18:03 -05:00) Updated `ICanRotate` to expose `AxisRotations`, added `Props.Abstractions/PropVisualModels/AxisRotationCollectionFactory.cs`, and added focused tests covering default and clone behavior.
- [x] (2026-05-21 18:14 -05:00) Removed `AxisRotations` from `Props.Abstractions/Props/BaseProp.cs`, added explicit rotation ownership to `Props.Runtime/Tree/TreeProp.cs`, made Tree summary generation enumerate rotations safely, and removed the stale PolyLine test assignment that depended on inherited rotation state.
- [x] (2026-05-21 18:21 -05:00) Replaced the remaining local Tree rotation clone logic in `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs` with `AxisRotationCollectionFactory.Clone(...)` and reran focused Tree rotation mapping and preview tests.
- [x] (2026-05-21 18:31 -05:00) Moved `IHasRotationsDraft` and `IHasSegmentsDraft` into `Props.Abstractions/Setup/Drafts`, updated runtime/test namespace references, and reran focused draft-backed wizard and preview tests.
- [x] (2026-05-21 18:42 -05:00) Normalized the remaining rotation contract names to `ICanAxisRotate` and `IHasAxisRotationsDraft`, renamed the corresponding files, updated discovery/runtime/wizard/test references, and reran focused validation.
- [x] (2026-05-21 18:54 -05:00) Updated the regular repository docs (`Docs/poc-system-overview.md`, `Docs/core-design-goals.md`, `Docs/feature-wizards-requirements.md`, and `Docs/naming-conventions.md`) to use the normalized `AxisRotation` names and to separate setup-time baseline axis rotations from runtime rendered motion/state.
- [x] (2026-05-21 19:02 -05:00) Ran `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` and `dotnet build PropCentric.sln` successfully. Parallel build/test initially hit a transient output-file lock in `Vixen.Shim`; rerunning the build sequentially succeeded.

## Surprises & Discoveries

- Observation: the rotation feature-page migration is complete, but the runtime model still treats rotations as universal prop state.
  Evidence: `Props.Abstractions/Props/BaseProp.cs` still initializes and exposes `AxisRotations` for every prop instance.

- Observation: the current `ICanRotate` interface only acts as a feature marker, so the runtime layer still depends on concrete prop types or base-class inheritance for actual rotation state.
  Evidence: `Props.Abstractions/Features/ICanRotate.cs` currently has no members, while `TreePropDraftMapper` and `TreePropToVisualInputMapper` read `AxisRotations` from `TreeProp`.

- Observation: test data still proves the leak from `BaseProp<TModel>` into non-rotating props.
  Evidence: `PropCentric.Tests/PolyLine/PolyLineTestData.cs` still assigns `AxisRotations` to `PolyLineProp`, which should not be possible once rotation storage is feature-scoped.

- Observation: Tree summary generation still assumes exactly three rotations and indexes them directly.
  Evidence: `Props.Runtime/Tree/TreeProp.cs` uses `AxisRotations[0]`, `AxisRotations[1]`, and `AxisRotations[2]` in `GetSummary()`.

- Observation: making `ICanRotate` expose `AxisRotations` did not break the current runtime because `TreeProp` still satisfies the contract through the inherited base-class property for now.
  Evidence: focused validation with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~PropDiscoveryTests|FullyQualifiedName~AxisRotationCollectionFactoryTests"` passed before any runtime ownership changes landed.

- Observation: removing `AxisRotations` from `BaseProp<TModel>` immediately exposed stale non-rotating test code, which is exactly the failure mode this refactor is supposed to eliminate.
  Evidence: `PropCentric.Tests/PolyLine/PolyLineTestData.cs` no longer compiled until the inherited rotation assignment was removed.

- Observation: after the ownership move, the only remaining handwritten rotation clone path was in `TreePropDraftMapper`.
  Evidence: searching for `CloneRotations(` and `new ObservableCollection<AxisRotationModel>` in runtime code narrowed the remaining custom logic to `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs`.

- Observation: draft capability interfaces such as `IHasRotationsDraft` and `IHasSegmentsDraft` are currently stored under `Props.Abstractions/Features`, even though they are only used by setup drafts and wizard preview flows.
  Evidence: `IHasRotationsDraft` is consumed by `RotationFeatureWizardPage.Initialize(...)` and draft mappers rather than by runtime prop discovery, so the current location mixes prop feature contracts with setup-only draft contracts.

- Observation: the namespace relocation was broader than the two interface declarations because `SegmentDraftState` lives alongside `IHasSegmentsDraft` and is used across PolyLine setup, preview, and tests.
  Evidence: moving `IHasSegmentsDraft` required coordinated `using Props.Abstractions.Setup.Drafts;` updates in `Props.Runtime/PolyLine/*`, `Props.Runtime/Wizards/Features/Segments/*`, and multiple test files that construct `SegmentDraftState`.

- Observation: the current rotation naming is inconsistent because the model and property names are `AxisRotation`-based while the capability names still use the generic `Rotate` and `Rotations` terms.
  Evidence: the code now mixes `AxisRotationModel`, `AxisRotationCollectionFactory`, and `AxisRotations` with `ICanRotate` and `IHasRotationsDraft`, which makes the abstraction naming less precise than the underlying state it represents.

- Observation: the naming cleanup needed file renames as well as symbol renames because the repository convention expects contract filenames to match their public type names.
  Evidence: `Props.Abstractions/Features/ICanRotate.cs` and `Props.Abstractions/Setup/Drafts/IHasRotationsDraft.cs` were renamed to `ICanAxisRotate.cs` and `IHasAxisRotationsDraft.cs` so future contributors can locate the contracts predictably.

- Observation: full-solution validation is stable when build and test are run sequentially, but parallel execution can transiently lock intermediate outputs in this workspace.
  Evidence: a parallel run hit `CS2012` on `Vixen.Shim.dll`; an immediate sequential rerun of `dotnet build PropCentric.sln` succeeded with 0 warnings and 0 errors.

## Decision Log

- Decision: `ICanRotate` will stop being a marker-only interface and will become the runtime contract for prop rotation state.
  Rationale: the user explicitly wants rotation-capable props to expose rotation state through the interface rather than through a concrete prop type or a universal base-class property.
  Date/Author: 2026-05-21 / Codex

- Decision: use a static helper/factory to provide the standard rotation implementation details.
  Rationale: props already inherit from `BaseProp<TModel>` or `BaseLightProp<TModel>`, so an additional base class would create inheritance pressure. A static helper centralizes defaults and cloning without forcing an inheritance hierarchy.
  Date/Author: 2026-05-21 / Codex

- Decision: keep the wizard draft contract separate from the prop runtime contract.
  Rationale: `IHasRotationsDraft` models wizard-owned temporary state, while `ICanRotate` models persisted prop-owned runtime state. Keeping them separate preserves the existing setup-flow architecture.
  Date/Author: 2026-05-21 / Codex

- Decision: move setup-only draft capability interfaces into a dedicated `Props.Abstractions/Setup/Drafts` area rather than leaving them under `Props.Abstractions/Features`.
  Rationale: these interfaces are not discovered runtime prop features. They describe draft shapes used only during setup and wizard preview flows, and more of them are expected, so a dedicated draft-specific namespace and folder will scale better and keep feature contracts segregated from setup contracts.
  Date/Author: 2026-05-21 / Codex

- Decision: normalize the rotation capability and draft contract names around `AxisRotation`.
  Rationale: the underlying state, factory, and property names are already `AxisRotation`-based, so the contracts should use the same term to make the abstractions self-describing and internally consistent. The intended targets are `ICanAxisRotate` and `IHasAxisRotationsDraft`.
  Date/Author: 2026-05-21 / Codex

- Decision: Tree remains the reference implementation for a rotating prop and PolyLine remains the reference implementation for a non-rotating prop.
  Rationale: this proves both supported and unsupported paths through discovery, wizard resolution, runtime mapping, and tests.
  Date/Author: 2026-05-21 / Codex

## Outcomes & Retrospective

This plan records the next refactor after the rotation feature-page migration. The current system already has the right wizard behavior, but the runtime ownership boundary is still wrong because `BaseProp<TModel>` provides rotation state to every prop. The goal of this plan is to finish the architecture shift so the feature boundary, runtime state boundary, and wizard resolution boundary all align around `ICanRotate`.

At plan creation time, no runtime ownership changes have landed yet. Tree still works, PolyLine still lacks a Rotation page, and the code still needs one more slice to eliminate universal runtime rotation storage.

The runtime refactor is now complete in code and in the regular repository docs. `AxisRotationCollectionFactory`, `AxisRotationModel`, `AxisRotations`, `ICanAxisRotate`, and `IHasAxisRotationsDraft` are aligned around the same term. The distinction between setup-time baseline axis rotations and runtime rendered motion/state is now documented. Full solution build and test validation also pass.

## Context and Orientation

In this repository, a prop is the persisted runtime object that eventually renders geometry and can be committed back into the Vixen model. A draft is a temporary wizard-owned object used during create and edit flows. A feature page is a reusable wizard page that is discovered from a feature interface such as `ICanRotate`.

The current rotation flow is split correctly in the wizard layer but not in the runtime layer. The wizard side now uses `RotationFeatureWizardPage` under `Props.Runtime/Wizards/Features/Rotation`, and the page edits `IHasRotationsDraft.AxisRotations`. That part is already working. The remaining mismatch is that `Props.Abstractions/Props/BaseProp.cs` still creates and stores `AxisRotations` for every prop instance even though only some props should support rotations.

The key files for this work are:

- `Props.Abstractions/Features/ICanAxisRotate.cs`, which identifies axis-rotation support for feature discovery and runtime prop state.
- `Props.Abstractions/Setup/Drafts/IHasAxisRotationsDraft.cs`, which exposes shared draft state for the rotation feature page.
- `Props.Abstractions/Props/BaseProp.cs`, which currently initializes `AxisRotations` for every prop.
- `Props.Runtime/Tree/TreeProp.cs`, which is the first rotation-capable prop and should explicitly own rotation state after this refactor.
- `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs`, `Props.Runtime/Tree/Visuals/TreePropToVisualInputMapper.cs`, and `Props.Runtime/Tree/Visuals/TreeDraftToVisualInputMapper.cs`, which must move to interface-based rotation access.
- `PropCentric.Tests/PolyLine/PolyLineTestData.cs`, which currently still sets rotations on a non-rotating prop and therefore demonstrates the remaining leak.

In this plan, a "static helper/factory" means a small static class that creates standard rotation collections and clones existing rotation collections. It is not a service registered with dependency injection. It is simply shared code that avoids rewriting the same three default rotations and clone logic in every rotating prop or mapper.

## Plan of Work

Start by changing `Props.Abstractions/Features/ICanRotate.cs` from a marker interface into the actual runtime contract. The interface should keep its `[PropFeature(PropFeatureFlags.Rotation)]` attribute so feature discovery continues to work, but it must also expose `ObservableCollection<AxisRotationModel> AxisRotations { get; set; }`. This makes the feature interface the same contract that runtime code uses when it needs persisted rotations.

Next add a small static helper/factory in `Props.Abstractions`. Place it in a location that keeps it close to the rotation model, such as `Props.Abstractions/PropVisualModels/AxisRotationCollectionFactory.cs`. The helper must provide two things: a method that returns the standard default collection of X, Y, and Z rotations at zero degrees, and a method that clones an existing rotation sequence into a new `ObservableCollection<AxisRotationModel>`. The clone method should preserve axis and angle values but create new model instances so drafts, props, and visual inputs do not share mutable collection items by reference.

Then remove `AxisRotations` from `Props.Abstractions/Props/BaseProp.cs`. Delete the default initialization in the constructor and remove the property from the base type. Do not add it to `IProp`; the user explicitly wants rotation exposure to come from `ICanRotate`, not from the universal prop interface.

After that, update `Props.Runtime/Tree/TreeProp.cs` so Tree explicitly implements `ICanRotate` and owns the `AxisRotations` property itself. Initialize it with the new static factory’s default method. Also change `GetSummary()` so it enumerates the collection instead of indexing `AxisRotations[0]`, `AxisRotations[1]`, and `AxisRotations[2]`. The summary should still show the current axis and angle values, but it must tolerate any valid collection contents.

Once Tree owns the property, update the runtime mapping path to target the interface rather than the concrete prop. `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs` should continue to map between `TreePropDraft` and `TreeProp`, but the clone work should use the shared helper/factory. Any mapper or helper that only needs rotation access should read through `ICanRotate` where practical rather than assuming the concrete prop type. The visual-input mapper in `Props.Runtime/Tree/Visuals/TreePropToVisualInputMapper.cs` should snapshot `AxisRotations` from the prop’s interface-backed property exactly as it does today, but now without relying on inherited base-class storage.

After the runtime code is updated, clean the tests. Remove the stale `AxisRotations` assignment from `PropCentric.Tests/PolyLine/PolyLineTestData.cs`. Add or update tests to prove that Tree still maps and previews correctly, that PolyLine no longer exposes rotation state at compile-time through its test helpers, and that the new helper/factory returns distinct model instances when cloning.

Before final validation, update the documentation under `Docs/` that describes feature interfaces, prop setup flow, or runtime ownership so a new contributor can learn the new rule from the repository docs instead of reverse-engineering the code. At minimum, review `Docs/poc-system-overview.md`, `Docs/core-design-goals.md`, and `Docs/rotation-feature-page-execplan.md`, and update any wording that still implies rotations are universal prop state or that `ICanRotate` is marker-only.

Before the final validation pass, align the repository docs with the now-complete `AxisRotation` naming. The code has already been normalized to `ICanAxisRotate` and `IHasAxisRotationsDraft`; the docs must now explain those names consistently.

Finally, run the full solution build and test commands and then manually verify the Tree and PolyLine wizard flows in the harness. The user-visible wizard behavior should be unchanged from the previous milestone, but the runtime ownership boundary will now match the feature boundary, the draft contract location will match its setup-only purpose, and the repository documentation will describe those boundaries accurately.

## Concrete Steps

Work from `C:\Dev\PropCentric`.

1. Re-read the current runtime ownership points:

       Get-Content Props.Abstractions/Features/ICanRotate.cs
       Get-Content Props.Abstractions/Props/BaseProp.cs
       Get-Content Props.Runtime/Tree/TreeProp.cs
       Get-Content Props.Runtime/Tree/Setup/TreePropDraftMapper.cs
       Get-Content Props.Runtime/Tree/Visuals/TreePropToVisualInputMapper.cs
       Get-Content PropCentric.Tests/PolyLine/PolyLineTestData.cs

2. Edit `Props.Abstractions/Features/ICanRotate.cs` so it exposes `AxisRotations`.

3. Add a static helper/factory file under `Props.Abstractions` that provides:

       ObservableCollection<AxisRotationModel> CreateDefaultAxisRotations()
       ObservableCollection<AxisRotationModel> Clone(IEnumerable<AxisRotationModel> rotations)

4. Remove `AxisRotations` from `Props.Abstractions/Props/BaseProp.cs`.

5. Add the property to `Props.Runtime/Tree/TreeProp.cs`, initialize it from the helper, and update the summary generation to enumerate rotations safely.

6. Update `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs` and any Tree rotation mapping code to use the shared clone helper.

7. Clean tests, especially `PropCentric.Tests/PolyLine/PolyLineTestData.cs`, and add any missing coverage for the helper/factory.

8. Update the relevant files in `Docs/` so the feature, draft, and runtime ownership model matches the implementation and uses the normalized `AxisRotation` names.

9. Run:

       dotnet build PropCentric.sln
       dotnet test PropCentric.Tests/PropCentric.Tests.csproj
       dotnet run --project PropCentric/PropCentric.csproj

10. Manually verify:

   1. Open the Tree wizard.
   2. Confirm the Rotation page still appears.
   3. Change one or more rotation values and confirm preview still updates.
   4. Finish the wizard, reopen Tree, and confirm the values persisted.
   5. Open the PolyLine wizard and confirm there is still no Rotation page.

## Validation and Acceptance

Acceptance requires both code-level and user-visible proof.

At the code level, the `AxisRotation`-named capability contract must be the only interface that exposes runtime prop rotation state. `BaseProp<TModel>` must no longer compile with an `AxisRotations` property. `TreeProp` must compile because it explicitly implements `ICanAxisRotate`, and `PolyLineProp` must compile without any rotation property at all.

At the test level, run `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` and expect all tests to pass. Add or update tests so the following are proved:

- `ICanRotate` still maps to `PropFeatureFlags.Rotation`.
- Tree draft mapping and visual-input mapping still preserve rotation values.
- the new static helper/factory returns three default zero-degree rotations in X, Y, Z order.
- the clone helper returns distinct `AxisRotationModel` instances with equal values.
- PolyLine test helpers no longer assign runtime rotations because PolyLine no longer exposes them.

At the documentation level, `Docs/poc-system-overview.md` and any other touched `Docs/*.md` files must explain that axis-rotation runtime state is exposed by `ICanAxisRotate`, not by `BaseProp<TModel>`, and that shared initialization and cloning are provided by `AxisRotationCollectionFactory`.

At the abstraction-layout level, setup-only draft capability interfaces must live under `Props.Abstractions/Setup/Drafts` rather than `Props.Abstractions/Features`, and the codebase must compile with the new namespaces.

At the harness level, Tree must still expose the Rotation feature page and preserve the edited values, while PolyLine must still expose no Rotation page. If the harness behavior changes, the implementation is incomplete even if the code compiles.

## Idempotence and Recovery

This refactor is safe to perform incrementally. The best order is: update the interface and helper first, then move Tree ownership, then remove the base-class property, then clean tests. If the repository stops compiling after removing `BaseProp<TModel>.AxisRotations`, search for all remaining references and move them either to `ICanRotate` or to `TreeProp` explicitly.

The static helper/factory should be additive and safe to reuse. If the first implementation location feels wrong, it can be moved without affecting behavior as long as all call sites are updated consistently and the public methods remain the same.

If a partial step leaves tests broken, recovery is straightforward: restore compile success by ensuring every remaining runtime `AxisRotations` access goes through either `ICanRotate` or `TreeProp`, and ensure PolyLine tests no longer assume inherited rotation state.

## Artifacts and Notes

The intended end-state relationships are:

    Runtime rotation-supporting prop path:
        TreeProp implements the AxisRotation capability contract
        TreeProp owns ObservableCollection<AxisRotationModel> AxisRotations
        AxisRotationCollectionFactory creates default and cloned collections
        TreePropDraftMapper clones between TreeProp and TreePropDraft
        TreePropToVisualInputMapper snapshots the prop-owned rotations

    Runtime non-rotation prop path:
        PolyLineProp does not implement ICanRotate
        PolyLineProp has no AxisRotations property
        PolyLine tests cannot assign runtime rotations
        Rotation feature resolution still excludes PolyLine

    Draft abstraction path:
        IHasAxisRotationsDraft lives under Props.Abstractions/Setup/Drafts
        IHasSegmentsDraft lives under Props.Abstractions/Setup/Drafts
        draft capability interfaces are setup-only shapes, not runtime prop features

    Wizard path:
        RotationFeatureWizardPage still resolves from the AxisRotation capability contract
        RotationFeatureWizardPage still edits IHasAxisRotationsDraft
        setup wrappers still map between prop runtime state and draft state

The helper/factory should be small and intentionally boring. It exists to prevent repeated code such as:

    new ObservableCollection<AxisRotationModel>
    {
        new AxisRotationModel { Axis = Axis.XAxis, RotationAngle = 0 },
        new AxisRotationModel { Axis = Axis.YAxis, RotationAngle = 0 },
        new AxisRotationModel { Axis = Axis.ZAxis, RotationAngle = 0 }
    }

and repeated clone loops across props and mappers.

## Interfaces and Dependencies

In `Props.Abstractions/Features/ICanAxisRotate.cs`, define:

    [PropFeature(PropFeatureFlags.Rotation)]
    public interface ICanAxisRotate
    {
        ObservableCollection<AxisRotationModel> AxisRotations { get; set; }
    }

In `Props.Abstractions/PropVisualModels/AxisRotationCollectionFactory.cs`, define a static class with methods shaped like:

    public static ObservableCollection<AxisRotationModel> CreateDefaultAxisRotations()
    public static ObservableCollection<AxisRotationModel> Clone(IEnumerable<AxisRotationModel> rotations)

The clone method must create new model instances rather than returning the same objects.

In `Props.Abstractions/Props/BaseProp.cs`, remove `AxisRotations` entirely. Do not add it to `Props.Abstractions/Props/IProp.cs`.

In `Props.Runtime/Tree/TreeProp.cs`, implement `ICanRotate` by adding the property:

    public ObservableCollection<AxisRotationModel> AxisRotations { get; set; }
        = AxisRotationCollectionFactory.CreateDefaultAxisRotations();

If `TreeProp` needs change notification for replacing the collection instance, use the existing property-change pattern already used elsewhere in the class.

In `Props.Runtime/Tree/Setup/TreePropDraftMapper.cs`, replace local rotation clone loops with the shared helper so there is one standard clone behavior for runtime props and drafts.

At the end of this plan, `ICanAxisRotate` is both the feature-discovery gate and the prop runtime contract, `IHasAxisRotationsDraft` is the matching draft-side contract, `BaseProp<TModel>` no longer exposes rotation state, Tree owns rotations explicitly, PolyLine does not own them at all, the shared static helper/factory eliminates repeated rotation initialization and clone code, and the regular repository docs explain that `AxisRotations` are a setup-time baseline prop-definition capability rather than runtime rendered motion.

Revision note: created on 2026-05-21 to finish the runtime ownership refactor after the reusable rotation feature-page migration. This plan exists because the wizard behavior is correct, but the runtime rotation contract still leaks through `BaseProp<TModel>` instead of being owned by `ICanRotate`.
Revision note: updated on 2026-05-21 to make repository documentation updates an explicit implementation and acceptance step, so the `Docs/` guidance stays aligned with the runtime-contract refactor.
Revision note: updated on 2026-05-21 after the first implementation slice landed. The plan now records that `ICanRotate` exposes `AxisRotations`, the shared static helper/factory exists, and focused tests for the helper pass.
Revision note: updated on 2026-05-21 after the runtime ownership move landed. The plan now records that `BaseProp<TModel>` no longer owns rotations, `TreeProp` owns them explicitly, and stale PolyLine test usage of inherited rotations has been removed.
Revision note: updated on 2026-05-21 after the clone-helper migration landed. The plan now records that Tree draft mapping uses `AxisRotationCollectionFactory.Clone(...)` and that the remaining work is docs plus final validation.
Revision note: updated on 2026-05-21 to add a dedicated `Setup/Drafts` relocation step for draft capability interfaces such as `IHasRotationsDraft` and `IHasSegmentsDraft`, because more setup-only draft contracts are expected and should be segregated from runtime feature contracts.
Revision note: updated on 2026-05-21 after the `Setup/Drafts` relocation landed. The plan now records that draft-only setup capability interfaces and `SegmentDraftState` live under `Props.Abstractions/Setup/Drafts`, with updated runtime and test references.
Revision note: updated on 2026-05-21 to add an explicit naming-normalization step so the remaining rotation contracts are renamed around `AxisRotation` for consistency with `AxisRotationModel`, `AxisRotationCollectionFactory`, and `AxisRotations`.
Revision note: updated on 2026-05-21 after the naming-normalization slice landed. The plan now records that `ICanAxisRotate` and `IHasAxisRotationsDraft` are the active contract names, with renamed files and updated discovery/runtime/wizard/test references.
Revision note: updated on 2026-05-21 after the final documentation and validation slice landed. The plan now records that the regular docs describe `AxisRotations` as setup-time baseline prop-definition state, and that the full solution build and tests pass when run sequentially.
