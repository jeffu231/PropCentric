using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup;

namespace Props.Abstractions.Visuals;

/// <summary>
/// Coordinates incremental visual model rebuilds during wizard editing.
/// </summary>
/// <typeparam name="TDraft">The draft type that provides the wizard's current field values.</typeparam>
/// <remarks>
/// Maps the draft to a <c>TVisualInput</c> record, compares it to the previously cached input
/// using structural equality, and only invokes the <see cref="IPropVisualModelFactory{TVisualInput}"/>
/// when the input has changed. This avoids expensive geometry recalculation on unrelated property changes.
/// </remarks>
public interface IWizardPreviewCoordinator<in TDraft>
    where TDraft : class, IPropDraft
{
    /// <summary>
    /// Returns a visual model for the current draft, rebuilding geometry only when the input has changed.
    /// </summary>
    /// <param name="draft">The current wizard draft whose fields drive geometry generation.</param>
    /// <returns>
    /// A cached <see cref="IPropVisualModel"/> if the input is unchanged, or a freshly built model
    /// if any geometry-relevant field has changed since the last call.
    /// </returns>
    IPropVisualModel BuildPreview(TDraft draft);
}
