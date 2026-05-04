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
        Console.WriteLine($"Loading Props from directory: {pluginDirectory}");
        var loadResult = AssemblyLoader.LoadAll(pluginDirectory);
       
        if (throwOnAssemblyLoadFailure && loadResult.Failures.Count > 0)
        {
            var files = string.Join(", ", loadResult.Failures.Select(f => f.File));
            throw new InvalidOperationException($"Failed to load plugin assemblies: {files}");
        }
        
        var interestedAssemblies = loadResult.Loaded.Where(x => !string.IsNullOrEmpty(x.FullName)
                                                                && x.FullName.StartsWith("Props")).ToList();
        
        Console.WriteLine("Starting Prop scanner");
        var descriptors = PropScanner.Scan(interestedAssemblies);
        Console.WriteLine($"Found {descriptors.Count} descriptors");
        Console.WriteLine("Starting Wizard Page Scanner");
        var featurePageRegistrations = FeatureWizardPageScanner.Scan(interestedAssemblies);
        Console.WriteLine($"Found {featurePageRegistrations.Count} features");
        // Register descriptors + registry
        services.AddSingleton(descriptors);
        services.AddSingleton<IReadOnlyList<FeatureWizardPageDescriptor>>(featurePageRegistrations);
        services.AddSingleton<IFeatureWizardPageResolver, FeatureWizardPageResolver>();
        services.AddSingleton<PropFeatureInferrer>();
        services.AddSingleton<IPropRegistry, PropRegistry>();
        services.AddSingleton<IPropFeatureResolver, PropFeatureResolver>();
        services.AddSingleton<IPropCatalogProvider, PropCatalogProvider>();
        services.AddSingleton<IPropFactory, PropFactory>();
        services.AddSingleton<IPropSetupFactory, PropSetupFactory>();
        
        Console.WriteLine("Singletons added");
        
        // Register all discovered types into DI
        foreach (var d in descriptors)
        {
            services.AddTransient(d.PropType);
            services.AddTransient(d.WizardType);
        }

        foreach (var reg in featurePageRegistrations)
            services.AddTransient(reg.PageType);
        Console.WriteLine("Completed Prop System Setup");
        return services;
    }
}