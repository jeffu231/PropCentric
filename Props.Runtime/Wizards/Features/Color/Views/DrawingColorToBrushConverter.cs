using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DrawingColor = System.Drawing.Color;

namespace Props.Runtime.Wizards.Features.Color.Views;

/// <summary>
/// Converts <see cref="DrawingColor"/> values into WPF brushes for binding.
/// </summary>
public sealed class DrawingColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DrawingColor color)
        {
            return System.Windows.Media.Brushes.Transparent;
        }

        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
