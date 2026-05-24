using System.Drawing;
using Props.Abstractions.Features;

namespace Props.Registry;

/// <summary>
/// In-memory catalog used by the POC to surface predefined and custom color choices.
/// </summary>
public sealed class InMemoryColorConfigurationCatalog : IColorConfigurationCatalog
{
    private static readonly IReadOnlyList<DiscreteColorSetDefinition> PredefinedDiscreteColorSets =
    [
        CreateDiscreteColorSet("RGB"),
        CreateDiscreteColorSet("RGBW"),
        CreateDiscreteColorSet("GRBW")
    ];

    private static readonly IReadOnlyList<FullColorOrderDefinition> PredefinedFullColorOrders =
    [
        CreateFullColorOrder("RGB"),
        CreateFullColorOrder("RBG"),
        CreateFullColorOrder("GBR"),
        CreateFullColorOrder("GRB"),
        CreateFullColorOrder("BRG"),
        CreateFullColorOrder("BGR"),
        CreateFullColorOrder("RGBW"),
        CreateFullColorOrder("GRWB")
    ];

    private readonly List<DiscreteColorSetDefinition> _customDiscreteColorSets = [];
    private readonly Lock _syncRoot = new();

    public IReadOnlyList<DiscreteColorSetDefinition> GetDiscreteColorSets()
    {
        lock (_syncRoot)
        {
            return PredefinedDiscreteColorSets
                .Concat(_customDiscreteColorSets)
                .Select(colorSet => colorSet.DeepClone())
                .ToArray();
        }
    }

    public IReadOnlyList<FullColorOrderDefinition> GetFullColorOrders()
        => PredefinedFullColorOrders
            .Select(order => order.DeepClone())
            .ToArray();

    public void SaveDiscreteColorSet(DiscreteColorSetDefinition colorSet)
    {
        ArgumentNullException.ThrowIfNull(colorSet);

        var validatedColorSet = ValidateAndClone(colorSet);

        lock (_syncRoot)
        {
            if (PredefinedDiscreteColorSets.Concat(_customDiscreteColorSets)
                .Any(existing => string.Equals(existing.Name, validatedColorSet.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"A discrete color set named '{validatedColorSet.Name}' already exists.");
            }

            _customDiscreteColorSets.Add(validatedColorSet);
        }
    }

    private static DiscreteColorSetDefinition ValidateAndClone(DiscreteColorSetDefinition colorSet)
    {
        var normalizedName = colorSet.Name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException("Discrete color set names must be non-empty.", nameof(colorSet));
        }

        if (colorSet.Colors.Count == 0)
        {
            throw new ArgumentException("Discrete color sets must contain at least one color.", nameof(colorSet));
        }

        return new DiscreteColorSetDefinition(normalizedName, colorSet.Colors.ToArray());
    }

    private static DiscreteColorSetDefinition CreateDiscreteColorSet(string presetName)
        => new(presetName, presetName.Select(MapPresetColor).ToArray());

    private static FullColorOrderDefinition CreateFullColorOrder(string presetName)
        => new(presetName, presetName.Select(MapPresetChannel).ToArray());

    private static Color MapPresetColor(char presetChannel) => presetChannel switch
    {
        'R' => Color.Red,
        'G' => Color.Lime,
        'B' => Color.Blue,
        'W' => Color.White,
        _ => throw new InvalidOperationException($"Unsupported preset color channel '{presetChannel}'.")
    };

    private static LightColorChannel MapPresetChannel(char presetChannel) => presetChannel switch
    {
        'R' => LightColorChannel.Red,
        'G' => LightColorChannel.Green,
        'B' => LightColorChannel.Blue,
        'W' => LightColorChannel.White,
        _ => throw new InvalidOperationException($"Unsupported preset color channel '{presetChannel}'.")
    };
}
