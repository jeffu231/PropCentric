using DrawingColor = System.Drawing.Color;

namespace Props.Runtime.Wizards.Features.Color.Models;

/// <summary>
/// Represents a quick-select preset color in the picker UI.
/// </summary>
public sealed record ColorPresetOption(string Name, DrawingColor Color);
