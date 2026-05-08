using Props.Abstractions.Features;

namespace Props.Abstractions.Props;

/// <summary>
/// Represents a single entry in the prop catalog, providing discovery metadata for one prop type.
/// </summary>
public interface IPropCatalogItem
{
    /// <summary>Gets the unique identifier for this prop type.</summary>
    /// <value>The GUID declared on the prop's <see cref="PropDescriptorAttribute"/>.</value>
    Guid Id { get; }

    /// <summary>Gets the human-readable display name of the prop.</summary>
    /// <value>The name shown in the prop catalog UI.</value>
    string Name { get; }

    /// <summary>Gets the icon resource key or path for the prop.</summary>
    /// <value>An icon identifier, or an empty string if none was declared.</value>
    string Icon { get; }

    /// <summary>Gets the wizard type used to configure this prop.</summary>
    /// <value>The <see cref="Type"/> of the prop's wizard class.</value>
    Type WizardType { get; init; }

    /// <summary>Gets the concrete prop type this catalog entry describes.</summary>
    /// <value>The <see cref="Type"/> of the prop class decorated with <see cref="PropDescriptorAttribute"/>.</value>
    Type PropType { get; init; }

    /// <summary>Gets the combined feature flags supported by this prop type.</summary>
    /// <value>A bitwise combination of the enumeration values describing the prop's capabilities.</value>
    PropFeatureFlags Features { get; }

    /// <summary>Gets a value that indicates whether this prop supports re-editing after initial setup.</summary>
    /// <value>
    /// <see langword="true"/> if the prop's wizard can be reopened to modify an existing instance;
    /// otherwise, <see langword="false"/>.
    /// </value>
    bool SupportsEditing { get; init; }
}