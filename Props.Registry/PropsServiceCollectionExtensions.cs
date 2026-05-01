using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;

namespace Props.Registry;

public static class PropServiceCollectionExtensions
{
    public static IServiceCollection AddPropSystem(
        this IServiceCollection services,
        string pluginDirectory,
        bool throwOnAssemblyLoadFailure = false)
    {
        var loadResult = AssemblyLoader.LoadAll(pluginDirectory);
        services.AddSingleton(loadResult);

        if (throwOnAssemblyLoadFailure && loadResult.Failures.Count > 0)
        {
            var files = string.Join(", ", loadResult.Failures.Select(f => f.File));
            throw new InvalidOperationException($"Failed to load plugin assemblies: {files}");
        }

        var descriptors = PropScanner.Scan(loadResult.Loaded);

        // Register descriptors + registry
        services.AddSingleton(descriptors);
        services.AddSingleton<PropFeatureInferrer>();
        services.AddSingleton<IPropRegistry, PropRegistry>();
        services.AddSingleton<IPropFeatureResolver, PropFeatureResolver>();
        services.AddSingleton<IPropCatalogProvider, PropCatalogProvider>();
        services.AddSingleton<IPropFactory, PropFactory>();
        services.AddSingleton<IPropSetupFactory, PropSetupFactory>();
        
        // Register all discovered types into DI
        foreach (var d in descriptors)
        {
            services.AddTransient(d.PropType);
            services.AddTransient(d.WizardType);
        }

        return services;
    }
}