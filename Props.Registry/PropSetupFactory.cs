using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;

namespace Props.Registry;

public class PropSetupFactory(IServiceProvider services, IPropRegistry registry) : IPropSetupFactory
{
    public IPropSetup Create(Guid id)
    {
        var descriptor = registry.GetDescriptor(id);
        return (IPropSetup)services.GetRequiredService(descriptor.WizardType);
    }

    public IPropSetup CreateSetup(IPropCatalogItem item)
        => Create(item.Id);

    public IPropSetup CreateSetupFor(IProp prop)
    {
        var descriptor = registry.GetDescriptor(prop);
        return (IPropSetup)services.GetRequiredService(descriptor.WizardType);
    }
}