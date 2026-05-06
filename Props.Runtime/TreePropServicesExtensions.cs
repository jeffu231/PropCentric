using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Visuals;

namespace Props.Runtime;

public static class TreePropServicesExtensions
{
    public static IServiceCollection AddTreePropServices(this IServiceCollection services)
    {
        services.AddTransient<IVisualInputMapper<TreeProp, TreeVisualInput>, TreePropToVisualInputMapper>();
        services.AddTransient<IPropVisualModelFactory<TreeVisualInput>, TreeVisualModelFactory>();
        return services;
    }
}
