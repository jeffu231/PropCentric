using System.Windows;
using Props.Runtime.Wizards.Features.Color.ViewModels;
using Props.Runtime.Wizards.Features.Color.Views;
using DrawingColor = System.Drawing.Color;

namespace Props.Runtime.Wizards.Features.Color.Services;

/// <summary>
/// WPF-backed interaction service for the Color feature wizard page.
/// </summary>
internal sealed class ColorFeatureWizardInteractionService : IColorFeatureWizardInteractionService
{
    public DrawingColor? PickColor(DrawingColor initialColor)
    {
        var picker = new ColorPickerDialogView
        {
            Owner = GetActiveWindow(),
            DataContext = new ColorPickerDialogViewModel(initialColor)
        };

        return picker.ShowDialog() == true && picker.DataContext is ColorPickerDialogViewModel pickerViewModel
            ? pickerViewModel.SelectedColor
            : null;
    }

    private static Window? GetActiveWindow()
    {
        return Application.Current?.Windows
            .OfType<Window>()
            .FirstOrDefault(window => window.IsActive)
            ?? Application.Current?.MainWindow;
    }
}
