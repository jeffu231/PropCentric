using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Props.Runtime.Wizards.Features.Color.Models;
using Props.Runtime.Wizards.Features.Color.ViewModels;
using DrawingColor = System.Drawing.Color;
using MediaBrushes = System.Windows.Media.Brushes;

namespace Props.Runtime.Wizards.Features.Color.Views;

/// <summary>
/// Code-behind for the reusable color picker dialog.
/// </summary>
public partial class ColorPickerDialogView : Window
{
    private bool _isDraggingSpectrum;

    public ColorPickerDialogView()
    {
        InitializeComponent();
    }

    private void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void PresetButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is ColorPickerDialogViewModel viewModel &&
            sender is FrameworkElement { Tag: ColorPresetOption preset })
        {
            viewModel.SelectPreset(preset);
        }
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

/// <summary>
/// Converts <see cref="Color"/> values into WPF brushes for binding.
/// </summary>
public sealed class DrawingColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DrawingColor color)
        {
            return MediaBrushes.Transparent;
        }

        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
