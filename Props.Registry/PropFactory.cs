using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public class PropFactory : IPropFactory
{
    private readonly IServiceProvider _services;
    private readonly IPropRegistry _registry;

    public PropFactory(IServiceProvider services, IPropRegistry registry)
    {
        _services = services;
        _registry = registry;
    }

    public Prop Create(Guid id)
    {
        var d = _registry.GetDescriptor(id);
        var prop = (Prop)_services.GetRequiredService(d.PropType);
        return prop;
    }

    public TProp Create<TProp>() where TProp : Prop
    {
        return _services.GetRequiredService<TProp>();
    }

    public IEnumerable<IPropCatalogItem> GetPropCatalog()
    {
        foreach (var d in _registry.GetAllDescriptors())
        {
            yield return CreatePropCatalogItem(d);
        }
    }

    public IEnumerable<IPropCatalogItem> GetPropCatalogByFeature(PropFeatureFlags flags)
    {
        return _registry
            .GetDescriptorsByFeature(flags)
            .Select(CreatePropCatalogItem);
    }
    
    private static PropCatalogItem CreatePropCatalogItem(PropDescriptor d)
    {
        return new PropCatalogItem
        {
            Id = d.Id,
            Name = d.Name,
            Icon = d.Icon,
            Features = d.Flags,
            WizardType = d.WizardType,
            PropType = d.PropType
        };
    }
}