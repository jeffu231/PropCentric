using System.Reflection;
using Props.Abstractions.Features;

namespace Props.Registry;

/// <summary>
/// Infers the <see cref="PropFeatureFlags"/> for a prop type by examining which feature interfaces it implements.
/// </summary>
/// <remarks>
/// On construction, scans <c>Props.Abstractions</c> once for interfaces carrying <see cref="PropFeatureAttribute"/>
/// and builds a lookup table. Subsequent <see cref="Infer"/> calls are a simple dictionary walk with no reflection.
/// Registered as a singleton so initialization cost is paid once at startup.
/// </remarks>
public sealed class PropFeatureInferrer
{
    private readonly IReadOnlyDictionary<Type, PropFeatureFlags> _interfaceToFlag;

    /// <summary>Initializes a new instance of the <see cref="PropFeatureInferrer"/> class.</summary>
    public PropFeatureInferrer()
    {
        var abstractions = typeof(PropFeatureFlags).Assembly;
        _interfaceToFlag = abstractions.GetTypes()
            .Where(t => t.IsInterface)
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<PropFeatureAttribute>()))
            .Where(x => x.Attr != null)
            .ToDictionary(x => x.Type, x => x.Attr!.Flag);
    }

    /// <summary>Returns the combined feature flags for the given prop type.</summary>
    /// <param name="propType">The concrete prop type to evaluate.</param>
    /// <returns>A bitwise combination of <see cref="PropFeatureFlags"/> values for every feature interface the type implements.</returns>
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
