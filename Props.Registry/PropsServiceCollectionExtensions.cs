using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;

namespace Props.Registry;

/// <summary>
/// Provides the <see cref="AddPropSystem"/> extension method for bootstrapping the prop plugin system
/// into an <see cref="IServiceCollection"/>.
/// </summary>
public static class PropServiceCollectionExtensions
{
    /// <summary>
    /// Scans the plugin directory, discovers all prop and feature wizard page types, and registers
    /// the prop system services into the DI container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="pluginDirectory">The path to the directory containing plugin DLLs.</param>
    /// <param name="throwOnAssemblyLoadFailure">
    /// <see langword="true"/> to throw if any plugin assembly fails to load;
    /// <see langword="false"/> to silently capture failures in <see cref="AssemblyLoadResult"/>. The default is <see langword="false"/>.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, for chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="throwOnAssemblyLoadFailure"/> is <see langword="true"/> and at least one plugin assembly could not be loaded.
    /// </exception>
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
            services.AddTransient(d.SetupType);
        }

        foreach (var reg in featurePageRegistrations)
            services.AddTransient(reg.PageType);
        Console.WriteLine("Completed Prop System Setup");
        return services;
    }
}