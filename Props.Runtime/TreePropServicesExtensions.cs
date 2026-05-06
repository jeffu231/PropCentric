using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Visuals;

namespace Props.Runtime;

public static class TreePropServicesExtensions
{
    public static IServiceCollection AddTreePropServices(this IServiceCollection services)
    {
        services.AddTransient<IVisualInputMapper<TreeProp, TreeVisualInput>, TreePropToVisualInputMapper>();
        services.AddTransient<IVisualInputMapper<TreePropDraft, TreeVisualInput>, TreeDraftToVisualInputMapper>();
        services.AddTransient<IPropVisualModelFactory<TreeVisualInput>, TreeVisualModelFactory>();
        services.AddTransient<IPropDraftMapper<TreePropDraft, TreeProp>, TreePropDraftMapper>();
        return services;
    }
}
