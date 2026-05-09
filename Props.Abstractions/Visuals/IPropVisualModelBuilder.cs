using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Visuals;

/// <summary>
/// Produces an <see cref="IPropVisualModel"/> from a strongly-typed visual input record.
/// </summary>
/// <typeparam name="TVisualInput">
/// The sealed record type whose positional parameters fully describe the geometry inputs.
/// </typeparam>
/// <typeparam name="TVisualModel"></typeparam>
/// <remarks>
/// Implementations own the geometry algorithm and are the single source of truth for how a prop
/// is rendered. The factory is a pure function — given the same input it always returns an
/// equivalent model. The <see cref="IWizardPreviewCoordinator{TDraft}"/> caches the last input
/// and skips factory calls when the input has not changed.
/// </remarks>
public interface IPropVisualModelBuilder<in TVisualInput, out TVisualModel> where TVisualModel : IPropVisualModel
{
    /// <summary>Creates a visual model from the provided geometry input.</summary>
    /// <param name="input">The visual input record that drives geometry generation.</param>
    /// <returns>A new <see cref="IPropVisualModel"/> representing the prop's current geometry.</returns>
    TVisualModel Create(TVisualInput input);
}
