using Props.Abstractions.Features;

namespace Props.Abstractions.Props;

/// <summary>
/// Provides access to all discovered prop types and supports filtering by feature capability.
/// </summary>
/// <remarks>
/// Use this interface for discovery. To create prop instances, use <see cref="IPropFactory"/> instead.
/// </remarks>
public interface IPropCatalogProvider
{
    /// <summary>Returns all registered prop types as catalog items.</summary>
    /// <returns>An enumerable of <see cref="IPropCatalogItem"/> entries for every discovered prop.</returns>
    IEnumerable<IPropCatalogItem> GetPropCatalog();

    /// <summary>Returns catalog items whose props support all of the specified feature flags.</summary>
    /// <param name="flags">A bitwise combination of the enumeration values that specifies the required features.</param>
    /// <returns>
    /// An enumerable of <see cref="IPropCatalogItem"/> entries for props that satisfy every flag in
    /// <paramref name="flags"/>.
    /// </returns>
    IEnumerable<IPropCatalogItem> GetPropCatalogByFeature(PropFeatureFlags flags);
}
