using Microsoft.Extensions.DependencyInjection;
using Props.Registry;
using Props.Runtime;

namespace PropCentric;

/// <summary>
/// Configures and builds the DI container for the prop plugin system at application startup.
/// </summary>
public static class PropSystemBootstrap
{
    /// <summary>
    /// Registers all prop system services and tree-prop services, then builds and returns the service provider.
    /// </summary>
    /// <returns>A fully configured <see cref="ServiceProvider"/> ready for resolving prop system services.</returns>
    public static ServiceProvider Initialize()
    {
        var services = new ServiceCollection();

        string path = AppContext.BaseDirectory;

        services.AddPropSystem(path);
        services.AddTreePropServices();

        return services.BuildServiceProvider();
    }
}