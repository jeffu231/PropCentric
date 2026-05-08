using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// Represents a single vertex in a <see cref="PropMesh"/>, carrying position, normal, and UV data.
/// </summary>
public readonly struct MeshVertex
{
    /// <summary>Gets the vertex position in prop-space coordinates.</summary>
    /// <value>A <see cref="Vector3"/> representing the vertex location.</value>
    public Vector3 Position { get; init; }

    /// <summary>Gets the surface normal at this vertex, used for lighting calculations.</summary>
    /// <value>A unit <see cref="Vector3"/> pointing outward from the surface.</value>
    public Vector3 Normal { get; init; }

    /// <summary>Gets the texture coordinate for this vertex.</summary>
    /// <value>A <see cref="System.Numerics.Vector2"/> in the [0, 1] range for U and V axes.</value>
    public Vector2 UV { get; init; }
}
