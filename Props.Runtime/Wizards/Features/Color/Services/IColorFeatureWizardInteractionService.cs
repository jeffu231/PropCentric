using DrawingColor = System.Drawing.Color;

namespace Props.Runtime.Wizards.Features.Color.Services;

/// <summary>
/// Handles modal interactions required by the Color feature wizard page.
/// </summary>
public interface IColorFeatureWizardInteractionService
{
    /// <summary>
    /// Opens the color picker for the specified starting color.
    /// </summary>
    /// <param name="initialColor">The color shown when the picker opens.</param>
    /// <returns>The selected color if the dialog is accepted; otherwise <c>null</c>.</returns>
    DrawingColor? PickColor(DrawingColor initialColor);
}
