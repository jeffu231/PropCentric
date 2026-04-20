using Props.Abstractions.Features;

namespace Props.Abstractions.Props;

public interface IPropFactory
{
    Prop Create(Guid id);
    TProp Create<TProp>() where TProp : Prop;
    IEnumerable<IPropCatalogItem> GetPropCatalog();
    IEnumerable<IPropCatalogItem> GetPropCatalogByFeature(PropFeatureFlags flags);
}