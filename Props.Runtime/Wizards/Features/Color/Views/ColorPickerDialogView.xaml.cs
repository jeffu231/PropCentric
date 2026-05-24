using System.Windows;
using System.Windows.Input;
using Props.Runtime.Wizards.Features.Color.ViewModels;

namespace Props.Runtime.Wizards.Features.Color.Views;

/// <summary>
/// Code-behind for the reusable color picker dialog.
/// </summary>
public partial class ColorPickerDialogView
{
    private bool _isDraggingSpectrum;

    public ColorPickerDialogView()
    {
        InitializeComponent();
    }

    private void SpectrumSurface_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDraggingSpectrum = true;
        SpectrumSurface.CaptureMouse();
        UpdateSpectrumSelection(e);
    }

    private void SpectrumSurface_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDraggingSpectrum)
        {
            UpdateSpectrumSelection(e);
        }
    }

    private void SpectrumSurface_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDraggingSpectrum = false;
        SpectrumSurface.ReleaseMouseCapture();
    }

    private void UpdateSpectrumSelection(MouseEventArgs e)
    {
        if (DataContext is not ColorPickerDialogViewModel viewModel)
        {
            return;
        }

        var position = e.GetPosition(SpectrumSurface);
        viewModel.SelectSpectrumPoint(position.X, position.Y, SpectrumSurface.ActualWidth, SpectrumSurface.ActualHeight);
    }
}
