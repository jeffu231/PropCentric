namespace Props.Abstractions.PropVisualModels;

public class PropVisualModel:IPropVisualModel
{
    public IReadOnlyList<IVisualElement> Elements { get; init; }
}