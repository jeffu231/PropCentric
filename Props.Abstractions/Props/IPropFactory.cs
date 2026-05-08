namespace Props.Abstractions.Props;

/// <summary>
/// Creates prop instances via the DI container.
/// </summary>
/// <remarks>
/// Use <see cref="IPropCatalogProvider"/> for discovery. Use this interface only when you need
/// a live, injected prop instance.
/// </remarks>
public interface IPropFactory
{
    /// <summary>Creates a prop instance for the given prop type identifier.</summary>
    /// <param name="id">The GUID declared on the prop's <see cref="PropDescriptorAttribute"/>.</param>
    /// <returns>A new <see cref="IProp"/> instance of the registered type.</returns>
    IProp Create(Guid id);

    /// <summary>Creates a prop instance of the specified concrete type.</summary>
    /// <typeparam name="TProp">The concrete prop type to instantiate.</typeparam>
    /// <returns>A new <typeparamref name="TProp"/> instance resolved from the DI container.</returns>
    TProp Create<TProp>() where TProp : IProp;
}