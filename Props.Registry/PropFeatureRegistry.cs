using System.Reflection;
using Props.Abstractions.Features;

namespace Props.Registry;

public static class PropFeatureRegistry
{
    private static readonly Dictionary<Type, PropFeatureFlags> InterfaceToFlag = new();

    public static void Initialize(IReadOnlyList<Assembly> assemblies)
    {
        Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Props.Abstractions");
        if (assembly == null) return;
        
        var interfaces = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsInterface && t.Namespace == "Props.Abstractions.Features");

        foreach (var iface in interfaces)
        {
            var attr = iface.GetCustomAttribute<PropFeatureAttribute>();
            if (attr == null)
                continue;

            InterfaceToFlag[iface] = attr.Flag;
        }
    }

    public static PropFeatureFlags Infer(Type propType)
    {
        PropFeatureFlags flags = 0;

        foreach (var kv in InterfaceToFlag)
        {
            var iface = kv.Key;
            var flag = kv.Value;

            if (iface.IsAssignableFrom(propType))
            {
                flags |= flag;
            }
        }

        return flags;
    }
}