using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Visuals;

/// <summary>
/// Produces an <see cref="IPropVisualModel"/> from a strongly-typed visual input record.
/// </summary>
/// <typeparam name="TVisualInput">
/// The sealed record type whose positional parameters fully describe the geometry inputs.
/// </typeparam>
/// <remarks>
/// Implementations own the geometry algorithm and are the single source of truth for how a prop
/// is rendered. The factory is a pure function — given the same input it always returns an
/// equivalent model. The <see cref="IWizardPreviewCoordinator{TDraft}"/> caches the last input
/// and skips factory calls when the input has not changed.
/// </remarks>
public interface IPropVisualModelFactory<in TVisualInput>
{
    /// <summary>Creates a visual model from the provided geometry input.</summary>
    /// <param name="input">The visual input record that drives geometry generation.</param>
    /// <returns>A new <see cref="IPropVisualModel"/> representing the prop's current geometry.</returns>
    IPropVisualModel Create(TVisualInput input);
}
