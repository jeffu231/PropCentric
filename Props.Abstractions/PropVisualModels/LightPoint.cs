using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public readonly struct LightPoint
{
    public Vector3 Position { get; init; }
    public Guid ElementId { get; init; }
}
