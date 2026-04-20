using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public readonly struct MeshVertex
{
    public Vector3 Position { get; init; }
    public Vector3 Normal { get; init; }
    public Vector2 UV { get; init; }
}
