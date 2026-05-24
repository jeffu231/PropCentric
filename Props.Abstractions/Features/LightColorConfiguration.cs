using System.Drawing;

namespace Props.Abstractions.Features;

/// <summary>
/// Captures the color configuration selected for a light-capable prop.
/// </summary>
/// <param name="LightType">The active light color mode.</param>
/// <param name="SingleColor">The selected single color used when <see cref="LightType"/> is <see cref="Features.LightType.SingleColor"/>.</param>
/// <param name="DiscreteColorSet">The selected named discrete color set used when <see cref="LightType"/> is <see cref="Features.LightType.MultipleDiscreteColors"/>.</param>
/// <param name="FullColorOrder">The selected full-color channel order used when <see cref="LightType"/> is <see cref="Features.LightType.FullColor"/>.</param>
public sealed record LightColorConfiguration(
    LightType LightType,
    Color SingleColor,
    DiscreteColorSetDefinition? DiscreteColorSet,
    FullColorOrderDefinition? FullColorOrder)
{
    /// <summary>
    /// Creates the default full-color RGB configuration used by light props in the POC.
    /// </summary>
    public static LightColorConfiguration CreateDefault() => new(
        LightType.FullColor,
        Color.RoyalBlue,
        new DiscreteColorSetDefinition("RGB", [Color.Red, Color.Green, Color.Blue]),
        new FullColorOrderDefinition(
            "RGB",
            [LightColorChannel.Red, LightColorChannel.Green, LightColorChannel.Blue]));

    /// <summary>
    /// Creates a deep clone of the current configuration.
    /// </summary>
    public LightColorConfiguration DeepClone() => new(
        LightType,
        SingleColor,
        DiscreteColorSet?.DeepClone(),
        FullColorOrder?.DeepClone());
}
