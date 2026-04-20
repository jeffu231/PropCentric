namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// A collection of individually addressable light points scattered in prop-space.
/// Use for pixel trees, LED matrices, star fields — lights that are not arranged
/// along a clear physical path.
/// </summary>
public sealed class LightPointCloud : IVisualElement
{
    public IReadOnlyList<LightPoint> Points { get; init; } = [];
    public float PointSize { get; init; } = 2f;
}
