using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Wizards;

namespace Props.Registry;

public class WizardFactory: IWizardFactory
{
    private readonly IServiceProvider _services;
    private readonly IPropRegistry _registry;
    
    public WizardFactory(IServiceProvider services, IPropRegistry registry)
    {
        _services = services;
        _registry = registry;
    }
    
    public IPropSetupWizard Create(Guid id)
    {
        var d = _registry.GetDescriptor(id);
        var wizard = (IPropSetupWizard)_services.GetRequiredService(d.WizardType);
        return wizard;
    }

    public IPropSetupWizard CreateWizard(IPropCatalogItem item)
    {
        return Create(item.Id);
    }

    public IPropSetupWizard CreateWizardFor(IProp prop)
    {
        var type = prop.GetType();

        var item = GetPropCatalog().FirstOrDefault(x => x.PropType == type);

        if (item == null)
            throw new InvalidOperationException($"No catalog item for {type}");

        return CreateWizard(item);
    }
    
    public IPropSetupWizard<TProp> CreateWizard<TProp>() where TProp : IProp
    {
        var type = typeof(TProp);

        var item = GetPropCatalog().FirstOrDefault(x => x.PropType == type);
        if (item == null)
            throw new InvalidOperationException($"No catalog item for {type}");
        
        return (IPropSetupWizard<TProp>)Create(item.Id);

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