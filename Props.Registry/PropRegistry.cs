using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public class PropRegistry : IPropRegistry
{
    private readonly Dictionary<Guid, PropDescriptor> _byId = new();
    private readonly Dictionary<Type, PropDescriptor> _byType = new();
    private readonly Dictionary<PropFeatureFlags, HashSet<Guid>> _featureIndex = new();
    private readonly PropFeatureInferrer _featureInferrer;

    public PropRegistry(IReadOnlyList<PropDescriptor> descriptors, PropFeatureInferrer featureInferrer)
    {
        _featureInferrer = featureInferrer;
        Build(descriptors);
    }
    
    private void Build(IEnumerable<PropDescriptor> descriptors)
    {
        foreach (var d in descriptors)
        {
            _byId[d.Id] = d;
            Register(d);
        }
    }
    private void Register(PropDescriptor descriptor)
    {
        var inferredFlags = _featureInferrer.Infer(descriptor.PropType);

        descriptor.Flags = inferredFlags; // cached result

        _byId[descriptor.Id] = descriptor;
        _byType[descriptor.PropType] = descriptor;

        IndexFeatures(descriptor);
    }
    
    private void IndexFeatures(PropDescriptor d)
    {
        foreach (PropFeatureFlags flag in Enum.GetValues(typeof(PropFeatureFlags)))
        {
            if (flag == PropFeatureFlags.None)
                continue;

            if (d.Flags.HasFlag(flag))
            {
                if (!_featureIndex.TryGetValue(flag, out var set))
                {
                    set = new HashSet<Guid>();
                    _featureIndex[flag] = set;
                }

                set.Add(d.Id);
            }
        }
    }

    public PropDescriptor GetDescriptor(Guid id)
    {
        if (_byId.TryGetValue(id, out var d))
            return d;

        throw new InvalidOperationException(
            $"No descriptor registered for prop id '{id}'. " +
            "Ensure the type is decorated with [PropDescriptor] and its assembly is in the plugin directory.");
    }

    public PropDescriptor GetDescriptor(IProp prop)
    {
        var type = prop.GetType();
        if (_byType.TryGetValue(type, out var d))
            return d;

        throw new InvalidOperationException(
            $"No descriptor registered for prop type '{type.FullName}'. " +
            "Ensure the type is decorated with [PropDescriptor] and its assembly is in the plugin directory.");
    }

    public IEnumerable<PropDescriptor> GetAllDescriptors()
    {
        return _byId.Values.ToArray();
    }

    public IEnumerable<PropDescriptor> GetDescriptorsByFeature(PropFeatureFlags flag)
    {
        return _byId.Values.Where(d => d.Flags.HasFlag(flag));
    }
    
}