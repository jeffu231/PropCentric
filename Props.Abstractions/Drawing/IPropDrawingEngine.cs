using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Drawing;

/// <summary>
/// Contract for an engine that renders one or more <see cref="IPropVisualModel"/> instances in a single pass.
/// The same engine serves both wizard preview (single temporary model) and world view (all active prop models).
/// </summary>
public interface IPropDrawingEngine
{
    /// <summary>Current set of models the engine will render on the next <see cref="Render"/> call.</summary>
    IReadOnlyList<IPropVisualModel> Models { get; }

    /// <summary>
    /// Replaces the model list. Implementations decide whether to rebind the underlying
    /// render control reference or update an existing render list in place — callers do not care.
    /// </summary>
    void SetModels(IReadOnlyList<IPropVisualModel> models);

    /// <summary>Renders the current model list to the output surface.</summary>
    void Render();
}
