using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public interface IPropRegistry
{
    
    PropDescriptor GetDescriptor(Guid id);
    PropDescriptor GetDescriptor(IProp prop);
    IEnumerable<PropDescriptor> GetAllDescriptors();
    IEnumerable<PropDescriptor> GetDescriptorsByFeature(PropFeatureFlags flag);
}