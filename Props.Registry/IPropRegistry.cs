using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public interface IPropRegistry
{
    //IPropMetaData GetMeta(Guid id);
    PropDescriptor GetDescriptor(Guid id);
    //IEnumerable<IPropMetaData> GetByFeature(PropFeatureFlags feature);
    IEnumerable<PropDescriptor> GetAllDescriptors();
    IEnumerable<PropDescriptor> GetDescriptorsByFeature(PropFeatureFlags flag);
}