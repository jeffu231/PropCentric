using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

/// <summary>
/// Adapts <see cref="IPropRegistry"/> descriptors into <see cref="IPropCatalogItem"/> entries
/// for consumption by the UI and discovery layer.
/// </summary>
public sealed class PropCatalogProvider : IPropCatalogProvider
{
    private readonly IPropRegistry _registry;

    /// <summary>Initializes a new instance of the <see cref="PropCatalogProvider"/> class.</summary>
    /// <param name="registry">The registry that provides the underlying prop descriptors.</param>
    public PropCatalogProvider(IPropRegistry registry)
    {
        _registry = registry;
    }

    public IEnumerable<IPropCatalogItem> GetPropCatalog()
        => _registry.GetAllDescriptors().Select(ToItem);

    public IEnumerable<IPropCatalogItem> GetPropCatalogByFeature(PropFeatureFlags flags)
        => _registry.GetDescriptorsByFeature(flags).Select(ToItem);

    private static PropCatalogItem ToItem(PropDescriptor d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Icon = d.Icon,
        Features = d.Flags,
        WizardType = d.SetupType,
        PropType = d.PropType
    };
}
