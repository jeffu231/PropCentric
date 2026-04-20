using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public class PointCloud: IVisualElement
{
    public IReadOnlyList<Vector3> Points { get; init; }

    public float Size { get; init; }
}