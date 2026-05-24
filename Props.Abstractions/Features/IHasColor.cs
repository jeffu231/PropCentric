namespace Props.Abstractions.Features;

/// <summary>
/// Marks a prop as supporting configurable color output.
/// </summary>
[PropFeature(PropFeatureFlags.Color)]
public interface IHasColor
{
    /// <summary>
    /// Gets or sets the current color configuration selected for the prop.
    /// </summary>
    LightColorConfiguration ColorConfiguration { get; set; }
}
