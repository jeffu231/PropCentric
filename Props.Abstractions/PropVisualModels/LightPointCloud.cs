namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// A collection of individually addressable light points scattered in prop-space.
/// Use for pixel trees, LED matrices, star fields — lights that are not arranged
/// along a clear physical path.
/// </summary>
public sealed class LightPointCloud : IVisualElement
{
    /// <summary>Gets the individually addressable light points in this cloud.</summary>
    /// <value>A read-only list of <see cref="LightPoint"/> values. The default is an empty list.</value>
    public IReadOnlyList<LightPoint> Points { get; init; } = [];
}
