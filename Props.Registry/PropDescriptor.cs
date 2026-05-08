using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

/// <summary>
/// Internal runtime descriptor for a discovered prop type, combining the metadata from
/// <see cref="PropDescriptorAttribute"/> with inferred feature flags.
/// </summary>
public sealed class PropDescriptor
{
    /// <summary>Gets the unique identifier for this prop type.</summary>
    /// <value>The GUID parsed from the prop's <see cref="PropDescriptorAttribute"/>.</value>
    public required Guid Id { get; init; }

    /// <summary>Gets the human-readable display name of the prop.</summary>
    /// <value>The name declared on the prop's <see cref="PropDescriptorAttribute"/>.</value>
    public required string Name { get; init; }

    /// <summary>Gets the icon resource key or path for the prop.</summary>
    /// <value>An icon identifier string, or an empty string if none was declared.</value>
    public required string Icon { get; init; }

    /// <summary>Gets the concrete prop class type.</summary>
    /// <value>The <see cref="Type"/> of the class decorated with <see cref="PropDescriptorAttribute"/>.</value>
    public required Type PropType { get; init; }

    /// <summary>Gets the setup class type that drives the prop's configuration wizard.</summary>
    /// <value>The <see cref="Type"/> of an <see cref="IPropSetup"/> implementation.</value>
    public required Type SetupType { get; init; }

    /// <summary>Gets or sets the combined feature flags inferred from the prop's implemented interfaces.</summary>
    /// <value>
    /// A bitwise combination of <see cref="PropFeatureFlags"/> values set by <see cref="PropFeatureInferrer"/>
    /// during registration. The default is <see cref="PropFeatureFlags.None"/>.
    /// </value>
    public PropFeatureFlags Flags { get; set; } = PropFeatureFlags.None;
}