namespace Props.Abstractions.Features;

/// <summary>
/// Provides predefined and user-defined color configuration choices for wizard flows.
/// </summary>
public interface IColorConfigurationCatalog
{
    /// <summary>
    /// Returns all available discrete color sets, including predefined and custom entries.
    /// </summary>
    IReadOnlyList<DiscreteColorSetDefinition> GetDiscreteColorSets();

    /// <summary>
    /// Returns the predefined full-color channel orders available to the user.
    /// </summary>
    IReadOnlyList<FullColorOrderDefinition> GetFullColorOrders();

    /// <summary>
    /// Saves a new custom discrete color set and makes it available to future queries.
    /// </summary>
    /// <param name="colorSet">The color set to save.</param>
    void SaveDiscreteColorSet(DiscreteColorSetDefinition colorSet);
}
