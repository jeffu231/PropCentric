using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public class PropVisualModel : IPropVisualModel
{
    public IReadOnlyList<IVisualElement> Elements { get; init; } = [];
    public Vector3? ReferencePoint { get; init; }
}
