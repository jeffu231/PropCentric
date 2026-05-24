# Add Live Preview Sessions To Feature Wizard Pages

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/plans.md`.

## Purpose / Big Picture

After this change, a feature wizard page can host the same OpenGL preview surface that today only appears on prop-specific wizard pages, and the preview will react to edits made on that feature page without waiting for the user to finish the wizard. The first concrete outcome is that the `SegmentsFeatureWizardPage` for `IHasSegments` will show the preview and changing any segment `PointCount` will immediately redraw the polyline lights on that same page.

The user-visible proof is straightforward. Start the harness, open the `PolyLineProp` setup flow, navigate to the Segments page, and change a segment point count. The preview on the right side of that feature page should redraw immediately, and when the user navigates back to the prop page or forward to the summary page the totals should remain consistent because the shared wizard draft was updated in place.

## Progress

- [x] (2026-05-11 15:16Z) Read `.agents/plans.md`, `Docs/poc-system-overview.md`, `Docs/segmentable-props.md`, `Docs/core-design-goals.md`, and `Docs/feature-wizards-requirements.md`.
- [x] (2026-05-11 15:16Z) Inspected the current implementation in `Props.Runtime/PolyLine`, `Props.Runtime/Tree`, `Props.Runtime/Wizards/Core`, and `Props.Runtime/Wizards/Features`.
- [x] (2026-05-11 15:16Z) Chosen the design direction: keep the existing feature-page discovery mechanism, add an additive draft-backed feature-page capability, and introduce a per-wizard preview session.
- [x] (2026-05-11 15:16Z) Implemented the shared preview-session abstractions, `FeatureWizardPageResolver.InitializePages(...)`, setup-wrapper wiring in Tree and PolyLine, and focused tests for preview-session delegation and draft-backed page initialization.
- [x] (2026-05-11 15:16Z) Ran targeted validation with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~WizardPreviewSessionTests|FullyQualifiedName~PolyLinePropSetupTests"` and confirmed the affected tests pass.
- [x] (2026-05-11 15:16Z) Generalized the graphics preview base and its prop/light view-model layers to store `IPropVisualModel` instead of concrete visual-model generics, and updated Tree/PolyLine prop-page view models to compile against the new base.
- [x] (2026-05-11 15:16Z) Ran targeted validation with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~WizardPreviewSessionTests|FullyQualifiedName~PolyLinePropSetupTests|FullyQualifiedName~TreeWizardPreviewCoordinatorTests|FullyQualifiedName~PolyLineWizardPreviewCoordinatorTests"` and confirmed the affected tests pass.
- [x] (2026-05-11 15:16Z) Added the shared `IHasSegmentsDraft` contract and `SegmentDraftState` model, migrated `PolyLinePropDraft` to that shared segment draft shape, and updated the polyline draft mapper and affected tests.
- [x] (2026-05-11 15:16Z) Converted `SegmentsFeatureWizardPage`, its view model, and its XAML view to the draft-backed preview-enabled path, and removed the active `SegmentsFeatureWizardDataMapper` path from discovery and runtime use.
- [x] (2026-05-11 15:16Z) Ran targeted validation with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~SegmentsFeatureWizardPageTests|FullyQualifiedName~PolyLinePropSetupTests|FullyQualifiedName~PolyLineDraftMappingTests|FullyQualifiedName~PolyLineVisualInputMappingTests|FullyQualifiedName~PolyLineWizardPreviewCoordinatorTests|FullyQualifiedName~WizardPreviewSessionTests|FullyQualifiedName~PropDiscoveryTests"` and confirmed the affected tests pass.
- [x] (2026-05-11 15:16Z) Manual harness validation confirmed the Segments page OpenGL preview updates immediately when `PointCount` changes and remains consistent when navigating back to the polyline basics page.
- [x] (2026-05-11 15:16Z) Updated `Docs/poc-system-overview.md`, `Docs/segmentable-props.md`, and `Docs/feature-wizards-requirements.md` to document draft-backed feature pages, preview sessions, and the implemented Segments-page live-preview flow.

## Surprises & Discoveries

- Observation: the current feature-page data flow duplicates state instead of sharing it. `SegmentsFeatureWizardDataMapper` copies data from `IHasSegments` on the prop into page-owned `SegmentFeatureWizardItem` objects, while `PolyLineWizardPreviewCoordinator` reads only from `PolyLinePropDraft`. A user edit on the page therefore cannot reach preview state until the wizard is accepted.
  Evidence: `Props.Runtime/Wizards/Features/Segments/Mappers/SegmentsFeatureWizardDataMapper.cs` populates `page.Segments` from `segmentsProp.Segments`, while `Props.Runtime/PolyLine/Visuals/PolyLineWizardPreviewCoordinator.cs` accepts only `PolyLinePropDraft`.

- Observation: the existing OpenGL page infrastructure is already reusable in spirit, but the current generic constraint ties it to a concrete prop visual-model type and assumes the page itself creates a `new()` model instance.
  Evidence: `Props.Runtime/Wizards/Core/ViewModels/GraphicsWizardPageViewModelBase.cs` is declared as `GraphicsWizardPageViewModelBase<TWizardPage, TPropModel> where TPropModel : class, IPropVisualModel, new()`.

- Observation: feature pages are created by discovery and DI with no runtime wizard context, but the setup wrappers already own the shared draft and the preview coordinator at the moment the pages are added to the wizard.
  Evidence: `TreePropSetup` and `PolyLinePropSetup` call `featurePageResolver.GetPagesFor(...)` and then add the pages, while each setup wrapper also creates the prop draft and already resolves `IWizardPreviewCoordinator<TDraft>`.

- Observation: the setup-wrapper tests already used a local `TestFeatureWizardPageResolver`, which made it straightforward to add the new initialization method without disturbing unrelated discovery tests.
  Evidence: `PropCentric.Tests/PolyLine/PolyLinePropSetupTests.cs` now verifies that a draft-backed test page receives both the shared draft and the preview session during `EditAsync(...)`.

- Observation: the full test suite currently contains an unrelated failing assertion in `TreeVisualModelBuilderTests`, so milestone validation for this slice has to rely on the targeted affected-test run until that baseline issue is addressed.
  Evidence: `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` failed at `PropCentric.Tests.TreeVisualModelBuilderTests.TreeVisualModelBuilder_Create_WithRotationInput_TransformsPointPositions` with `Assert.NotEqual() Failure: Values are equal`.

- Observation: the graphics-base refactor did not require changes to the preview coordinators because they already returned `IPropVisualModel`; the concrete visual-model coupling existed only in the WPF wizard view-model hierarchy.
  Evidence: `TreeWizardPreviewCoordinator` and `PolyLineWizardPreviewCoordinator` were unchanged, while `GraphicsWizardPageViewModelBase`, `PropBaseWizardPageViewModel`, `LightWizardPageViewModel`, and the two prop-page view models were updated.

- Observation: once `SegmentFeatureWizardItem` became a wrapper over shared draft state, the old mapper class was not just unnecessary but incompatible because it assumed page-owned segment objects with public setters and a parameterless constructor.
  Evidence: `Props.Runtime/Wizards/Features/Segments/Mappers/SegmentsFeatureWizardDataMapper.cs` had to be removed after `SegmentsFeatureWizardPage` switched to `SegmentFeatureWizardItem(SegmentDraftState segment)` and stopped advertising a mapper in `FeatureWizardPageAttribute`.

- Observation: after the implementation landed, the repository docs still described Segments as a mapper-backed feature page and did not mention `IWizardPreviewSession` or draft-backed feature pages, so doc alignment was necessary to avoid steering future work back toward the old pattern.
  Evidence: the pre-update versions of `Docs/poc-system-overview.md`, `Docs/segmentable-props.md`, and `Docs/feature-wizards-requirements.md` referenced `SegmentsFeatureWizardDataMapper` and did not describe draft-backed feature-page initialization.

## Decision Log

- Decision: add an additive draft-backed feature-page capability instead of rewriting every existing feature page to be draft-only in one pass.
  Rationale: this delivers the requested Segments-page behavior now, keeps existing `DimmingFeatureWizardPage` behavior working, and allows future feature pages to opt into live preview without forcing a large migration across all props in one change.
  Date/Author: 2026-05-11 / Codex

- Decision: introduce a per-wizard preview session object that owns the shared draft reference plus the preview coordinator and is passed to any page that wants live preview.
  Rationale: the preview session makes the preview API page-agnostic. Prop-specific pages and feature pages can both ask for “build the current preview from the shared draft” without duplicating coordinator logic or depending on a specific prop page.
  Date/Author: 2026-05-11 / Codex

- Decision: keep the single source of truth for preview-driving feature edits in the shared wizard draft, not in page-owned copies.
  Rationale: the repository docs already state that wizard state is draft-owned. The live-preview requirement fails precisely because the Segments page currently holds a second copy of segment state. Updating the shared draft in place removes that divergence.
  Date/Author: 2026-05-11 / Codex

- Decision: generalize the graphics page base to operate on `IPropVisualModel` instead of requiring a concrete `TPropModel : new()`.
  Rationale: a reusable feature page such as Segments cannot know at compile time which concrete prop visual model it will preview in the future. The preview session already returns `IPropVisualModel`, so the base class should accept that abstraction directly.
  Date/Author: 2026-05-11 / Codex

- Decision: remove `SegmentsFeatureWizardDataMapper` entirely after converting the Segments page to a draft-backed page instead of leaving it dormant.
  Rationale: the converted page now edits shared draft segment state directly, so keeping a mapper built for page-owned segment copies would preserve dead code around the exact duplication problem this change removed.
  Date/Author: 2026-05-11 / Codex

## Outcomes & Retrospective

The first implementation slice is now in place. The repository has shared preview-session abstractions, setup wrappers create a preview session per wizard flow, and the feature-page resolver can initialize draft-backed pages without affecting legacy mapper-backed pages.

The second implementation slice is also now in place. The wizard graphics base no longer depends on a concrete preview model type, which removes the main technical blocker for reusable feature pages such as Segments to host the OpenGL viewer.

The third implementation slice is now in place as well. The Segments feature page now binds to shared draft segment state, advertises no mapper, hosts the OpenGL preview surface directly, and schedules preview rebuilds when any segment point count changes.

Validation for the affected path is now broad enough to cover discovery, draft mapping, setup flow, preview coordination, preview-session plumbing, and the new segments page behavior. Manual harness validation is still pending, and full-suite validation is still blocked by the unrelated existing tree visual-model rotation test failure.

Manual harness validation is now complete and matched the intended user-visible behavior. The remaining validation gap is a clean full-suite run, which is still blocked by the unrelated existing tree visual-model rotation test failure.

## Context and Orientation

The repository already has a live preview pipeline, but it is currently scoped to prop-specific wizard pages. The key files are `Props.Abstractions/Visuals/IWizardPreviewCoordinator.cs`, `Props.Runtime/PolyLine/Visuals/PolyLineWizardPreviewCoordinator.cs`, `Props.Runtime/Tree/Visuals/TreeWizardPreviewCoordinator.cs`, and `Props.Runtime/Wizards/Core/ViewModels/GraphicsWizardPageViewModelBase.cs`. Those files define the current behavior where a page view model asks a preview coordinator to map the current draft into visual input, reuse cached preview state when possible, and return an `IPropVisualModel` that the OpenGL drawing engine renders.

Feature pages are discovered separately from props. `Props.Registry/FeatureWizardPageResolver.cs` returns the feature pages for a given prop type and can also construct optional page-specific data mappers declared through `FeatureWizardPageAttribute`. Today those mappers are prop-facing, not draft-facing. For example, `Props.Runtime/Wizards/Features/Segments/Mappers/SegmentsFeatureWizardDataMapper.cs` copies segment data from the prop into the page and then writes it back to the prop only after the wizard is accepted.

The Segments feature page itself lives in `Props.Runtime/Wizards/Features/Segments/Pages/SegmentsFeatureWizardPage.cs` with its WPF view in `Props.Runtime/Wizards/Features/Segments/Views/SegmentsFeatureWizardPageView.xaml` and view model in `Props.Runtime/Wizards/Features/Segments/ViewModels/SegmentsFeatureWizardPageViewModel.cs`. It currently owns an `ObservableCollection<SegmentFeatureWizardItem>` that has no connection to `PolyLinePropDraft`, which is why editing a point count cannot affect the preview.

The shared draft for the polyline flow lives in `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs`. That draft already contains the geometry and light-count data the preview coordinator consumes. The correct architectural direction is therefore to let the Segments feature page edit draft-backed segment state instead of a page-local copy.

Within this document, “draft-backed page” means a wizard page whose editable fields are either direct views over the shared `IPropDraft` instance or thin wrappers around objects owned by that draft. “Preview session” means a small object created once per wizard instance that exposes the shared draft and a method that returns the latest preview model for that draft.

## Milestones

### Milestone 1: Introduce preview sessions and draft-backed feature-page initialization

At the end of this milestone, prop setup wrappers will create a preview session once per wizard instance and will initialize any feature page that opts into the new capability. Existing feature pages that do not opt in will continue to use the current mapper-based behavior. No user-visible UI change is required yet, but the setup flow will now have the plumbing required for live preview on feature pages.

The proof for this milestone is a focused test run. Add unit tests that create a prop draft, create a preview session, and verify that a feature page implementing the new interface receives the shared draft instance and can be initialized without breaking legacy page resolution. Run `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` from `C:\Dev\PropCentric` and confirm the new tests pass alongside the existing resolver and preview tests.

### Milestone 2: Generalize the graphics preview view-model base

At the end of this milestone, preview-capable page view models will no longer need to know a concrete visual-model type at compile time. The shared base will accept the `IPropVisualModel` returned by the preview session and feed it into the OpenGL drawing engine. Existing prop-specific pages for Tree and PolyLine must still render normally after this refactor.

The proof for this milestone is a combination of tests and manual smoke validation. Re-run the existing preview coordinator tests and open both the Tree and PolyLine setup flows in the harness to confirm their existing preview panes still render and respond to edits.

### Milestone 3: Convert the Segments feature page to the new capability

At the end of this milestone, the Segments feature page will display the OpenGL preview surface and changing a segment `PointCount` will rebuild the preview immediately from the shared polyline draft. The old duplicate state path through `SegmentsFeatureWizardDataMapper` will be removed or bypassed for this page so there is only one editable source of segment truth during the wizard flow.

The proof for this milestone is the full end-to-end scenario. Open the PolyLine wizard, navigate to Segments, change a point count, and observe that the number of previewed points changes immediately on the same page. Navigate back to the prop page and verify the totals remain aligned because both pages are reading the same draft state.

## Plan of Work

Start in `Props.Abstractions/Visuals` by adding two new interfaces: `IWizardPreviewSession` and `IWizardPreviewSession<TDraft>`. The non-generic interface should expose `IPropDraft Draft { get; }` and `IPropVisualModel BuildPreview()`. The generic interface should narrow `Draft` to the concrete draft type. The implementation belongs in `Props.Runtime/Wizards/Core/Preview/WizardPreviewSession.cs` and should simply hold the shared draft instance and delegate preview generation to the existing `IWizardPreviewCoordinator<TDraft>`.

Add a new feature-page opt-in contract in `Props.Abstractions/Features/IFeatureWizardDraftPage.cs`. Keep it non-generic so discovery and setup code can work with it uniformly:

    public interface IFeatureWizardDraftPage
    {
        void Initialize(IPropDraft draft, IWizardPreviewSession previewSession);
    }

The intent is that only pages that need live access to the draft or the preview session implement this interface. Legacy pages such as `DimmingFeatureWizardPage` can remain unchanged until there is a reason to migrate them.

Update `Props.Registry/FeatureWizardPageResolver.cs` or add a small helper service next to it so setup wrappers can initialize pages through one shared code path. The preferred shape is a new resolver method:

    void InitializePages(IReadOnlyList<IWizardPage> pages, IPropDraft draft, IWizardPreviewSession previewSession)

That method should loop through the pages, detect `IFeatureWizardDraftPage`, and call `Initialize(...)`. It should do nothing for legacy pages. This keeps the runtime-specific initialization policy in one place instead of duplicating it in every prop setup wrapper.

Modify `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` so each setup flow creates a preview session immediately after populating the prop draft and before constructing the wizard pages. The same preview session instance must then be used for the prop-specific page and for `featurePageResolver.InitializePages(...)`. Keep the existing `IFeatureWizardDataMapper` path for legacy pages. For draft-backed pages, stop applying prop-bound mappers on accept.

Refactor `Props.Runtime/Wizards/Core/ViewModels/GraphicsWizardPageViewModelBase.cs` so it no longer requires a concrete `TPropModel : new()`. Replace the `PropVisualModel` property with an `IPropVisualModel? CurrentPreviewModel` property, keep the `OpenGLPropDrawingEngine`, and update `TriggerPreviewRebuild()` to set the drawing engine model list from the interface instance returned by `PreviewBuilder`. This change lets a feature page render any prop visual model supplied by the preview session. Update the Tree and PolyLine prop-page view models to compile against the new base signature and continue to assign `PreviewBuilder`.

Add a feature-specific draft contract for segments in `Props.Abstractions/Features/IHasSegmentsDraft.cs`. This interface should expose a mutable ordered segment collection that feature pages can edit without knowing the concrete prop draft type. To avoid placing polyline-specific setup types in abstractions, also add a small shared segment draft model, for example `SegmentDraftState`, in the same area. Then update `Props.Runtime/PolyLine/Setup/PolyLinePropDraft.cs` to implement `IHasSegmentsDraft` and replace the current local `SegmentDraftItem` type with the shared model.

Convert `Props.Runtime/Wizards/Features/Segments/Pages/SegmentsFeatureWizardPage.cs` into a draft-backed page. Remove the attribute mapper reference from `[FeatureWizardPage(...)]` because this page should no longer depend on `SegmentsFeatureWizardDataMapper`. Implement `IFeatureWizardDraftPage.Initialize(...)` by validating that `draft` implements `IHasSegmentsDraft`, storing the shared draft reference, creating wrapper items over that draft collection, and storing the preview session. Each `SegmentFeatureWizardItem` should proxy `PointCount` reads and writes to its underlying `SegmentDraftState` so page edits immediately update the shared draft.

Update `Props.Runtime/Wizards/Features/Segments/ViewModels/SegmentsFeatureWizardPageViewModel.cs` to derive from the generalized graphics base instead of plain `WizardPageViewModelBase`. In the constructor, set `PreviewBuilder = () => featureWizardPage.PreviewSession.BuildPreview();`. Keep the existing validation logic, but validate against the draft-backed items. The page should still own totals and summary text because those are view concerns, but it should no longer own the actual segment counts independently of the draft.

Replace the Segments view in `Props.Runtime/Wizards/Features/Segments/Views/SegmentsFeatureWizardPageView.xaml` with a two-column layout similar to `PolyLinePropWizardPageView.xaml`: the editable grid on the left, a bordered `GLWpfControl` on the right, and the same render and mouse event bindings already used by `WizardPageViewBase`. The code-behind file should inherit from `WizardPageViewBase` behavior just like the prop-page views so the OpenGL control is initialized and rendered consistently.

Delete `Props.Runtime/Wizards/Features/Segments/Mappers/SegmentsFeatureWizardDataMapper.cs` if no tests or other pages depend on it after the migration. If a short compatibility window is preferable, leave the class in place but remove it from the attribute and from any setup-wrapper apply loop so it is no longer active. The important rule is that segment edits during wizard execution must have only one active mutable source.

After the live-preview path works for Segments, update `Docs/poc-system-overview.md`, `Docs/segmentable-props.md`, and `Docs/feature-wizards-requirements.md` so they explain the new optional draft-backed feature-page capability, the preview session concept, and the rule that preview-driving feature data must live in the shared wizard draft.

## Concrete Steps

Work from `C:\Dev\PropCentric`.

1. Read the current abstractions and setup flow again before editing:

       Get-Content Props.Abstractions/Visuals/IWizardPreviewCoordinator.cs
       Get-Content Props.Registry/FeatureWizardPageResolver.cs
       Get-Content Props.Runtime/PolyLine/PolyLinePropSetup.cs
       Get-Content Props.Runtime/Wizards/Core/ViewModels/GraphicsWizardPageViewModelBase.cs

2. Add the new preview-session and draft-page interfaces under `Props.Abstractions`, then implement `WizardPreviewSession<TDraft>` under `Props.Runtime/Wizards/Core/Preview`.

3. Update `FeatureWizardPageResolver` with the page-initialization helper and modify Tree and PolyLine setup wrappers to create and pass the preview session.

4. Refactor `GraphicsWizardPageViewModelBase` and update the Tree and PolyLine prop-page view models to compile against the new base.

5. Add `IHasSegmentsDraft` plus the shared segment draft model and migrate `PolyLinePropDraft` and any affected draft-to-visual-input mapping code to use it.

6. Convert the Segments feature page, view model, and XAML view to the draft-backed preview-enabled path.

7. Remove or deactivate the old Segments data mapper path.

8. Add tests, run the test suite, and perform manual harness validation.

The expected command sequence after implementation is:

       dotnet build PropCentric.sln
       dotnet test PropCentric.Tests/PropCentric.Tests.csproj
       dotnet run --project PropCentric/PropCentric.csproj

The expected test transcript should still end with a successful summary similar to:

       Build succeeded.
       Test run for ...\PropCentric.Tests.dll (.NETCoreApp,Version=v10.0)
       Passed!  - Failed: 0, Passed: <updated count>, Skipped: 0, Total: <updated count>

The expected manual behavior in the harness is:

1. Create or edit a `PolyLineProp`.
2. Navigate to the Segments page.
3. Change a `PointCount` value.
4. Observe the preview redraw on the Segments page without leaving the page.
5. Navigate back to the polyline basics page and confirm the total point count matches the edit.

## Validation and Acceptance

Acceptance requires both automated and manual evidence.

Automated acceptance begins with the existing preview tests. `PropCentric.Tests/TreeWizardPreviewCoordinatorTests.cs` and `PropCentric.Tests/PolyLine/PolyLineWizardPreviewCoordinatorTests.cs` must still pass after the graphics-base refactor. Add a new unit test file for the new preview session, for example `PropCentric.Tests/Wizards/WizardPreviewSessionTests.cs`, that verifies the session returns the same draft instance and delegates preview generation to the coordinator.

Add a test for the Segments feature page, for example `PropCentric.Tests/PolyLine/SegmentsFeatureWizardPageTests.cs`, that initializes the page with a `PolyLinePropDraft`, changes a wrapper item `PointCount`, and asserts that the underlying draft segment count changed immediately. Add or update a setup-flow test so `PolyLinePropSetup` initializes draft-backed feature pages and does not depend on `SegmentsFeatureWizardDataMapper`.

Manual acceptance is the PolyLine wizard scenario described above. The important observation is not merely that the page contains a viewer, but that the viewer reflects edits made on that page before the wizard is accepted. If the preview only updates after navigating away or finishing the wizard, the change is incomplete.

## Idempotence and Recovery

The implementation steps are additive and can be repeated safely. Creating a preview session does not mutate persisted prop state; it only wraps the current wizard draft. Re-running the test suite is also safe.

If the graphics-base refactor breaks the existing Tree or PolyLine preview pages, restore the last working shape of `GraphicsWizardPageViewModelBase` first and then reintroduce the abstraction change in a smaller step. Because the preview session is additive, it can remain in place while the view-model base is corrected.

If the Segments page migration exposes hidden dependence on `SegmentsFeatureWizardDataMapper`, temporarily leave the mapper class in the tree but ensure the `FeatureWizardPageAttribute` no longer references it. That keeps the old code available for comparison without allowing two mutable segment state paths to run at the same time.

## Artifacts and Notes

The most important code relationships to preserve during implementation are:

    Tree/PolyLine prop setup:
        prop -> draft via IPropDraftMapper
        draft -> preview via IWizardPreviewSession + IWizardPreviewCoordinator
        draft-backed feature page edits -> shared draft immediately
        draft -> prop on accept via IPropDraftMapper

    Legacy feature page path:
        prop -> page via IFeatureWizardDataMapper.PopulateFrom(prop)
        page -> prop on accept via IFeatureWizardDataMapper.ApplyTo(prop)

    New preview-capable feature page path:
        setup wrapper creates preview session once
        resolver initializes page if it implements IFeatureWizardDraftPage
        page edits shared draft state directly or through wrappers
        page view model calls previewSession.BuildPreview()

The design intentionally allows both feature-page patterns to coexist during migration, but only one pattern should be active for any individual feature page.

## Interfaces and Dependencies

In `Props.Abstractions/Visuals/IWizardPreviewSession.cs`, define:

    public interface IWizardPreviewSession
    {
        IPropDraft Draft { get; }
        IPropVisualModel BuildPreview();
    }

    public interface IWizardPreviewSession<out TDraft> : IWizardPreviewSession
        where TDraft : class, IPropDraft
    {
        new TDraft Draft { get; }
    }

In `Props.Abstractions/Features/IFeatureWizardDraftPage.cs`, define:

    public interface IFeatureWizardDraftPage
    {
        void Initialize(IPropDraft draft, IWizardPreviewSession previewSession);
    }

In `Props.Abstractions/Features/IHasSegmentsDraft.cs`, define:

    public interface IHasSegmentsDraft
    {
        ObservableCollection<SegmentDraftState> Segments { get; }
    }

    public sealed class SegmentDraftState
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public int PointCount { get; set; }
    }

In `Props.Runtime/Wizards/Core/Preview/WizardPreviewSession.cs`, implement:

    public sealed class WizardPreviewSession<TDraft> : IWizardPreviewSession<TDraft>
        where TDraft : class, IPropDraft

The constructor should accept the shared `TDraft draft` and `IWizardPreviewCoordinator<TDraft> coordinator`.

In `Props.Registry/FeatureWizardPageResolver.cs`, add:

    public void InitializePages(
        IReadOnlyList<IWizardPage> pages,
        IPropDraft draft,
        IWizardPreviewSession previewSession)

In `Props.Runtime/Wizards/Core/ViewModels/GraphicsWizardPageViewModelBase.cs`, end with a page base that exposes:

    public OpenGLPropDrawingEngine DrawingEngine { get; }
    protected Func<IPropVisualModel>? PreviewBuilder { get; set; }

and that no longer requires a concrete visual-model type parameter.

In `Props.Runtime/Wizards/Features/Segments/Pages/SegmentsFeatureWizardPage.cs`, implement `IFeatureWizardDraftPage` and expose the preview session to the view model, for example:

    public IWizardPreviewSession PreviewSession { get; private set; }

The Segments page should continue to target `IHasSegments` through `FeatureWizardPageAttribute`, but it should no longer declare `mapperType: typeof(SegmentsFeatureWizardDataMapper)`.

Revision note: updated on 2026-05-11 after manual harness validation and docs alignment so the plan now reflects the completed implementation and the remaining known issue is the unrelated existing full-suite test failure.
