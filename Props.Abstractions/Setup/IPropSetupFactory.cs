using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

/// <summary>
/// Creates <see cref="IPropSetup"/> instances for driving the prop configuration wizard.
/// </summary>
public interface IPropSetupFactory
{
    /// <summary>Creates a setup instance for the prop type identified by the given GUID.</summary>
    /// <param name="id">The GUID declared on the prop's <see cref="Props.PropDescriptorAttribute"/>.</param>
    /// <returns>A new <see cref="IPropSetup"/> instance for the matching prop type.</returns>
    IPropSetup Create(Guid id);

    /// <summary>Creates a setup instance for the prop type described by a catalog item.</summary>
    /// <param name="item">The catalog item whose prop type is used to resolve the setup class.</param>
    /// <returns>A new <see cref="IPropSetup"/> instance for the prop type in <paramref name="item"/>.</returns>
    IPropSetup CreateFromCatalogItem(IPropCatalogItem item);
}