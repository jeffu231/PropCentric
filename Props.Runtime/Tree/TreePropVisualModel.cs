using Props.Abstractions;
using Props.Abstractions.PropVisualModels;

namespace Props.Runtime.Tree;

public class TreePropVisualModel:IPropVisualModel
{
    public IReadOnlyList<IVisualElement> Elements { get; init; }
}