using System.Drawing;
using Props.Runtime.Wizards.Features.Color.Models;
using Props.Runtime.Wizards.Features.Color.ViewModels;

namespace PropCentric.Tests.ColorPicker;

/// <summary>
/// Verifies color picker RGB/HSV synchronization and preset behavior.
/// </summary>
public class ColorPickerDialogViewModelTests
{
    [Fact]
    public void Constructor_SeedsOriginalAndSelectedState()
    {
        var viewModel = new ColorPickerDialogViewModel(Color.DeepPink);

        Assert.Equal(Color.DeepPink, viewModel.OriginalColor);
        Assert.Equal(Color.DeepPink, viewModel.SelectedColor);
        Assert.Equal("#FF1493", viewModel.OriginalHex);
        Assert.Equal("#FF1493", viewModel.SelectedHex);
    }

    [Theory]
    [InlineData(255, 0, 0, 0, 100, 100)]
    [InlineData(0, 255, 0, 120, 100, 100)]
    [InlineData(0, 0, 255, 240, 100, 100)]
    [InlineData(255, 255, 255, 0, 0, 100)]
    public void RgbToHsv_ReturnsExpectedValues(
        int red,
        int green,
        int blue,
        int expectedHue,
        int expectedSaturation,
        int expectedValue)
    {
        var actual = ColorPickerDialogViewModel.RgbToHsv(Color.FromArgb(red, green, blue));

        Assert.Equal(expectedHue, actual.Hue);
        Assert.Equal(expectedSaturation, actual.Saturation);
        Assert.Equal(expectedValue, actual.Value);
    }

    [Theory]
    [InlineData(0, 100, 100, 255, 0, 0)]
    [InlineData(120, 100, 100, 0, 255, 0)]
    [InlineData(240, 100, 100, 0, 0, 255)]
    [InlineData(0, 0, 100, 255, 255, 255)]
    public void HsvToRgb_ReturnsExpectedValues(
        int hue,
        int saturation,
        int value,
        int expectedRed,
        int expectedGreen,
        int expectedBlue)
    {
        var actual = ColorPickerDialogViewModel.HsvToRgb(hue, saturation, value);

        Assert.Equal(expectedRed, actual.R);
        Assert.Equal(expectedGreen, actual.G);
        Assert.Equal(expectedBlue, actual.B);
    }

    [Fact]
    public void ChangingRgb_UpdatesSelectedColorAndHsv()
    {
        var viewModel = new ColorPickerDialogViewModel(Color.Black);

        viewModel.Red = 255;
        viewModel.Green = 0;
        viewModel.Blue = 0;

        Assert.Equal(Color.Red.ToArgb(), viewModel.SelectedColor.ToArgb());
        Assert.Equal(0, viewModel.Hue);
        Assert.Equal(100, viewModel.Saturation);
        Assert.Equal(100, viewModel.Value);
    }

    [Fact]
    public void ChangingHsv_UpdatesSelectedColorAndRgb()
    {
        var viewModel = new ColorPickerDialogViewModel(Color.Black);

        viewModel.Hue = 240;
        viewModel.Saturation = 100;
        viewModel.Value = 100;

        Assert.Equal(Color.Blue.ToArgb(), viewModel.SelectedColor.ToArgb());
        Assert.Equal(0, viewModel.Red);
        Assert.Equal(0, viewModel.Green);
        Assert.Equal(255, viewModel.Blue);
    }

    [Fact]
    public void SelectPreset_UpdatesSelectedColorAndHex()
    {
        var viewModel = new ColorPickerDialogViewModel(Color.White);
        var preset = new ColorPresetOption("Green", Color.Lime);

        viewModel.SelectPreset(preset);

        Assert.Equal(Color.Lime.ToArgb(), viewModel.SelectedColor.ToArgb());
        Assert.Equal("#00FF00", viewModel.SelectedHex);
    }

    [Fact]
    public void SelectSpectrumPoint_UsesHueAndSaturationCoordinates()
    {
        var viewModel = new ColorPickerDialogViewModel(Color.Black)
        {
            Value = 100
        };

        viewModel.SelectSpectrumPoint(120d, 0d, 240d, 240d);

        Assert.Equal(180, viewModel.Hue);
        Assert.Equal(100, viewModel.Saturation);
        Assert.Equal(Color.Cyan.ToArgb(), viewModel.SelectedColor.ToArgb());
    }

    [Fact]
    public void Inputs_AreClampedToSupportedRanges()
    {
        var rgbViewModel = new ColorPickerDialogViewModel(Color.Black);
        rgbViewModel.Red = 999;
        rgbViewModel.Green = -10;
        rgbViewModel.Blue = 42;

        Assert.Equal(255, rgbViewModel.Red);
        Assert.Equal(0, rgbViewModel.Green);
        Assert.Equal(42, rgbViewModel.Blue);

        var hsvViewModel = new ColorPickerDialogViewModel(Color.Black);
        hsvViewModel.Hue = -5;
        hsvViewModel.Saturation = 400;
        hsvViewModel.Value = -25;

        Assert.Equal(355, hsvViewModel.Hue);
        Assert.Equal(100, hsvViewModel.Saturation);
        Assert.Equal(0, hsvViewModel.Value);
    }
}
