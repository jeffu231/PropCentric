using System.Drawing;

namespace Props.Abstractions.Features;

/// <summary>
/// Defines a named set of discrete colors that can be assigned to a prop.
/// </summary>
/// <param name="Name">The display name of the color set.</param>
/// <param name="Colors">The ordered colors that belong to the set.</param>
public sealed record DiscreteColorSetDefinition(string Name, IReadOnlyList<Color> Colors)
{
    /// <summary>
    /// Creates a deep clone of the current definition.
    /// </summary>
    public DiscreteColorSetDefinition DeepClone() => new(Name, Colors.ToArray());
}
