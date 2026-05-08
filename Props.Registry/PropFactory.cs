using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Props;

namespace Props.Registry;

/// <summary>
/// Creates prop instances from the DI container, resolving the concrete type via the registry.
/// </summary>
public class PropFactory : IPropFactory
{
    private readonly IServiceProvider _services;
    private readonly IPropRegistry _registry;

    /// <summary>Initializes a new instance of the <see cref="PropFactory"/> class.</summary>
    /// <param name="services">The DI service provider used to resolve prop instances.</param>
    /// <param name="registry">The registry used to look up prop types by identifier.</param>
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