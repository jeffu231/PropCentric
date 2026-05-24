# Bring The Color Feature Wizard Up To Catel WPF Standards

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md`.

This plan focuses on the reusable Color feature wizard page under `Props.Runtime/Wizards/Features/Color`. The goal is to preserve the current user-facing behavior while aligning the implementation with the repository's Catel MVVM and WPF standards.

## Purpose / Big Picture

After this work, the Color feature wizard should still let a user choose a light type, edit a single color, manage discrete color sets, and pick a full-color order. The difference is structural: page actions should be command-driven, the view model should own the interaction surface, and the view should stop coordinating application behavior through code-behind and direct `DataContext` access.

The proof is straightforward. Open the Color feature page for both Tree and PolyLine, switch between light types, launch the picker for single and discrete colors, save a custom set, and confirm the draft, preview, and summary all still behave the same.

## Progress

- [x] (2026-05-24 17:10 -05:00) Reviewed `ColorFeatureWizardPageView.xaml`, `ColorFeatureWizardPageView.xaml.cs`, `ColorFeatureWizardPageViewModel.cs`, and `ColorFeatureWizardPage.cs` against the local `catel-mvvm` and `dotnet-best-practices` skills.
- [x] (2026-05-24 17:10 -05:00) Recorded the current standards gaps and implementation risks for the Color feature wizard.
- [x] (2026-05-24 17:26 -05:00) Implemented Milestone 1 by moving single-color pick, discrete-color edit, add/remove, and custom-set save onto commands exposed by `ColorFeatureWizardPageViewModel`, rebinding the XAML away from button click handlers, and shrinking `ColorFeatureWizardPageView.xaml.cs` to an empty view shell.
- [x] (2026-05-24 17:28 -05:00) Added focused command-surface tests and validated Milestone 1 with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorFeatureWizardPageTests|FullyQualifiedName~ColorFeatureWizardPageViewModelTests|FullyQualifiedName~ColorPickerDialogViewModelTests"` and `dotnet build Props.Runtime/Props.Runtime.csproj`.
- [ ] Narrow the code-behind to view-specific concerns only, or remove it entirely if no justified visual-only logic remains.
- [ ] Validate with focused tests, full tests, build, and manual harness exercise.

## Surprises & Discoveries

- Observation: the view still relies on code-behind for all feature actions.
  Evidence: `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml.cs` handles single-color picking, discrete-color editing, add/remove operations, and custom-set saving.

- Observation: the view code-behind is directly instantiating dialogs and view models instead of routing UI behavior through injected services.
  Evidence: `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml.cs:22` and `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml.cs:42` create `ColorPickerDialogView` and `ColorPickerDialogViewModel` directly and call `ShowDialog()`.

- Observation: the current implementation uses `MessageBox.Show(...)` from the view layer for feature workflow errors instead of a Catel dialog or message service.
  Evidence: `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml.cs:74` catches exceptions from `SaveCustomDiscreteColorSet()` and shows a WPF `MessageBox`.

- Observation: the view model still exposes the underlying page object directly.
  Evidence: `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs:25` exposes `public ColorFeatureWizardPage Page => WizardPage;`, and the current view code-behind drives behavior through that page instance.

- Observation: the view model does not yet expose commands for the operations the XAML needs.
  Evidence: `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs` surfaces state properties but no `Command` / `TaskCommand` members for picking colors, adding/removing colors, or saving custom sets.

- Observation: the page type still contains UI-editing state and editor item types in addition to draft synchronization logic.
  Evidence: `Props.Runtime/Wizards/Features/Color/Pages/ColorFeatureWizardPage.cs` owns `SelectedWorkingDiscreteColor`, `NewDiscreteColorSetName`, `WorkingDiscreteColors`, and also defines `EditableDiscreteColorItem` in the same file.

- Observation: the XAML still uses plain `{Binding ...}` against the ambient `DataContext` instead of the stronger Catel ancestor binding pattern preferred by the local skill.
  Evidence: `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml` binds directly to properties such as `LightType`, `WorkingDiscreteColors`, and `SelectedFullColorOrder`.

- Observation: the page actions could move to commands without changing the underlying wizard-page draft synchronization model.
  Evidence: `ColorFeatureWizardPageViewModel` now wraps the existing page operations in explicit commands, and the focused page plus view-model tests continued to pass after the XAML switched off the old click handlers.

- Observation: a thin interaction service was enough to remove the old button-click orchestration from the view immediately.
  Evidence: single-color picking, discrete-color editing, and save-warning presentation now route through `IColorFeatureWizardInteractionService` instead of `ColorFeatureWizardPageView.xaml.cs`.

- Observation: the warning path should use Catel `IMessageService` directly rather than extending the custom picker interaction abstraction.
  Evidence: after review, the save-warning flow was corrected so `ColorFeatureWizardPageViewModel` calls `IMessageService.ShowWarningAsync(...)`, while `IColorFeatureWizardInteractionService` remains limited to typed color-picking interaction.

## Decision Log

- Decision: preserve the current user-facing flow and page layout while refactoring the interaction plumbing.
  Rationale: the current issue is standards alignment and maintainability, not a feature redesign.
  Date/Author: 2026-05-24 / Codex

- Decision: move picker launches, add/remove actions, and custom-set save behavior onto the view model via commands and injected services.
  Rationale: this is the clearest way to remove application behavior from code-behind and align with Catel MVVM.
  Date/Author: 2026-05-24 / Codex

- Decision: keep the Orc wizard page as the draft-synchronization owner for now, but shrink the view model's dependency on the page to a narrower adapter surface.
  Rationale: the page already owns the wizard integration and draft state. A full page/domain split would be larger than necessary for this standards pass.
  Date/Author: 2026-05-24 / Codex

- Decision: treat `EditableDiscreteColorItem` extraction as part of the cleanup if the refactor touches it.
  Rationale: the local .NET standards prefer one type per file, and the current nested-in-file helper is part of the UI-editing surface being refactored anyway.
  Date/Author: 2026-05-24 / Codex

## Outcomes & Retrospective

Review is complete. The Color feature wizard has the same broad standards problems the picker had before its refactor: too much view code-behind, direct dialog construction, direct `DataContext` coordination, and a view model that does not yet own the command surface required by the XAML.

The most important implementation constraint is that this page sits on top of an Orc wizard page that already owns draft synchronization and preview updates. The refactor should improve MVVM boundaries without destabilizing that existing wizard-page responsibility split.

Milestone 1 is now complete. The view no longer contains ordinary button-click workflow logic, and the page actions now flow through explicit commands on `ColorFeatureWizardPageViewModel`. The underlying page object still owns draft synchronization, but the view no longer coordinates those operations directly.

This first slice also introduced a narrow interaction-service boundary earlier than originally planned. That reduced the amount of temporary rework because the picker and warning interactions could move off the view at the same time as the command migration.

Follow-up review corrected one detail in that slice: warning presentation now uses injected Catel `IMessageService` instead of a custom warning method on the picker interaction service. The custom service remains only for the typed color-picker workflow.

## Context and Orientation

The Color feature wizard currently spans these files:

- `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml`
- `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml.cs`
- `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs`
- `Props.Runtime/Wizards/Features/Color/Pages/ColorFeatureWizardPage.cs`

Today, the page behaves correctly from a feature perspective, but the view still acts as the workflow coordinator. It opens the color picker, mutates page state, and handles error presentation. The view model mostly mirrors state from the page and triggers preview rebuilds. That is the same structural mismatch that previously existed in the color picker before its Catel standards cleanup.

The page itself is also carrying several responsibilities at once: wizard integration, draft synchronization, editor collection state, and reusable inline editor item definitions. This plan does not require a full domain redesign, but it does require a cleaner boundary between page orchestration, view-model interaction, and view-only rendering.

## Plan of Work

Start by moving user actions out of `ColorFeatureWizardPageView.xaml.cs`. Introduce Catel commands on `ColorFeatureWizardPageViewModel` for:

- picking the single color
- editing one discrete color
- adding a discrete color
- removing the selected discrete color
- saving a custom discrete color set

Those commands should own the workflow, and the XAML buttons should bind to them instead of to click handlers.

Next, introduce the service boundary needed for picker launch and error display. The view model should not construct `ColorPickerDialogView` or use `MessageBox`. Use injected Catel UI services or a small repository-local abstraction for modal color picking if the existing services are not a good fit. The critical point is that the view model owns the intent while the concrete WPF dialog remains in the view/service layer.

Then reduce the view model's dependence on exposing the full page object. The view model can keep delegating into the wizard page for draft synchronization, but the public surface used by the view should be explicit properties and commands rather than `Page.*` access from code-behind. If useful, introduce small page-facing methods on the view model that wrap the underlying page operations and keep preview rebuild behavior centralized.

After that, clean up the remaining structure around the discrete color editor surface. If `EditableDiscreteColorItem` remains part of the public editing model, move it to its own file. Review whether `WorkingDiscreteColors`, selection state, and custom-set naming are better surfaced directly from the view model while the page remains responsible for applying changes back to the draft.

Finally, tighten the XAML bindings to match the local Catel guidance where practical. If the repository's Catel views consistently use ancestor-based `ViewModel` bindings, bring this page in line. If not, keep the simpler bindings only where they are already an established local pattern and document that decision.

## Milestones

### Milestone 1: Move Page Actions To Commands

At the end of this milestone, the page's buttons should no longer require ordinary click handlers. Single-color pick, discrete-color edit, add/remove, and custom-set save should all flow through commands on `ColorFeatureWizardPageViewModel`.

The proof is a focused test run plus a manual page check confirming the same user actions still work.

### Milestone 2: Move Dialog And Error Orchestration Behind Services

At the end of this milestone, the view model should no longer depend on direct WPF dialog construction or `MessageBox` usage through the view code-behind.

The proof is that picker launch and custom-set validation messages still work, but the logic now routes through injected services or an explicit modal abstraction.

### Milestone 3: Narrow The View/Page Boundary

At the end of this milestone, the view should no longer need access to `Page` for normal interactions, and the view model should expose the explicit interaction surface the XAML needs.

The proof is that `ColorFeatureWizardPageView.xaml.cs` is empty or limited to a truly view-specific concern, and the XAML binds only to explicit view-model state and commands.

### Milestone 4: Finish Structural Cleanup And Validation

At the end of this milestone, the remaining editing helper types and bindings should be aligned with the local standards as far as practical without destabilizing the wizard page behavior.

The proof is passing focused tests, full `PropCentric.Tests`, a successful solution build, and a manual harness check across Tree and PolyLine color flows.

## Concrete Steps

Run these commands from `C:\Dev\PropCentric`.

1. Add or update focused tests around the Color feature view model command surface and page-state transitions:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorFeatureWizardPageTests|FullyQualifiedName~ColorFeatureWizardPageViewModelTests"

   Expect focused tests to protect mode switching, single-color updates, discrete-color edits, and custom-set save behavior as the command surface moves.

2. After moving the button actions to commands and introducing dialog/message abstractions, rerun the focused tests:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorFeatureWizardPageTests|FullyQualifiedName~ColorFeatureWizardPageViewModelTests|FullyQualifiedName~ColorPickerDialogViewModelTests"

   Expect the focused tests to keep passing, with new tests added for command behavior and any service-driven branching.

3. Before closing the standards work, run broader validation:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj
       dotnet build PropCentric.sln

   Expect both commands to pass once no external process is locking build outputs.

4. Perform a manual harness check:

       dotnet run --project PropCentric/PropCentric.csproj

   Open both Tree and PolyLine setup flows, exercise all three light types, launch the picker from single and discrete-color editing, save a custom set, and confirm preview plus summary behavior remain correct.

## Validation and Acceptance

The work is acceptable only when all of the following are true:

1. `ColorFeatureWizardPageView.xaml.cs` no longer contains ordinary button-click workflow logic.
2. The Color feature view model exposes Catel commands for the page's user actions.
3. The workflow no longer depends on direct `ColorPickerDialogView` construction from the view code-behind.
4. The workflow no longer depends on `MessageBox.Show(...)` for custom-set save validation.
5. The view does not need to reach through `Page` for normal interactions.
6. Single-color editing still updates the preview and summary correctly.
7. Discrete-color add/edit/remove and custom-set save still update the working set, preview, and summary correctly.
8. Full-color order selection still updates the draft and summary correctly.
9. Focused tests pass, `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` passes, and `dotnet build PropCentric.sln` passes.
10. Manual harness validation passes for both Tree and PolyLine flows.

## Idempotence and Recovery

Do this refactor in small slices. The page sits in the middle of wizard draft synchronization and preview rebuilding, so large edits will make regressions harder to isolate.

If the service-based picker launch causes friction with the current Catel/Orc wiring, keep the command refactor first and introduce the service abstraction in a follow-up slice rather than mixing both concerns into one large patch. If moving too much state out of `ColorFeatureWizardPage` destabilizes preview updates, preserve the page as the state owner and narrow only the public interaction surface exposed through the view model.

## Artifacts and Notes

The current standards review can be summarized as follows:

- `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml.cs` contains most of the page workflow logic.
- `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml.cs` constructs `ColorPickerDialogView` and `ColorPickerDialogViewModel` directly and calls `ShowDialog()`.
- `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml.cs` uses `MessageBox.Show(...)` for validation failures.
- `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs` exposes page state but not the command surface needed to remove code-behind.
- `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs` exposes `Page`, which is a smell in the current structure because it enables the view to bypass the view-model surface.
- `Props.Runtime/Wizards/Features/Color/Pages/ColorFeatureWizardPage.cs` still mixes wizard-page orchestration with inline editor-model definitions.

Expected post-refactor command usage should look conceptually like this:

    <Button Content="Pick Color"
            Command="{Binding PickSingleColorCommand}" />

    <Button Content="Add Color"
            Command="{Binding AddWorkingDiscreteColorCommand}" />

Expected post-refactor service-driven workflow should look conceptually like this:

    var selectedColor = await _colorPickerService.PickColorAsync(currentColor);
    if (selectedColor is { } color)
    {
        _page.SetSingleColor(color);
    }

## Interfaces and Dependencies

This standards pass will likely need one new abstraction for modal color picking if the current Catel services are too generic for passing a `System.Drawing.Color` in and getting an optional `System.Drawing.Color` back. Keep that abstraction narrow and local to the wizard/UI layer.

Avoid redesigning `LightColorConfiguration`, the catalog interfaces, or the preview session. Those are not the problem here. The target is the page/view/view-model boundary and Catel alignment.

Revision note: created this ExecPlan on 2026-05-24 after reviewing the Color feature wizard against the local `catel-mvvm` and `dotnet-best-practices` skills.
