using Props.Abstractions.Features;

namespace Props.Abstractions.Props;

public interface IPropCatalogProvider
{
    IEnumerable<IPropCatalogItem> GetPropCatalog();
    IEnumerable<IPropCatalogItem> GetPropCatalogByFeature(PropFeatureFlags flags);
}
