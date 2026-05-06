using Microsoft.Extensions.DependencyInjection;
using Props.Registry;
using Props.Runtime;

namespace PropCentric;

public static class PropSystemBootstrap
{
    public static ServiceProvider Initialize()
    {
        var services = new ServiceCollection();

        string path = AppContext.BaseDirectory;

        services.AddPropSystem(path);
        services.AddTreePropServices();

        return services.BuildServiceProvider();
    }
}