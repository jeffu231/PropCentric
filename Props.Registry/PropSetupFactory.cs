using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;

namespace Props.Registry;

public class PropSetupFactory(IServiceProvider services, IPropRegistry registry) : IPropSetupFactory
{
    public IPropSetup Create(Guid id)
    {
        var descriptor = registry.GetDescriptorById(id);
        return (IPropSetup)services.GetRequiredService(descriptor.WizardType);
    }

    public IPropSetup CreateFromCatalogItem(IPropCatalogItem item)
        => Create(item.Id);
}