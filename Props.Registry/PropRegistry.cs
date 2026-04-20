using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public class PropRegistry : IPropRegistry
{
    private readonly IServiceProvider _services;

    private readonly Dictionary<Guid, PropDescriptor> _byId = new();
    
    private readonly Dictionary<Type, PropDescriptor> _byType = new();

    private readonly Dictionary<PropFeatureFlags, HashSet<Guid>> _featureIndex = new();

    public PropRegistry(IServiceProvider services, IReadOnlyList<PropDescriptor> descriptors)
    {
        _services = services; //If we need DI for something. 
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
        var inferredFlags = PropFeatureRegistry.Infer(descriptor.PropType);

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
        {
            return d;
        }
        
        throw new Exception($"Descriptor not found: {id}");
    }

    public PropDescriptor GetDescriptor(IProp prop)
    {
        return _byType[prop.GetType()];
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