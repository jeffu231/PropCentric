using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Visuals;

namespace Props.Runtime;

/// <summary>
/// Provides the <see cref="AddTreePropServices"/> extension method for registering all
/// tree-prop-specific visual pipeline services into an <see cref="IServiceCollection"/>.
/// </summary>
public static class TreePropServicesExtensions
{
    /// <summary>
    /// Registers the visual input mappers, visual model factory, draft mapper, and preview coordinator
    /// for the <see cref="TreeProp"/> pipeline.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, for chaining.</returns>
    public static IServiceCollection AddTreePropServices(this IServiceCollection services)
    {
        services.AddTransient<IVisualInputMapper<TreeProp, TreeVisualInput>, TreePropToVisualInputMapper>();
        services.AddTransient<IVisualInputMapper<TreePropDraft, TreeVisualInput>, TreeDraftToVisualInputMapper>();
        services.AddTransient<IPropVisualModelBuilder<TreeVisualInput, TreePropVisualModel>, TreeVisualModelBuilder>();
        services.AddTransient<IPropDraftMapper<TreePropDraft, TreeProp>, TreePropDraftMapper>();
        services.AddTransient<IWizardPreviewCoordinator<TreePropDraft>, TreeWizardPreviewCoordinator>();
        return services;
    }
}
