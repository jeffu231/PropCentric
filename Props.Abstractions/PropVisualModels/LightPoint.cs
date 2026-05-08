using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// Represents a single individually addressable light point in prop-space.
/// </summary>
public readonly struct LightPoint
{
    /// <summary>Gets the 3-D position of the light in prop-space coordinates.</summary>
    /// <value>A <see cref="Vector3"/> representing the light's location.</value>
    public Vector3 Position { get; init; }

    /// <summary>Gets the unique identifier of the element node mapped to this light.</summary>
    /// <value>The <see cref="Guid"/> of the corresponding <c>ElementNode</c>.</value>
    public Guid ElementId { get; init; }
}
