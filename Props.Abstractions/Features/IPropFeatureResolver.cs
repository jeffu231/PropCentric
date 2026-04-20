using Props.Abstractions.Props;

namespace Props.Abstractions.Features;

public interface IPropFeatureResolver
{
    bool HasFeature(IProp prop, PropFeatureFlags feature);
    PropFeatureFlags GetFeatures(IProp prop);
}