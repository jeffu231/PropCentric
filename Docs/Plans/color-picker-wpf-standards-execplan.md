# Bring The Color Picker Up To Catel WPF Standards

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

This document must be maintained in accordance with `.agents/PLANS.md`.

This plan focuses on the reusable color picker dialog under `Props.Runtime/Wizards/Features/Color`. It is a standards-alignment refactor rather than a behavioral redesign. The target is to keep the current picker behavior while bringing the implementation into line with the repository's Catel MVVM and WPF guidance.

## Purpose / Big Picture

After this change, the reusable color picker will still let a user pick a color using the spectrum surface, RGB and HSV inputs, quick presets, and old-versus-new previews. The difference is that the implementation will follow the local WPF standards instead of relying on heavy code-behind and WPF-specific state inside the view model. A developer should be able to look at the picker and see the same architectural pattern used elsewhere in the solution: a Catel dialog view, a view model that owns UI-independent state and commands, and a view that stays mostly declarative.

The user-visible proof is modest but important: the picker should behave the same in the harness while the code structure becomes testable and standards-compliant. Open the picker from the Color feature page, click the quick presets, drag around the spectrum, edit RGB and HSV values, and confirm the previews and selected color still update correctly.

## Progress

- [x] (2026-05-24 15:20 -05:00) Read the local `catel-mvvm` and `dotnet-best-practices` skills and reviewed the current color picker view, code-behind, and view model.
- [x] (2026-05-24 15:20 -05:00) Identified the current standards gaps in `ColorPickerDialogView.xaml`, `ColorPickerDialogView.xaml.cs`, and `ColorPickerDialogViewModel.cs`.
- [x] (2026-05-24 16:16 -05:00) Implemented Milestone 1 by converting `ColorPickerDialogView` from a raw WPF `Window` to `catel:Window` while preserving the existing picker behavior and launch path.
- [x] (2026-05-24 16:16 -05:00) Validated Milestone 1 with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorPickerDialogViewModelTests"` and `dotnet build Props.Runtime/Props.Runtime.csproj`.
- [x] (2026-05-24 16:21 -05:00) Implemented Milestone 2 by moving OK, Cancel, and preset selection onto Catel commands in `ColorPickerDialogViewModel` and removing the corresponding button-click handlers from `ColorPickerDialogView.xaml.cs`.
- [x] (2026-05-24 16:21 -05:00) Added focused picker command tests for preset selection and save/cancel command execution, then validated them with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorPickerDialogViewModelTests"` and `dotnet build Props.Runtime/Props.Runtime.csproj`.
- [x] (2026-05-24 16:29 -05:00) Implemented Milestone 3 by removing `Brush`-based preview properties from `ColorPickerDialogViewModel` and updating the view to convert `OriginalColor` and `SelectedColor` into brushes through `DrawingColorToBrushConverter`.
- [x] (2026-05-24 16:29 -05:00) Added a focused preview-surface test and revalidated the picker with `dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorPickerDialogViewModelTests"` and `dotnet build Props.Runtime/Props.Runtime.csproj`.
- [x] (2026-05-24 16:41 -05:00) Reviewed the remaining code-behind and confirmed it is limited to spectrum-specific mouse capture and coordinate translation into `ColorPickerDialogViewModel.SelectSpectrumPoint(...)`.
- [x] (2026-05-24 16:46 -05:00) Reran broader automated validation after the file locks cleared; `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` passed with 86 tests and `dotnet build PropCentric.sln` passed.
- [x] (2026-05-24 16:58 -05:00) Manual harness picker exercise completed. Functional verification confirmed OK and Cancel close behavior, preset selection, spectrum drag, and RGB/HSV editing all still behave correctly from the Color feature page.

## Surprises & Discoveries

- Observation: the current picker view is a raw WPF `Window` rather than a Catel dialog view.
  Evidence: `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml:1` begins with `<Window ...>` even though the local Catel skill says views should inherit from `Catel.Windows.Controls.Window` or `Catel.Windows.Controls.UserControl`.

- Observation: the current picker relies on several code-behind event handlers for non-visual behavior.
  Evidence: `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml.cs` contains button-click handlers for OK, Cancel, and presets plus spectrum interaction handlers that directly read `DataContext` and call view-model methods.

- Observation: the current picker view model exposes WPF brush types, which violates the local UI-agnostic view-model rule.
  Evidence: `Props.Runtime/Wizards/Features/Color/ViewModels/ColorPickerDialogViewModel.cs` returns `System.Windows.Media.Brush` from `OriginalBrush` and `SelectedBrush`.

- Observation: the current picker view model uses hand-written field plus `RaisePropertyChanged` plumbing instead of Catel registered properties.
  Evidence: the view model stores mutable fields such as `_red`, `_green`, `_blue`, `_hue`, `_saturation`, and `_value` and raises property changes manually throughout the file.

- Observation: the earlier picker implementation deliberately moved away from Catel registered properties because of property-bag synchronization failures.
  Evidence: `Docs/Plans/color-feature-execplan.md` records that the first implementation failed during constructor synchronization and was replaced with plain CLR properties.

- Observation: switching the picker shell to `catel:Window` required the code-behind partial class to stop explicitly inheriting from `System.Windows.Window`.
  Evidence: the first Milestone 1 validation failed with `CS0263` until `ColorPickerDialogView.xaml.cs` stopped specifying a conflicting base class.

- Observation: Catel `TaskCommand` does not expose an awaitable `ExecuteAsync` method in this solution's version of Catel.
  Evidence: the first Milestone 2 test attempt failed with `CS1061`, so the command tests were adjusted to call `Execute()` and wait for the resulting `IsSaved` / `IsCanceled` state transition.

- Observation: the picker preview panes only needed color-domain values from the view model; the WPF brush dependency was entirely in the binding surface.
  Evidence: after replacing `OriginalBrush` and `SelectedBrush` with XAML converter usage over `OriginalColor` and `SelectedColor`, the focused picker tests and runtime project build still passed without additional color-logic changes.

- Observation: the remaining `ColorPickerDialogView.xaml.cs` code-behind is now confined to spectrum drag state and mouse-position translation, which matches the exception allowed by the local WPF skills.
  Evidence: `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml.cs` now contains only the spectrum mouse handlers plus `UpdateSpectrumSelection(...)`, and that helper only forwards normalized surface coordinates into `ColorPickerDialogViewModel.SelectSpectrumPoint(...)`.

- Observation: broader validation is currently blocked by external file locks rather than by picker code errors.
  Evidence: `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` failed with `CS2012` on `Vixen.Shim\obj\Debug\net10.0-windows\Vixen.Shim.dll`, and `dotnet build PropCentric.sln` failed with `CS2012` on `Props.WPFCommon\obj\Debug\net10.0-windows\Props.WPFCommon.dll`.

- Observation: once the external locks cleared, the broader automated validation passed without requiring further picker changes.
  Evidence: the rerun of `dotnet test PropCentric.Tests/PropCentric.Tests.csproj` passed with 86 tests, and `dotnet build PropCentric.sln` completed successfully.

- Observation: moving OK and Cancel off the old click handlers exposed a dialog-close gap in the custom Catel commands.
  Evidence: functional testing showed the picker buttons no longer closed the dialog, and the fix was to follow `SaveViewModelAsync()` / `CancelViewModelAsync()` with `CloseViewModelAsync(true|false)` in `ColorPickerDialogViewModel`.

## Decision Log

- Decision: keep the current picker behavior and interaction model while refactoring the structure toward Catel standards.
  Rationale: the picker now behaves correctly from a user perspective. The goal of this plan is standards alignment and maintainability, not a new UX design.
  Date/Author: 2026-05-24 / Codex

- Decision: move button and preset actions to Catel commands, but allow the spectrum drag surface to keep a minimal amount of view-specific code-behind if command-only wiring becomes awkward.
  Rationale: the local WPF skills strongly prefer no code-behind, but they also allow code-behind for view-specific visual setups. Pointer capture and drag tracking on a spectrum canvas are the one area where a thin view adapter may still be justified.
  Date/Author: 2026-05-24 / Codex

- Decision: remove WPF `Brush` dependencies from the view model even if that means introducing simple color-string or color-value properties plus a converter in the view.
  Rationale: this is the clearest current standards violation and it is easy to fix without changing behavior.
  Date/Author: 2026-05-24 / Codex

- Decision: treat the Catel shell conversion as a standalone first slice and keep the existing click-handler behavior intact for now.
  Rationale: this satisfies the dialog-base part of the standards work without mixing in the later command refactor, which keeps Milestone 1 behavior-preserving and easy to validate.
  Date/Author: 2026-05-24 / Codex

- Decision: keep spectrum drag logic in code-behind for now while moving button and preset actions to commands.
  Rationale: the spectrum surface still needs view-specific mouse capture and position tracking, but the ordinary button-driven interactions were straightforward to move into the view model immediately.
  Date/Author: 2026-05-24 / Codex

- Decision: preserve the existing `OriginalColor` and `SelectedColor` properties as the picker preview surface state and let the view convert them to brushes.
  Rationale: those properties already express the domain state the picker owns, so reusing them avoids inventing a new preview DTO while still removing the WPF presentation dependency from the view model.
  Date/Author: 2026-05-24 / Codex

## Outcomes & Retrospective

Milestone 1 is now complete. The picker dialog now uses a Catel window shell instead of a raw WPF `Window`, which aligns the dialog base with the repository's Catel view conventions while keeping the current runtime behavior unchanged.

Focused picker validation passed after the shell conversion. The remaining work is still the command migration, the removal of WPF presentation types from the view model, and the later manual harness validation.

Milestone 2 is now complete. OK, Cancel, and preset selection now flow through Catel commands exposed by `ColorPickerDialogViewModel`, and the corresponding code-behind handlers are gone. The remaining code-behind is narrowed to the spectrum mouse interaction, which is the one part of the dialog that is still view-specific.

Focused command validation passed after the command migration. The remaining work is now the removal of WPF presentation types from the view model, followed by the broader validation pass.

Milestone 3 is now complete. The picker view model no longer exposes WPF `Brush` values, and the preview panes are now driven by `System.Drawing.Color` values converted in XAML. That removes the clearest UI-agnosticity violation while preserving the same visual behavior.

Milestone 4 is structurally complete. The remaining code-behind is limited to spectrum-specific pointer tracking and mouse capture, while button actions and dialog orchestration now live in the view model.

The broader automated validation and the manual harness exercise are now complete.

Functional testing surfaced one follow-up regression after the command migration: the custom OK and Cancel commands updated view-model state but did not explicitly close the Catel dialog. That is now fixed by explicitly closing the view model with the appropriate dialog result after save or cancel succeeds, and the focused picker tests now assert both state transition and closure.

The color picker standards refactor is now complete. The dialog follows the Catel shell pattern, button and preset actions are command-driven, the view model no longer exposes WPF brush types, the remaining code-behind is limited to spectrum-specific pointer handling, and both automated plus manual validation passed.

## Context and Orientation

The color picker is the reusable modal dialog that the Color feature page uses whenever the user edits one concrete `System.Drawing.Color`. It lives in these files:

- `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml`
- `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml.cs`
- `Props.Runtime/Wizards/Features/Color/ViewModels/ColorPickerDialogViewModel.cs`
- `Props.Runtime/Wizards/Features/Color/Models/ColorPresetOption.cs`

In this repository, Catel MVVM means the view model should own the interaction state and commands, while the view should mostly declare bindings and visual structure. A dialog view should normally derive from a Catel dialog base, not a raw `Window`. A view model should avoid WPF-specific types such as `Brush`, `Panel`, or `MessageBox` APIs. Those belong to the view layer.

The picker currently has four behavior groups. First, it tracks one original color and one currently selected color. Second, it synchronizes RGB and HSV numeric fields. Third, it maps pointer interaction on a spectrum canvas into hue and saturation values. Fourth, it supports accepting or canceling the dialog result. The refactor in this plan keeps all four groups but redistributes responsibility more cleanly between the view and the view model.

## Plan of Work

Start with the dialog shell. Change `ColorPickerDialogView.xaml` from a raw `Window` to a Catel `Window`-style dialog view that works naturally with a Catel view model. Keep the same layout and controls, but stop treating the code-behind as the main coordinator for dialog results. If the solution already has a preferred Catel dialog base elsewhere, match that pattern rather than inventing a new one.

Next, refactor `ColorPickerDialogViewModel.cs` so that dialog lifecycle and actions are command-driven. Introduce explicit Catel commands for accepting the dialog, canceling it, and selecting a preset. The OK and Cancel buttons in XAML should bind to those commands instead of using click handlers. If the view model needs to communicate dialog completion, use Catel's normal save or cancel lifecycle rather than manually setting `DialogResult` in the view.

Then remove WPF-specific surface types from the view model. Replace `OriginalBrush` and `SelectedBrush` with UI-agnostic values that the view can convert, such as `System.Drawing.Color`, a small immutable preview model, or hex/color-channel values. Keep `DrawingColorToBrushConverter` or a similar converter in the view layer so WPF brushes are created only in XAML or converters, not in the view model.

After that, revisit the property implementation strategy. The local skills prefer Catel registered properties, but the prior ExecPlan documents a real synchronization bug with Catel's property bag. The safest route is to convert only where it is straightforward and defensible. If a full registered-property migration reintroduces the old synchronization failure, keep plain CLR properties for the tightly coupled numeric color state and explicitly document that exception in the final outcomes. The standards goal is important, but preserving correct picker behavior is more important.

Finally, reduce the code-behind to the truly view-specific interaction that remains. Spectrum dragging and mouse capture may remain in code-behind if needed, but the view should stop reading arbitrary `DataContext` state for button logic. If spectrum interaction stays in code-behind, limit it to translating mouse positions into a view-model method or command parameter and keep all color math in the view model.

## Milestones

### Milestone 1: Move The Picker Dialog Onto Catel Dialog Patterns

At the end of this milestone, the picker will no longer be a raw `Window` with manual OK and Cancel click handling. It will use the repository's Catel dialog pattern so dialog lifecycle is handled consistently with the rest of the WPF codebase.

The proof is a focused build plus a manual picker check. Open the picker, click OK and Cancel, and confirm the dialog still closes correctly and returns the expected result to the caller.

### Milestone 2: Move Button And Preset Actions To Commands

At the end of this milestone, the picker's non-visual actions will be command-driven. Preset selection, accept, and cancel will no longer depend on direct button-click code-behind.

The proof is a focused test run and a manual picker check. Click the preset swatches, then OK and Cancel, and confirm the selected color and dialog result still behave correctly.

### Milestone 3: Remove WPF Types From The ViewModel

At the end of this milestone, the picker view model will no longer expose `Brush` values or any other WPF presentation types. The view will be responsible for converting color values into WPF visuals.

The proof is a focused build and existing picker tests plus any new tests needed to cover the replacement preview state surface.

### Milestone 4: Narrow Code-Behind To Spectrum-Specific View Logic

At the end of this milestone, the only remaining code-behind should be view-specific pointer handling that is hard to express cleanly in commands. All button and dialog orchestration should already be in the view model.

The proof is a code review pass against the local skills plus manual picker interaction to ensure the spectrum still tracks correctly.

## Concrete Steps

Run these commands from `C:\Dev\PropCentric`.

1. Record the current picker baseline before editing:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorPickerDialogViewModelTests"
       dotnet build Props.Runtime/Props.Runtime.csproj

   Expect both commands to pass. This confirms the current picker logic is stable before structural refactoring begins.

2. After moving dialog actions to Catel patterns and commands, rerun the focused picker tests:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorPickerDialogViewModelTests"

   Expect the existing tests to keep passing. Add more focused tests if the command surface introduces new behavior worth locking down.

3. After removing WPF brush types from the view model, rerun the same focused tests plus any new preview-state tests:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj --filter "FullyQualifiedName~ColorPickerDialogViewModelTests|FullyQualifiedName~ColorPicker"

   Expect the new tests to fail before the refactor and pass after it.

4. Before considering the standards work complete, run broader validation:

       dotnet test PropCentric.Tests/PropCentric.Tests.csproj
       dotnet build PropCentric.sln

   Expect the full test suite and solution build to pass once no running harness process is locking the output files.

5. Perform a manual harness check:

       dotnet run --project PropCentric/PropCentric.csproj

   Open the Color feature page, launch the picker, exercise presets, spectrum drag, RGB and HSV edits, and confirm the previews and final selected color still behave the same.

## Validation and Acceptance

Acceptance is behavioral and structural.

The implementation is acceptable only when all of the following are true:

1. The picker view uses an appropriate Catel dialog base instead of a raw WPF `Window`.
2. OK, Cancel, and preset actions no longer require ordinary button-click handlers in code-behind.
3. The picker view model does not expose WPF-specific presentation types such as `System.Windows.Media.Brush`.
4. RGB and HSV synchronization still behaves exactly as the current tests describe.
5. Spectrum selection still updates the selected color correctly.
6. Quick preset selection still updates the selected color correctly.
7. The old-versus-new preview panes still render correctly in the view.
8. Focused picker tests pass, the full `PropCentric.Tests` suite passes, and `dotnet build PropCentric.sln` passes.
9. A manual harness run confirms the picker still works when launched from the Color feature page.

## Idempotence and Recovery

This refactor should be done in small slices because the picker has a lot of tightly coupled state. The safest recovery rule is to preserve behavior after every slice. Do not mix dialog-shell changes, command changes, and color-state rewrites into one large patch without validating in between.

If a Catel-specific dialog change breaks picker launch behavior, temporarily keep the existing launch call site and restore the previous shell while preserving the smaller command or view-model changes. If a registered-property migration reintroduces the old synchronization bug recorded in the earlier ExecPlan, revert just that part and keep the rest of the standards cleanup moving forward.

## Artifacts and Notes

The current review findings can be summarized as follows:

- `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml:1` uses `Window` instead of a Catel dialog base.
- `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml.cs:17` through `Props.Runtime/Wizards/Features/Color/Views/ColorPickerDialogView.xaml.cs:55` contain most of the dialog interaction logic in code-behind.
- `Props.Runtime/Wizards/Features/Color/ViewModels/ColorPickerDialogViewModel.cs:174` and `Props.Runtime/Wizards/Features/Color/ViewModels/ColorPickerDialogViewModel.cs:176` expose WPF `Brush` values from the view model.
- `Props.Runtime/Wizards/Features/Color/ViewModels/ColorPickerDialogViewModel.cs` uses manual fields and property-changed calls throughout rather than Catel registered properties.

Expected post-refactor command usage should look conceptually like this:

    <Button Content="OK" Command="{Binding SaveCommand}" />
    <Button Content="Cancel" Command="{Binding CancelCommand}" />

Expected post-refactor preview binding should look conceptually like this:

    <Rectangle Fill="{Binding SelectedColor, Converter={StaticResource DrawingColorToBrushConverter}}" />

## Interfaces and Dependencies

Reuse the existing `ColorPresetOption` model and the existing color conversion logic in `ColorPickerDialogViewModel`. Do not redesign the RGB/HSV algorithms unless a bug is found.

In `Props.Runtime/Wizards/Features/Color/ViewModels/ColorPickerDialogViewModel.cs`, the final public surface should continue to expose color-domain values such as:

    public System.Drawing.Color OriginalColor { get; }
    public System.Drawing.Color SelectedColor { get; }
    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }
    public int Hue { get; set; }
    public int Saturation { get; set; }
    public int Value { get; set; }
    public ObservableCollection<ColorPresetOption> Presets { get; }

If commands are introduced, they should be Catel `Command` or `TaskCommand` members on the view model rather than code-behind handlers.

Revision note: created this ExecPlan on 2026-05-24 after reviewing the color picker against the local `catel-mvvm` and `dotnet-best-practices` skills. The plan captures the current standards violations and a behavior-preserving path to fix them.
Revision note: updated this ExecPlan on 2026-05-24 after Milestone 1 implementation to record the Catel window-shell conversion, the temporary base-class mismatch discovery, and the focused passing validation.
Revision note: updated this ExecPlan on 2026-05-24 after Milestone 2 implementation to record the command migration, the TaskCommand test nuance, and the narrowed remaining code-behind.
Revision note: updated this ExecPlan on 2026-05-24 after Milestone 3 implementation to record the removal of `Brush` dependencies from the view model and the focused passing validation.
