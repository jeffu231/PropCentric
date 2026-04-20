namespace Props.Abstractions.Features;

[Flags]
public enum PropFeatureFlags
{
    None = 0,
    Lights = 1,
    Color = 2,
    Segments = 4,
    Fixture = 8,
    Dimming = 16,
    Orientation = 32,
    Face = 64,
    State = 128
}