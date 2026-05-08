using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

/// <summary>
/// Provides indexed access to all registered <see cref="PropDescriptor"/> entries.
/// </summary>
public interface IPropRegistry
{
    /// <summary>Returns the descriptor for the prop type with the given identifier.</summary>
    /// <param name="id">The GUID declared on the prop's <see cref="Props.Abstractions.Props.PropDescriptorAttribute"/>.</param>
    /// <returns>The matching <see cref="PropDescriptor"/>.</returns>
    /// <exception cref="InvalidOperationException">No descriptor is registered for <paramref name="id"/>.</exception>
    PropDescriptor GetDescriptorById(Guid id);

    /// <summary>Returns the descriptor for the same type as the given prop instance.</summary>
    /// <param name="prop">The prop instance whose runtime type is used for lookup.</param>
    /// <returns>The matching <see cref="PropDescriptor"/>.</returns>
    /// <exception cref="InvalidOperationException">The prop's type has no registered descriptor.</exception>
    PropDescriptor GetDescriptorForProp(IProp prop);

    /// <summary>Returns all registered descriptors.</summary>
    /// <returns>An enumerable of every <see cref="PropDescriptor"/> in the registry.</returns>
    IEnumerable<PropDescriptor> GetAllDescriptors();

    /// <summary>Returns descriptors for props that support all of the specified feature flags.</summary>
    /// <param name="flags">A bitwise combination of the enumeration values that specifies the required features.</param>
    /// <returns>An enumerable of <see cref="PropDescriptor"/> entries whose props satisfy every flag in <paramref name="flags"/>.</returns>
    IEnumerable<PropDescriptor> GetDescriptorsByFeature(PropFeatureFlags flags);
}