using System.Reflection;
using Props.Abstractions.Features;

namespace Props.Registry;

public sealed class PropFeatureInferrer
{
    private readonly IReadOnlyDictionary<Type, PropFeatureFlags> _interfaceToFlag;

    public PropFeatureInferrer()
    {
        var abstractions = typeof(PropFeatureFlags).Assembly;
        _interfaceToFlag = abstractions.GetTypes()
            .Where(t => t.IsInterface)
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<PropFeatureAttribute>()))
            .Where(x => x.Attr != null)
            .ToDictionary(x => x.Type, x => x.Attr!.Flag);
    }

    public PropFeatureFlags Infer(Type propType)
    {
        PropFeatureFlags flags = PropFeatureFlags.None;
        foreach (var (iface, flag) in _interfaceToFlag)
        {
            if (iface.IsAssignableFrom(propType))
                flags |= flag;
        }
        return flags;
    }
}
