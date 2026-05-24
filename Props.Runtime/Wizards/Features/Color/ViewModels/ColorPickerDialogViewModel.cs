using System.Collections.ObjectModel;
using System.Globalization;
using Catel.Data;
using Catel.MVVM;
using Props.Runtime.Wizards.Features.Color.Models;
using DrawingColor = System.Drawing.Color;

namespace Props.Runtime.Wizards.Features.Color.ViewModels;

/// <summary>
/// View model for the reusable color picker dialog.
/// </summary>
public sealed class ColorPickerDialogViewModel : ViewModelBase
{
    private bool _isSynchronizing;
    private DrawingColor _originalColor;
    private DrawingColor _selectedColor;
    private int _red;
    private int _green;
    private int _blue;
    private int _hue;
    private int _saturation;
    private int _value;
    private double _spectrumSelectorX;
    private double _spectrumSelectorY;

    public ColorPickerDialogViewModel() : this(DrawingColor.White)
    {
    }

    public ColorPickerDialogViewModel(DrawingColor initialColor)
    {
        _originalColor = initialColor;
        Presets = new ObservableCollection<ColorPresetOption>
        {
            new("White", DrawingColor.White),
            new("Red", DrawingColor.Red),
            new("Green", DrawingColor.Lime),
            new("Blue", DrawingColor.Blue)
        };
        ConfirmCommand = new TaskCommand(OnConfirmAsync);
        CancelDialogCommand = new TaskCommand(OnCancelAsync);
        SelectPresetCommand = new Command<ColorPresetOption?>(preset =>
        {
            if (preset is not null)
            {
                SelectPreset(preset);
            }
        });

        SelectColor(initialColor);
    }

    public DrawingColor OriginalColor
    {
        get => _originalColor;
        private set
        {
            if (_originalColor == value)
            {
                return;
            }

            _originalColor = value;
            RaisePropertyChanged(nameof(OriginalColor));
            RaisePropertyChanged(nameof(OriginalHex));
        }
    }

    public DrawingColor SelectedColor
    {
        get => _selectedColor;
        private set
        {
            if (_selectedColor == value)
            {
                return;
            }

            _selectedColor = value;
            RaisePropertyChanged(nameof(SelectedColor));
            RaisePropertyChanged(nameof(SelectedHex));
        }
    }

    public int Red
    {
        get => _red;
        set
        {
            var clamped = ClampByte(value);
            if (_red == clamped)
            {
                return;
            }

            _red = clamped;
            RaisePropertyChanged(nameof(Red));
            SyncFromRgb();
        }
    }

    public int Green
    {
        get => _green;
        set
        {
            var clamped = ClampByte(value);
            if (_green == clamped)
            {
                return;
            }

            _green = clamped;
            RaisePropertyChanged(nameof(Green));
            SyncFromRgb();
        }
    }

    public int Blue
    {
        get => _blue;
        set
        {
            var clamped = ClampByte(value);
            if (_blue == clamped)
            {
                return;
            }

            _blue = clamped;
            RaisePropertyChanged(nameof(Blue));
            SyncFromRgb();
        }
    }

    public int Hue
    {
        get => _hue;
        set
        {
            var normalized = NormalizeHue(value);
            if (_hue == normalized)
            {
                return;
            }

            _hue = normalized;
            RaisePropertyChanged(nameof(Hue));
            SyncFromHsv();
        }
    }

    public int Saturation
    {
        get => _saturation;
        set
        {
            var clamped = ClampPercentage(value);
            if (_saturation == clamped)
            {
                return;
            }

            _saturation = clamped;
            RaisePropertyChanged(nameof(Saturation));
            SyncFromHsv();
        }
    }

    public int Value
    {
        get => _value;
        set
        {
            var clamped = ClampPercentage(value);
            if (_value == clamped)
            {
                return;
            }

            _value = clamped;
            RaisePropertyChanged(nameof(Value));
            SyncFromHsv();
        }
    }

    public double SpectrumSelectorX
    {
        get => _spectrumSelectorX;
        private set
        {
            if (Math.Abs(_spectrumSelectorX - value) < 0.001d)
            {
                return;
            }

            _spectrumSelectorX = value;
            RaisePropertyChanged(nameof(SpectrumSelectorX));
        }
    }

    public double SpectrumSelectorY
    {
        get => _spectrumSelectorY;
        private set
        {
            if (Math.Abs(_spectrumSelectorY - value) < 0.001d)
            {
                return;
            }

            _spectrumSelectorY = value;
            RaisePropertyChanged(nameof(SpectrumSelectorY));
        }
    }

    public string OriginalHex => ToHex(OriginalColor);

    public string SelectedHex => ToHex(SelectedColor);

    public ObservableCollection<ColorPresetOption> Presets { get; }

    public TaskCommand ConfirmCommand { get; }

    public TaskCommand CancelDialogCommand { get; }

    public Command<ColorPresetOption?> SelectPresetCommand { get; }

    public void SelectPreset(ColorPresetOption preset)
    {
        ArgumentNullException.ThrowIfNull(preset);
        SelectColor(preset.Color);
    }

    public void SelectColor(DrawingColor color)
    {
        if (_isSynchronizing)
        {
            return;
        }

        Synchronize(() =>
        {
            SelectedColor = color;
            SetRgbValues(color.R, color.G, color.B);

            var (hue, saturation, value) = RgbToHsv(color);
            SetHsvValues(hue, saturation, value);
            UpdateSpectrumSelector();
        });
    }

    public void SelectSpectrumPoint(double x, double y, double width, double height)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        var normalizedX = Math.Clamp(x / width, 0d, 1d);
        var normalizedY = Math.Clamp(y / height, 0d, 1d);

        Synchronize(() =>
        {
            SetHsvValues(
                NormalizeHue((int)Math.Round(normalizedX * 360d)),
                ClampPercentage((int)Math.Round((1d - normalizedY) * 100d)),
                Value);
            var color = HsvToRgb(Hue, Saturation, Value);
            SetRgbValues(color.R, color.G, color.B);
            SelectedColor = color;
            UpdateSpectrumSelector(width, height);
        });
    }

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);

        AddRangeValidation(validationResults, nameof(Red), Red, 0, 255);
        AddRangeValidation(validationResults, nameof(Green), Green, 0, 255);
        AddRangeValidation(validationResults, nameof(Blue), Blue, 0, 255);
        AddRangeValidation(validationResults, nameof(Hue), Hue, 0, 360);
        AddRangeValidation(validationResults, nameof(Saturation), Saturation, 0, 100);
        AddRangeValidation(validationResults, nameof(Value), Value, 0, 100);
    }

    public static (int Hue, int Saturation, int Value) RgbToHsv(DrawingColor color)
    {
        var red = color.R / 255d;
        var green = color.G / 255d;
        var blue = color.B / 255d;

        var max = Math.Max(red, Math.Max(green, blue));
        var min = Math.Min(red, Math.Min(green, blue));
        var delta = max - min;

        double hue;
        if (delta == 0d)
        {
            hue = 0d;
        }
        else if (Math.Abs(max - red) < double.Epsilon)
        {
            hue = 60d * (((green - blue) / delta) % 6d);
        }
        else if (Math.Abs(max - green) < double.Epsilon)
        {
            hue = 60d * (((blue - red) / delta) + 2d);
        }
        else
        {
            hue = 60d * (((red - green) / delta) + 4d);
        }

        if (hue < 0d)
        {
            hue += 360d;
        }

        var saturation = max == 0d ? 0d : delta / max;
        var value = max;

        return ((int)Math.Round(hue), (int)Math.Round(saturation * 100d), (int)Math.Round(value * 100d));
    }

    public static DrawingColor HsvToRgb(int hue, int saturation, int value)
    {
        var normalizedHue = NormalizeHue(hue);
        var normalizedSaturation = ClampPercentage(saturation) / 100d;
        var normalizedValue = ClampPercentage(value) / 100d;

        if (normalizedSaturation == 0d)
        {
            var component = ClampByte((int)Math.Round(normalizedValue * 255d));
            return DrawingColor.FromArgb(component, component, component);
        }

        var chroma = normalizedValue * normalizedSaturation;
        var huePrime = normalizedHue / 60d;
        var secondary = chroma * (1d - Math.Abs((huePrime % 2d) - 1d));
        var match = normalizedValue - chroma;

        var (red, green, blue) = huePrime switch
        {
            >= 0d and < 1d => (chroma, secondary, 0d),
            >= 1d and < 2d => (secondary, chroma, 0d),
            >= 2d and < 3d => (0d, chroma, secondary),
            >= 3d and < 4d => (0d, secondary, chroma),
            >= 4d and < 5d => (secondary, 0d, chroma),
            _ => (chroma, 0d, secondary)
        };

        return DrawingColor.FromArgb(
            ClampByte((int)Math.Round((red + match) * 255d)),
            ClampByte((int)Math.Round((green + match) * 255d)),
            ClampByte((int)Math.Round((blue + match) * 255d)));
    }

    private void SyncFromRgb()
    {
        if (_isSynchronizing)
        {
            return;
        }

        Synchronize(() =>
        {
            var color = DrawingColor.FromArgb(Red, Green, Blue);
            SelectedColor = color;
            var (hue, saturation, value) = RgbToHsv(color);
            SetHsvValues(hue, saturation, value);
            UpdateSpectrumSelector();
        });
    }

    private void SyncFromHsv()
    {
        if (_isSynchronizing)
        {
            return;
        }

        Synchronize(() =>
        {
            var color = HsvToRgb(Hue, Saturation, Value);
            SetRgbValues(color.R, color.G, color.B);
            SelectedColor = color;
            UpdateSpectrumSelector();
        });
    }

    private void SetRgbValues(int red, int green, int blue)
    {
        if (_red != red)
        {
            _red = red;
            RaisePropertyChanged(nameof(Red));
        }

        if (_green != green)
        {
            _green = green;
            RaisePropertyChanged(nameof(Green));
        }

        if (_blue != blue)
        {
            _blue = blue;
            RaisePropertyChanged(nameof(Blue));
        }
    }

    private void SetHsvValues(int hue, int saturation, int value)
    {
        hue = NormalizeHue(hue);
        saturation = ClampPercentage(saturation);
        value = ClampPercentage(value);

        if (_hue != hue)
        {
            _hue = hue;
            RaisePropertyChanged(nameof(Hue));
        }

        if (_saturation != saturation)
        {
            _saturation = saturation;
            RaisePropertyChanged(nameof(Saturation));
        }

        if (_value != value)
        {
            _value = value;
            RaisePropertyChanged(nameof(Value));
        }
    }

    private void UpdateSpectrumSelector(double width = 240d, double height = 240d)
    {
        SpectrumSelectorX = Math.Clamp(Hue / 360d, 0d, 1d) * width;
        SpectrumSelectorY = (1d - Math.Clamp(Saturation / 100d, 0d, 1d)) * height;
    }

    private void Synchronize(Action action)
    {
        _isSynchronizing = true;
        try
        {
            action();
        }
        finally
        {
            _isSynchronizing = false;
        }
    }

    private static void AddRangeValidation(
        ICollection<IFieldValidationResult> validationResults,
        string fieldName,
        int value,
        int minimum,
        int maximum)
    {
        if (value < minimum || value > maximum)
        {
            validationResults.Add(FieldValidationResult.CreateError(
                fieldName,
                $"{fieldName} must be between {minimum} and {maximum}."));
        }
    }

    private static int ClampByte(int value) => Math.Clamp(value, 0, 255);

    private static int ClampPercentage(int value) => Math.Clamp(value, 0, 100);

    private static int NormalizeHue(int value)
    {
        var normalized = value % 360;
        if (normalized < 0)
        {
            normalized += 360;
        }

        return normalized == 360 ? 0 : normalized;
    }

    private static string ToHex(DrawingColor color)
        => string.Create(
            CultureInfo.InvariantCulture,
            $"#{color.R:X2}{color.G:X2}{color.B:X2}");
    protected override Task<bool> SaveAsync()
        => Task.FromResult(true);

    protected override Task<bool> CancelAsync()
        => Task.FromResult(true);

    private Task OnConfirmAsync()
        => SaveViewModelAsync();

    private Task OnCancelAsync()
        => CancelViewModelAsync();
}
