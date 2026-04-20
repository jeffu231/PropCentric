using Microsoft.Extensions.DependencyInjection;
using Props.Registry;

namespace PropCentric;

public static class PropSystemBootstrap
{
    public static ServiceProvider Initialize()
    {
        //This bootstraps the DI system.
        var services = new ServiceCollection();
        
        string path = AppContext.BaseDirectory;

        // Register Prop system
        services.AddPropSystem(path, path);

        // Build provider
        var provider = services.BuildServiceProvider();
        
        return provider;
    }
}