# Fix The Color Feature Wizard And Picker UI Regressions

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md`.

This plan builds on the completed color-feature implementation described in `Docs/Plans/color-feature-execplan.md`. That earlier plan introduced the reusable `IHasColor` feature, the color picker, the color catalog, and the draft-backed `ColorFeatureWizardPage`. This plan is the corrective follow-up for the WPF behavior bugs found during functional testing.

## Purpose / Big Picture

After this change, the reusable Color feature page will behave like the other reusable feature pages instead of like a prop page. A user will be able to open the Color page for a Tree or PolyLine prop and see only color-related controls, switch Light Type and see only the controls for the active mode, use the quick-pick swatches in the color picker because they visibly render their colors, and see the single-color preview surface update immediately when a new color is chosen.

The user-visible proof is in the harness flow. Open a Tree or PolyLine prop, navigate to the Color page, and confirm there is no embedded prop viewer, only one mode section is visible at a time, the quick swatches show White/Red/Green/Blue visibly, and the single-color preview box changes to the selected color after accepting the picker dialog.

## Progress

- [x] (2026-05-24 14:55 -05:00) Reviewed `.agents/PLANS.md`, `Docs/color-feature-requirements.md`, and the completed color feature ExecPlan.
- [x] (2026-05-24 14:55 -05:00) Inspected `ColorFeatureWizardPageView`, `ColorFeatureWizardPageViewModel`, `ColorPickerDialogView`, `ColorPickerDialogViewModel`, and the Dimming feature view to map each functional bug to the current implementation.
- [x] (2026-05-24 14:55 -05:00) Clarified `Docs/color-feature-requirements.md` so it explicitly requires a preview-free Color feature page, one visible Light Type editor at a time, visible quick-pick swatches, and a live-updating single-color preview surface.
- [x] (2026-05-24 14:55 -05:00) Implemented Milestone 1 by converting `ColorFeatureWizardPageView` from the preview-hosting `WizardPageViewBase` to a plain Catel user control and removing the embedded OpenTK viewer pane.
- [x] (2026-05-24 15:07 -05:00) Implemented Milestone 2 by surfacing the missing Color page state through `ColorFeatureWizardPageViewModel`, including mode flags, single-color display values, and the remaining bound page properties used by the WPF view.
- [x] (2026-05-24 15:07 -05:00) Added focused `ColorFeatureWizardPageViewModelTests` covering initial state, Light Type switching, and single-color preview updates, then validated them with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorFeatureWizardPageTests|FullyQualifiedName~ColorFeatureWizardPageViewModelTests|FullyQualifiedName~ColorPickerDialogViewModelTests"` and `dotnet build Props.Runtime/Props.Runtime.csproj`.
- [x] (2026-05-24 15:13 -05:00) Implemented Milestone 3 by replacing the picker preset-button content with an explicitly sized colored `Border` so the quick swatches visibly render White, Red, Green, and Blue.
- [x] (2026-05-24 15:13 -05:00) Revalidated the focused color test set with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorPickerDialogViewModelTests|FullyQualifiedName~ColorFeatureWizardPageTests|FullyQualifiedName~ColorFeatureWizardPageViewModelTests"` and confirmed `dotnet build Props.Runtime/Props.Runtime.csproj` still succeeds.
- [ ] Validate the remaining fix with targeted automated tests, a full test run, a solution build, and a manual harness check.

## Surprises & Discoveries

- Observation: the current Color feature view is inheriting from the preview-hosting base view used by prop pages.
  Evidence: `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml` starts with `views:WizardPageViewBase` and declares an OpenTK `GLWpfControl` column.

- Observation: the current Color feature view binds to mode and single-color properties that the view model does not expose.
  Evidence: `ColorFeatureWizardPageView.xaml` binds to `IsSingleColorMode`, `IsMultipleDiscreteColorsMode`, `IsFullColorMode`, `SingleColor`, and `SingleColorHex`, while `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs` currently exposes none of those properties.

- Observation: the picker quick swatches are likely invisible because the content shape inside each button has no explicit size.
  Evidence: `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml` places a `Rectangle` inside the preset button template without a width or height.

- Observation: the reported “single color box is gray” bug is consistent with the same missing view-model surface as the mode-switching bug.
  Evidence: the view binds the preview box background to `SingleColor`, but the view model does not publish a `SingleColor` property or relay its changes.

- Observation: the Milestone 1 XAML-only change can be validated with focused test compilation even while the full solution build is blocked by a running harness process.
  Evidence: `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorFeatureWizardPageTests|FullyQualifiedName~ColorPickerDialogViewModelTests"` passed after the XAML/code-behind refactor, while `dotnet build PropCentric.sln` failed because `PropCentric.exe` was holding files in `PropCentric/bin/Debug/net10.0-windows`.

- Observation: the Color page XAML was depending on more missing view-model properties than the three mode flags alone.
  Evidence: `ColorFeatureWizardPageView.xaml` also binds to `SingleColor`, `SingleColorHex`, `AvailableDiscreteColorSets`, `SelectedWorkingDiscreteColor`, `CanRemoveWorkingDiscreteColor`, `NewDiscreteColorSetName`, and `AvailableFullColorOrders`, none of which were surfaced by the original `ColorFeatureWizardPageViewModel`.

- Observation: the picker swatch bug was isolated to the button content's visual measurement rather than the preset-color data or converter logic.
  Evidence: `ColorPickerDialogViewModelTests` already proved preset selection updated the chosen color correctly; the only UI element in `ColorPickerDialogView.xaml` that could suppress visible color was the unsized `Rectangle` used as button content.

## Decision Log

- Decision: the Color feature page should follow the Dimming feature-page composition pattern and not host a prop preview surface.
  Rationale: functional testing showed that the extra viewer is confusing and violates the intent of a reusable feature page. The preview already belongs to the prop-specific page and wizard flow; duplicating it on the feature page adds UI noise and unwanted OpenTK coupling.
  Date/Author: 2026-05-24 / Codex

- Decision: fix the mode-switching bug by exposing the missing state on `ColorFeatureWizardPageViewModel` instead of binding the view directly to the page object.
  Rationale: the repository already uses Catel view-model mediation for wizard pages. Keeping the view bound through the view model preserves the established pattern and makes UI-state tests straightforward.
  Date/Author: 2026-05-24 / Codex

- Decision: prefer simple, explicit WPF sizing for the quick-pick swatches instead of relying on implicit content measurement.
  Rationale: the bug is visual and deterministic. Explicit sizing is the lowest-risk way to guarantee the colored swatches render consistently across themes and button templates.
  Date/Author: 2026-05-24 / Codex

- Decision: implement the preview-removal slice first without changing the page-domain logic or view model.
  Rationale: the extra viewer is structurally isolated in the Color page XAML and code-behind, so removing it first delivers an immediate user-facing improvement and keeps the first milestone low-risk.
  Date/Author: 2026-05-24 / Codex

- Decision: expose the Color page's bound state through explicit pass-through properties on `ColorFeatureWizardPageViewModel` instead of relying on Catel's `ViewModelToModel` mapping for only a small subset of fields.
  Rationale: the WPF view binds to a mix of editable values, computed booleans, collections, and display-only strings. Explicit pass-through properties made the binding surface complete, predictable, and easy to test.
  Date/Author: 2026-05-24 / Codex

- Decision: replace the picker's unsized `Rectangle` with an explicitly sized `Border` centered inside each preset button.
  Rationale: `Border` gives predictable layout inside a WPF button template and makes the visible swatch size explicit without changing the preset-selection behavior.
  Date/Author: 2026-05-24 / Codex

## Outcomes & Retrospective

Milestone 1 is now complete. The Color feature page no longer derives from the preview-hosting base view and no longer contains an embedded OpenTK viewer pane. This aligns the page composition with the intent of a reusable feature page and removes the most obvious functional mismatch reported during testing.

Focused automated validation for the color feature and picker logic passed after the XAML/code-behind refactor. The full solution build did not complete because a running `PropCentric` process was locking output files, so that broader validation remains pending for a later milestone or after the harness process is closed.

Milestone 2 is now also complete. `ColorFeatureWizardPageViewModel` now surfaces the mode booleans, the single-color preview values, and the remaining page-backed properties that the XAML depends on. Focused tests prove that the initial state, Light Type switching, and single-color preview values now track the page correctly.

Milestone 3 is now also complete. The picker preset buttons now host explicitly sized color swatches, which removes the ambiguous WPF measurement behavior that had been hiding the quick-pick colors during functional testing.

The remaining work is now the final validation pass in the full test suite, solution build, and harness once the running app no longer locks the build outputs.

## Context and Orientation

The current Color feature work lives under `Props.Runtime/Wizards/Features/Color`. The key runtime types are:

- `Props.Runtime/Wizards/Features/Color/Pages/ColorFeatureWizardPage.cs`, which is the draft-backed feature page that owns the color state and summary behavior.
- `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs`, which is the Catel view model used by the WPF view.
- `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml`, which is the WPF visual tree for the feature page.
- `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml` and `Props.Runtime/Wizards/Features/Color/ViewModels/ColorPickerDialogViewModel.cs`, which together implement the reusable modal color picker.
- `Props.Runtime/Wizards/Features/Dimming/Views/DimmingFeatureWizardPageView.xaml`, which is the nearest working example of a simple reusable feature page with no embedded preview surface.

In this repository, a “feature page” means a reusable wizard page that edits one capability shared by more than one prop. The Color page is one of those. A “prop page” means the wizard page that owns prop-specific geometry or setup data such as Tree dimensions or PolyLine capture results. The functional bug here is that the Color feature page is currently shaped too much like a prop page.

Catel is the MVVM framework used in this solution. In practice here, the WPF view binds to a view model, and the view model mirrors or mediates state from the underlying wizard page object. If the view binds to properties the view model does not expose, the UI can appear stuck, blank, or incorrectly visible even when the page object itself is changing correctly.

## Plan of Work

Start by correcting the feature-page composition in `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml`. Replace the `WizardPageViewBase` root with the same plain Catel user-control pattern used by `DimmingFeatureWizardPageView.xaml`. Remove the OpenTK namespaces, preview border, preview context menu, and `GLWpfControl`. Keep only the color controls and the existing WPF resources that are still needed, such as the color-to-brush converter and visibility converter.

Next, repair the view-model surface in `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs`. Add pass-through properties for `IsSingleColorMode`, `IsMultipleDiscreteColorsMode`, `IsFullColorMode`, `SingleColor`, and `SingleColorHex`. Update the page-property-change handler so that when `LightType` changes it raises property-changed notifications for all three mode flags, and when `SingleColor` changes it raises notifications for both `SingleColor` and `SingleColorHex`. Keep the existing preview-rebuild scheduling only if it still has a caller after the view no longer hosts a graphics surface; if it becomes dead weight after the refactor, remove or narrow it rather than leaving misleading preview behavior in place.

Then update `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml` so its bindings rely on the repaired view model surface. The three mode-specific sections can remain as separate XAML sections with `Visibility` bindings, but only the currently active section should be visible. Confirm that the single-color preview border background binds through the corrected `SingleColor` property and that the hex text binds through `SingleColorHex`.

After that, fix the picker swatch rendering in `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml`. Replace the preset button content with an explicitly sized `Border` or a sized `Rectangle` so each quick-pick option visibly renders White, Red, Green, or Blue. Keep the click behavior and preset binding unchanged unless the fix exposes a second issue.

Add focused tests next. The minimum automated coverage should live in `PropCentric.Tests/Color`. Extend or add tests around `ColorFeatureWizardPageViewModel` to prove that changing `LightType` updates the three mode flags correctly and that changing the selected single color updates the exposed `SingleColor` and `SingleColorHex` values. Add a small picker-facing test if needed to prove the preset model or converter still produces the correct colors, but prioritize tests that would have failed on the missing-property bug because that is the most consequential regression.

Finally, validate the end result manually in the harness. Use both Tree and PolyLine because both resolve the Color feature page. Confirm that the page layout is now viewer-free, that the dynamic region switches correctly for all three Light Types, that the quick swatches are visibly colored, and that the single-color preview patch reflects the newly chosen color immediately after accepting the picker.

## Milestones

### Milestone 1: Remove The Accidental Prop Preview From The Color Feature Page

At the end of this milestone, the Color feature page will look and behave like a reusable feature page rather than a prop page. The embedded OpenTK preview surface will be gone, and the page will contain only color-related controls. The work happens primarily in `ColorFeatureWizardPageView.xaml` and should not require changes to the color-domain contracts.

The proof is a manual harness check: open a Tree or PolyLine prop, navigate to the Color page, and confirm there is no viewer pane or preview context menu on that page. A focused build should still succeed because this is largely a XAML composition change.

### Milestone 2: Repair The Light Type Dynamic UI And Single-Color Preview Binding

At the end of this milestone, switching Light Type will show only the active mode’s controls, and the single-color preview box plus hex text will update when the selected color changes. The work happens in `ColorFeatureWizardPageViewModel.cs` and the Color page XAML bindings.

The proof is a focused automated test run for the view model plus a quick harness check. In the harness, switch among Single Color, Multiple Discrete Colors, and Full Color, and confirm that only one section is visible at a time. In Single Color mode, pick a new color and confirm that the preview box and hex text update immediately.

### Milestone 3: Make The Picker Quick Swatches Visibly Render Their Colors

At the end of this milestone, the White/Red/Green/Blue preset buttons in the color picker will show those colors directly. The work is localized to `ColorPickerDialogView.xaml`, with only supporting test adjustments if needed.

The proof is a manual picker check plus any supporting test added in `PropCentric.Tests/Color`. Open the picker, verify the four quick-pick swatches are visibly colored, click each one, and confirm the selected color state updates as before.

### Milestone 4: Lock The Fix In With Tests And Full Validation

At the end of this milestone, the UI-state bugs will be covered by focused automated tests, and the repository will have passed the normal validation commands. This milestone is where the corrective work becomes durable rather than purely manual.

The proof is a targeted color test run, a full `PropCentric.Tests` run, a solution build, and a final harness pass using Tree and PolyLine.

## Concrete Steps

Run these commands from `C:\Dev\PropCentric`.

1. Before editing, run a focused baseline that exercises the current Color page and picker logic:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorFeatureWizardPageTests|FullyQualifiedName~ColorPickerDialogViewModelTests"

   Expect the existing logic-level tests to pass even though the functional UI bugs still exist. This confirms the gap is in the WPF layer and the view-model surface, not in the basic page-domain behavior.

2. After removing the preview host and fixing the view-model properties, run focused color tests again:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorFeatureWizardPageTests|FullyQualifiedName~ColorPickerDialogViewModelTests|FullyQualifiedName~ColorFeatureWizardPageViewModel"

   Expect the new or updated tests for mode switching and single-color preview state to pass.

3. After fixing the picker swatches, run the same focused tests and then the full suite:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj

   Expect all tests to pass. If a new test was added specifically for the Color feature view model, it should fail before the implementation and pass after it.

4. Before considering the bugfix complete, run the solution build:

       dotnet build PropCentric.sln

   Expect a successful build with no new warnings introduced by the fix.

5. Perform the manual harness check:

       dotnet run --project PropCentric/PropCentric.csproj

   In the harness, open both a Tree prop and a PolyLine prop. Navigate to the Color page. Confirm there is no embedded preview viewer, switch through all three Light Types and confirm only one mode section is visible at a time, open the picker from Single Color mode and confirm the quick swatches are visibly colored, then choose a new single color and confirm the preview patch and hex text update when the dialog closes.

## Validation and Acceptance

Acceptance is behavioral, not just structural.

The implementation is acceptable only when all of the following are true:

1. The Color feature page no longer hosts the OpenTK preview viewer or any viewer-only context menu.
2. The Color feature page behaves like the Dimming feature page in overall composition: it is a reusable feature editor surface, not a prop-preview surface.
3. When `LightType.SingleColor` is selected, only the single-color editor section is visible.
4. When `LightType.MultipleDiscreteColors` is selected, only the inline discrete-color editor section is visible.
5. When `LightType.FullColor` is selected, only the full-color order section is visible.
6. The quick-pick swatches in the picker visibly render White, Red, Green, and Blue.
7. After accepting a new single color from the picker, the Color feature page preview patch and hex text reflect that new color immediately.
8. Focused Color feature tests pass, the full `PropCentric.Tests` suite passes, and `dotnet build PropCentric.sln` passes.
9. A manual harness check with both Tree and PolyLine confirms the corrected WPF behavior.

## Idempotence and Recovery

These changes are UI-focused and should be safe to apply incrementally. The safest recovery strategy is to keep each slice coherent: do not remove the old preview-hosting view root without also keeping the XAML valid, and do not add view bindings to new properties without adding the corresponding view-model properties in the same slice.

If a test or harness run shows that the page stops loading entirely, first check for XAML binding and root-element mismatches before changing the page-domain logic. The color-domain logic is already covered by passing tests from the earlier color feature plan, so the recovery bias here should be toward local WPF fixes rather than broad refactors.

If the picker swatches still do not render after sizing changes, retry by replacing the button content with a `Border` instead of a `Shape`, because WPF button template measurement is more predictable with a standard framework element that owns its own size.

## Artifacts and Notes

The most important current file relationships are:

- `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml` currently contains the accidental viewer pane and the visibility bindings.
- `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs` currently schedules preview rebuilds but does not expose the mode flags or single-color display properties the view needs.
- `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml` currently contains the preset swatch row whose content does not visibly render as expected.
- `Props.Runtime/Wizards/Features/Dimming/Views/DimmingFeatureWizardPageView.xaml` is the comparison point for how simple a reusable feature-page view should be.

Expected view-model assertions after the fix should look conceptually like this:

    page.LightType = LightType.FullColor;
    Assert.False(viewModel.IsSingleColorMode);
    Assert.False(viewModel.IsMultipleDiscreteColorsMode);
    Assert.True(viewModel.IsFullColorMode);

Expected single-color preview assertions should look conceptually like this:

    page.SetSingleColor(System.Drawing.Color.Cyan);
    Assert.Equal(System.Drawing.Color.Cyan.ToArgb(), viewModel.SingleColor.ToArgb());
    Assert.Equal("#00FFFF", viewModel.SingleColorHex);

## Interfaces and Dependencies

No new domain contracts are needed for this fix. Reuse the existing `ColorFeatureWizardPage`, `ColorFeatureWizardPageViewModel`, `ColorPickerDialogViewModel`, and `DrawingColorToBrushConverter` types.

In `Props.Runtime/Wizards/Features/Color/ViewModels/ColorFeatureWizardPageViewModel.cs`, ensure the final type exposes at least these public properties for WPF binding:

    public LightType LightType { get; set; }
    public bool IsSingleColorMode { get; }
    public bool IsMultipleDiscreteColorsMode { get; }
    public bool IsFullColorMode { get; }
    public System.Drawing.Color SingleColor { get; }
    public string SingleColorHex { get; }

In `Props.Runtime/Wizards/Features/Color/Views/ColorFeatureWizardPageView.xaml`, the root should be a non-preview Catel user control comparable to `DimmingFeatureWizardPageView.xaml`, not `WizardPageViewBase`.

In `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml`, the preset swatch content must be an explicitly visible colored element, and the old/new preview panes must continue to bind to `OriginalBrush`, `OriginalHex`, `SelectedBrush`, and `SelectedHex`.

Revision note: created this ExecPlan on 2026-05-24 after functional testing found four WPF behavior bugs in the completed Color feature implementation. The plan captures the current root-cause analysis and adds the missing requirements clarifications so the corrective work is unambiguous.
Revision note: updated this ExecPlan on 2026-05-24 after Milestone 1 implementation to record the preview-removal change, the focused passing test run, and the blocked full build caused by a running `PropCentric` process.
Revision note: updated this ExecPlan on 2026-05-24 after Milestone 2 implementation to record the completed view-model binding repair, the new focused tests, and the narrowed remaining work.
Revision note: updated this ExecPlan on 2026-05-24 after Milestone 3 implementation to record the quick-swatch rendering fix and the refreshed focused validation results.
