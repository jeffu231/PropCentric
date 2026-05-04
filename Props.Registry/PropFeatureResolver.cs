using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

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