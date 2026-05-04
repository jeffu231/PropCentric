using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public interface IPropRegistry
{
    
    PropDescriptor GetDescriptorById(Guid id);
    PropDescriptor GetDescriptorForProp(IProp prop);
    IEnumerable<PropDescriptor> GetAllDescriptors();
    IEnumerable<PropDescriptor> GetDescriptorsByFeature(PropFeatureFlags flags);
}