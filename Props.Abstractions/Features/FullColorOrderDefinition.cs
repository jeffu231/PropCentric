namespace Props.Abstractions.Features;

/// <summary>
/// Defines a named ordered set of full-color channels.
/// </summary>
/// <param name="Name">The display name of the channel order.</param>
/// <param name="Channels">The ordered channels that define the layout.</param>
public sealed record FullColorOrderDefinition(string Name, IReadOnlyList<LightColorChannel> Channels)
{
    /// <summary>
    /// Creates a deep clone of the current definition.
    /// </summary>
    public FullColorOrderDefinition DeepClone() => new(Name, Channels.ToArray());
}
