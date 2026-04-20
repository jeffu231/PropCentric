using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public class PropFeatureResolver(IPropRegistry registry) : IPropFeatureResolver
{
    public bool HasFeature(IProp prop, PropFeatureFlags feature)
    {
        var flags = GetDescriptor(prop).Flags;
        if(flags.HasFlag(feature)) return true;
        return false;
    }

    public PropFeatureFlags GetFeatures(IProp prop)
    {
        return GetDescriptor(prop).Flags;
    }
    
    private PropDescriptor GetDescriptor(IProp prop) => registry.GetDescriptor(prop);
}