using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public sealed class PropCatalogProvider : IPropCatalogProvider
{
    private readonly IPropRegistry _registry;

    public PropCatalogProvider(IPropRegistry registry)
    {
        _registry = registry;
    }

    public IEnumerable<IPropCatalogItem> GetPropCatalog()
        => _registry.GetAllDescriptors().Select(ToItem);

    public IEnumerable<IPropCatalogItem> GetPropCatalogByFeature(PropFeatureFlags flags)
        => _registry.GetDescriptorsByFeature(flags).Select(ToItem);

    private static PropCatalogItem ToItem(PropDescriptor d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Icon = d.Icon,
        Features = d.Flags,
        WizardType = d.WizardType,
        PropType = d.PropType
    };
}
