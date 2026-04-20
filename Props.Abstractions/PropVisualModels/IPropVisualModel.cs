namespace Props.Abstractions.PropVisualModels;

public interface IPropVisualModel
{
    public IReadOnlyList<IVisualElement> Elements { get; init; }

    // Feature flags / capabilities outside of the visual model ??
    //public IReadOnlyDictionary<Type, IVisualFeatureData> FeatureData { get; init; }
}