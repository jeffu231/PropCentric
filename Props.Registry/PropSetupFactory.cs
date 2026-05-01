using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;

namespace Props.Registry;

public class PropSetupFactory : IPropSetupFactory
{
    private readonly IServiceProvider _services;
    private readonly IPropCatalogProvider _catalog;

    public PropSetupFactory(IServiceProvider services, IPropCatalogProvider catalog)
    {
        _services = services;
        _catalog = catalog;
    }

    public IPropSetup Create(Guid id)
    {
        var item = _catalog.GetPropCatalog().FirstOrDefault(x => x.Id == id)
            ?? throw new InvalidOperationException($"No catalog item with id '{id}'.");
        return (IPropSetup)_services.GetRequiredService(item.WizardType);
    }

    public IPropSetup CreateSetup(IPropCatalogItem item)
        => Create(item.Id);

    public IPropSetup CreateSetupFor(IProp prop)
    {
        var type = prop.GetType();
        var item = _catalog.GetPropCatalog().FirstOrDefault(x => x.PropType == type)
            ?? throw new InvalidOperationException($"No catalog item for prop type '{type.FullName}'.");
        return CreateSetup(item);
    }

    public IPropSetup<TProp> CreateSetup<TProp>() where TProp : IProp
    {
        var type = typeof(TProp);
        var item = _catalog.GetPropCatalog().FirstOrDefault(x => x.PropType == type)
            ?? throw new InvalidOperationException($"No catalog item for prop type '{type.FullName}'.");
        return (IPropSetup<TProp>)Create(item.Id);
    }
}