using Microsoft.Extensions.DependencyInjection;
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

    public IProp Create(Guid id)
    {
        var descriptor = _registry.GetDescriptorById(id);
        return (IProp)_services.GetRequiredService(descriptor.PropType);
    }

    public TProp Create<TProp>() where TProp : IProp
    {
        return _services.GetRequiredService<TProp>();
    }
}