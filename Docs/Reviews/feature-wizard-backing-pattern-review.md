# Feature Wizard Backing Pattern Review

## Scope

This review covers the current feature wizard backing patterns across:

- `Props.Abstractions/Features/IFeatureWizardDataMapper.cs`
- `Props.Abstractions/Features/IFeatureWizardDraftPage.cs`
- `Props.Abstractions/Features/IFeatureWizardPageResolver.cs`
- `Props.Registry/FeatureWizardPageResolver.cs`
- `Props.Runtime/Tree/TreePropSetup.cs`
- `Props.Runtime/PolyLine/PolyLinePropSetup.cs`
- `Props.Runtime/Wizards/Features/Dimming/*`
- `Props.Runtime/Wizards/Features/Color/*`
- `Props.Runtime/Wizards/Features/Rotation/*`
- `Props.Runtime/Wizards/Features/Segments/*`

The question under review is whether the repository should continue to support both legacy mapper-backed feature pages and draft-backed feature pages, or consolidate on one backing pattern that works consistently for all wizard pages, including pages that do and do not host the preview viewer.

## Findings

### 1. The repository currently has two different feature-page state lifecycles

Severity: High

The feature wizard system still supports two incompatible state ownership models:

- mapper-backed pages own page-local state and rely on `IFeatureWizardDataMapper` to populate from and apply to the prop
- draft-backed pages receive the shared `IPropDraft` plus `IWizardPreviewSession` and either edit draft state directly or synchronize page-facing state back into the draft during editing

This split is visible in both the abstractions and the setup flow:

- `Props.Abstractions/Features/IFeatureWizardPageResolver.cs` exposes both `GetMappersFor(...)` and `InitializePages(...)`
- `Props.Runtime/Tree/TreePropSetup.cs` and `Props.Runtime/PolyLine/PolyLinePropSetup.cs` both perform two separate feature-page setup/apply phases
- `Props.Runtime/Wizards/Features/Dimming/Pages/DimmingFeatureWizardPage.cs` is still mapper-backed while Color, Rotation, and Segments are draft-backed

Impact:

- every setup wrapper must remember both initialization paths and both write-back paths
- feature authors must choose between two patterns for conceptually identical wizard responsibilities
- the architecture no longer has one obvious “source of truth” for wizard-editable feature state

This is the biggest obstacle to a consistent prop wizard architecture.

### 2. Mapper-backed pages violate the repository’s preferred wizard-state boundary

Severity: High

The architecture docs repeatedly state that wizard pages should edit wizard-owned draft state rather than page-owned state or the prop directly. The mapper-backed pattern keeps feature state outside the shared draft until wizard completion.

Concrete example:

- `Props.Runtime/Wizards/Features/Dimming/Pages/DimmingFeatureWizardPage.cs` stores `Brightness` and `Gamma` on the page itself
- `Props.Runtime/Wizards/Features/Dimming/Mappers/DimmingFeatureWizardDataMapper.cs` copies values prop -> page before the wizard and page -> prop after confirmation

That is inconsistent with:

- `Docs/feature-wizards-requirements.md`
- `Docs/poc-system-overview.md`
- `Docs/core-design-goals.md`

Impact:

- dimming edits are not part of the shared draft during the wizard
- preview cannot naturally consume dimming changes through the same draft pipeline used by other pages
- page-local state becomes another place that can drift from draft state and prop state

### 3. The setup wrappers still carry legacy orchestration complexity because of the dual pattern

Severity: Medium

`TreePropSetup` and `PolyLinePropSetup` already follow the shared-draft pattern for prop pages and for newer feature pages, but they still need extra loops for legacy feature mappers:

- initialize draft-backed pages
- build mapper list for mapper-backed pages
- populate mapper-backed pages from the prop before showing the wizard
- apply mapper-backed page data back to the prop after wizard acceptance

That complexity is purely transitional. It does not express business rules; it exists only because the architecture supports two backing models.

Impact:

- more boilerplate in every setup wrapper
- more branches to preserve in future setup wrappers
- harder testing because setup behavior depends on page type, not on one uniform contract

### 4. Draft-backed pages are directionally correct, but the internal style is still uneven

Severity: Medium

The newer pages all use the shared draft, but they do not yet present a single internal pattern:

- `SegmentsFeatureWizardPage` wraps shared draft items and mutates them directly through `SegmentFeatureWizardItem`
- `RotationFeatureWizardPage` wraps shared draft items and mutates them directly through `RotationFeatureWizardItem`
- `ColorFeatureWizardPage` keeps more page-local helper state, then pushes snapshots back into `draft.ColorConfiguration`

This is not a functional problem, but it means “draft-backed” still covers two sub-patterns:

- direct wrapper over mutable draft-owned child objects
- page-local projection plus explicit synchronization back to the draft

Impact:

- feature-page authors still need judgment about where temporary UI state should live
- the long-term target pattern should define when page-local helper state is allowed and what must remain canonical on the draft

### 5. Preview capability should be orthogonal to backing-state ownership

Severity: Medium

The current code already demonstrates the right separation:

- `SegmentsFeatureWizardPage` and `RotationFeatureWizardPage` use the preview session and host the viewer path
- `ColorFeatureWizardPage` receives the same preview session but does not host the OpenGL viewer

This is promising because it means the repository does not need two backing patterns to support “viewer pages” versus “non-viewer pages.” The preview session can be shared across all feature pages, and each page can choose whether to render or merely keep preview data current.

Impact:

- the target architecture should not split by “has viewer” versus “does not have viewer”
- backing state should always be draft-owned; preview usage should be optional behavior layered on top

## Recommendation

Consolidate on one feature-page backing model:

- all wizard-editable feature state lives in the shared prop draft
- all feature wizard pages are initialized against the shared draft for the current wizard instance
- all feature pages may also receive the shared `IWizardPreviewSession`, regardless of whether they host the viewer
- feature pages may maintain page-local helper/projection state only as a UI convenience, but the draft remains the only canonical backing store
- setup wrappers should stop resolving, populating, and applying feature-specific data mappers once migration is complete

In practice, this means the current draft-backed approach should become the only supported pattern, and the legacy mapper-backed pattern should be retired.

## Proposed Target Contract Shape

The current direction can be preserved while simplifying naming and intent. The simplest evolution is:

- keep a single feature-page initialization contract centered on shared draft state
- treat preview-session access as part of the same per-wizard context, not as a separate architecture branch
- remove `IFeatureWizardDataMapper` and `FeatureWizardPageAttribute.MapperType` after all pages are migrated
- simplify `IFeatureWizardPageResolver` so it resolves and initializes pages only

A reasonable end state is either:

- keep `IFeatureWizardDraftPage.Initialize(IPropDraft, IWizardPreviewSession)` as the one feature-page contract, or
- replace it with a clearer context object such as `FeatureWizardContext` containing `Draft` and `PreviewSession`

The context-object variant is slightly better for future growth because it can absorb more wizard-scoped services without another interface churn.

## Migration Priority

1. Migrate Dimming to the shared-draft pattern.
2. Add draft contracts for dimming state, likely under `Props.Abstractions/Setup/Drafts`.
3. Update Tree and PolyLine drafts plus draft mappers so dimming round-trips through the draft.
4. Change setup wrappers and resolver interfaces so draft initialization becomes the only feature-page setup path.
5. Remove legacy mapper abstractions and mapper metadata after the migration is complete.

## Residual Risks

- `ColorFeatureWizardPage` demonstrates that some UI-heavy pages may still need page-local projection models even when the draft is canonical. The implementation plan should explicitly permit this.
- Removing mapper support is a cross-cutting change because it touches abstractions, resolver logic, setup wrappers, discovery metadata, and tests.
- Dimming has no current draft contract, so the migration must define one without polluting runtime feature interfaces with setup-only concerns.

## Summary

The current codebase is already more committed to draft-backed feature editing than to mapper-backed editing. Only the dimming path remains fully legacy. The strongest architectural move is to finish that migration and make “shared draft + shared preview session” the only wizard backing pattern.
