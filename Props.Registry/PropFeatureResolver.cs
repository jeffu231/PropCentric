using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

/// <summary>
/// Resolves feature flags for a prop instance by looking up its descriptor in the registry.
/// </summary>
public class PropFeatureResolver(IPropRegistry registry) : IPropFeatureResolver
{
    public bool HasFeature(IProp prop, PropFeatureFlags feature)
        => GetDescriptor(prop).Flags.HasFlag(feature);

    public PropFeatureFlags GetFeatures(IProp prop)
    {
        return GetDescriptor(prop).Flags;
    }
    
    private PropDescriptor GetDescriptor(IProp prop) => registry.GetDescriptorForProp(prop);
}