namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// A triangle mesh for rendering graphical fixture geometry (moving heads, par cans, etc.).
/// Indices are a triangle list — every 3 consecutive indices form one triangle.
/// </summary>
public sealed class PropMesh : IVisualElement
{
    public IReadOnlyList<MeshVertex> Vertices { get; init; } = [];
    public IReadOnlyList<int> Indices { get; init; } = [];
}
