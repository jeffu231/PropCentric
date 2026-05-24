using DrawingColor = System.Drawing.Color;

namespace Props.Runtime.Wizards.Features.Color.Pages;

/// <summary>
/// Represents one editable discrete color entry in the inline color-set editor.
/// </summary>
public sealed class EditableDiscreteColorItem : Catel.Data.ModelBase
{
    private DrawingColor _color;
    private int _displayIndex;

    public EditableDiscreteColorItem(DrawingColor color, int displayIndex)
    {
        _color = color;
        _displayIndex = displayIndex;
    }

    public int DisplayIndex
    {
        get => _displayIndex;
        set
        {
            if (_displayIndex == value)
            {
                return;
            }

            _displayIndex = value;
            RaisePropertyChanged(nameof(DisplayIndex));
            RaisePropertyChanged(nameof(Label));
        }
    }

    public string Label => $"Color {DisplayIndex}";

    public DrawingColor Color
    {
        get => _color;
        set
        {
            if (_color.ToArgb() == value.ToArgb())
            {
                return;
            }

            _color = value;
            RaisePropertyChanged(nameof(Color));
            RaisePropertyChanged(nameof(Hex));
        }
    }

    public string Hex => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";
}
