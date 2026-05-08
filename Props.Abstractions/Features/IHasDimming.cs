namespace Props.Abstractions.Features;

/// <summary>
/// Marks a prop as supporting brightness and gamma dimming control.
/// </summary>
[PropFeature(PropFeatureFlags.Dimming)]
public interface IHasDimming
{
    /// <summary>Gets or sets the maximum brightness level as a percentage.</summary>
    /// <value>A percentage value between 0 and 100 inclusive.</value>
    double Brightness { get; set; }

    /// <summary>Gets or sets the gamma correction factor applied to the light output.</summary>
    /// <value>A positive non-zero value where <c>1.0</c> represents no correction.</value>
    double Gamma { get; set; }
}