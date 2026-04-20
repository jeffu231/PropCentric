using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public class Mesh:IVisualElement
{
    public IReadOnlyList<Vector3> Vertices { get; init; } = [];

    public IReadOnlyList<int> Indices { get; init; } = [];
}