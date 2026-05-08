namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// A triangle mesh for rendering graphical fixture geometry (moving heads, par cans, etc.).
/// Indices are a triangle list — every 3 consecutive indices form one triangle.
/// </summary>
public sealed class PropMesh : IVisualElement
{
    /// <summary>Gets the vertex buffer for this mesh.</summary>
    /// <value>A read-only list of <see cref="MeshVertex"/> values. The default is an empty list.</value>
    public IReadOnlyList<MeshVertex> Vertices { get; init; } = [];

    /// <summary>Gets the index buffer for this mesh, interpreted as a triangle list.</summary>
    /// <value>
    /// A read-only list of zero-based vertex indices where every three consecutive values form one triangle.
    /// The default is an empty list.
    /// </value>
    public IReadOnlyList<int> Indices { get; init; } = [];
}
