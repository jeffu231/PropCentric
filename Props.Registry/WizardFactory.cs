using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Props;
using Props.Abstractions.Wizards;

namespace Props.Registry;

public class WizardFactory : IWizardFactory
{
    private readonly IServiceProvider _services;
    private readonly IPropCatalogProvider _catalog;

    public WizardFactory(IServiceProvider services, IPropCatalogProvider catalog)
    {
        _services = services;
        _catalog = catalog;
    }

    public IPropSetupWizard Create(Guid id)
    {
        var item = _catalog.GetPropCatalog().FirstOrDefault(x => x.Id == id)
            ?? throw new InvalidOperationException($"No catalog item with id '{id}'.");
        return (IPropSetupWizard)_services.GetRequiredService(item.WizardType);
    }

    public IPropSetupWizard CreateWizard(IPropCatalogItem item)
        => Create(item.Id);

    public IPropSetupWizard CreateWizardFor(IProp prop)
    {
        var type = prop.GetType();
        var item = _catalog.GetPropCatalog().FirstOrDefault(x => x.PropType == type)
            ?? throw new InvalidOperationException($"No catalog item for prop type '{type.FullName}'.");
        return CreateWizard(item);
    }

    public IPropSetupWizard<TProp> CreateWizard<TProp>() where TProp : IProp
    {
        var type = typeof(TProp);
        var item = _catalog.GetPropCatalog().FirstOrDefault(x => x.PropType == type)
            ?? throw new InvalidOperationException($"No catalog item for prop type '{type.FullName}'.");
        return (IPropSetupWizard<TProp>)Create(item.Id);
    }
}