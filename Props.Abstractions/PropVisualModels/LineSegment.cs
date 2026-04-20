using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public class LineSegment:IVisualElement
{
    public IReadOnlyList<Vector3> Points { get; init; }
}