namespace Props.Abstractions.Features;

/// <summary>
/// Specifies the capabilities a prop supports, expressed as a bitwise combination of flags.
/// </summary>
[Flags]
public enum PropFeatureFlags
{
    /// <summary>No features.</summary>
    None = 0,

    /// <summary>The prop contains individually addressable light elements.</summary>
    Lights = 1,

    /// <summary>The prop supports configurable color output.</summary>
    Color = 2,

    /// <summary>The prop is composed of discrete physical segments.</summary>
    Segments = 4,

    /// <summary>The prop represents a moving-head or fixed fixture.</summary>
    Fixture = 8,

    /// <summary>The prop supports brightness and gamma dimming control.</summary>
    Dimming = 16,

    /// <summary>The prop has a configurable orientation in 3-D space.</summary>
    Orientation = 32,

    /// <summary>The prop has a defined facing direction.</summary>
    Face = 64,

    /// <summary>The prop tracks a discrete operational state.</summary>
    State = 128
}