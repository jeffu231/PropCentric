using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Wizards;

namespace Props.Registry;

public static class PropServiceCollectionExtensions
{
    public static IServiceCollection AddPropSystem(
        this IServiceCollection services,
        string pluginDirectory, string featureDirectory)
    {
        
        var featureAssemblies = AssemblyLoader.LoadAll(featureDirectory);
        PropFeatureRegistry.Initialize(featureAssemblies);
        
        var propAssemblies = AssemblyLoader.LoadAll(pluginDirectory);
        var descriptors = PropScanner.Scan(propAssemblies);

        // Register descriptors + registry
        services.AddSingleton(descriptors);
        services.AddSingleton<IPropRegistry, PropRegistry>();
        services.AddSingleton<IPropFeatureResolver, PropFeatureResolver>();
        
        // Factory is the ONLY public entry point
        services.AddSingleton<IPropFactory, PropFactory>();
        
        // Factories
        services.AddSingleton<IPropFactory, PropFactory>();
        services.AddSingleton<IWizardFactory, WizardFactory>();
        
        // Register all discovered types into DI
        foreach (var d in descriptors)
        {
            services.AddTransient(d.PropType);
            services.AddTransient(d.WizardType);
        }

        return services;
    }
}