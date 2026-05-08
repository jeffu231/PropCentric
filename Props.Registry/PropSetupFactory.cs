using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;

namespace Props.Registry;

/// <summary>
/// Creates <see cref="IPropSetup"/> instances by resolving the setup type registered in the
/// <see cref="IPropRegistry"/> from the DI container.
/// </summary>
public class PropSetupFactory(IServiceProvider services, IPropRegistry registry) : IPropSetupFactory
{
    public IPropSetup Create(Guid id)
    {
        var descriptor = registry.GetDescriptorById(id);
        return (IPropSetup)services.GetRequiredService(descriptor.SetupType);
    }

    public IPropSetup CreateFromCatalogItem(IPropCatalogItem item)
        => Create(item.Id);
}