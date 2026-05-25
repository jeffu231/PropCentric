using Orc.Wizard;

namespace Props.Abstractions.Features;

/// <summary>
/// Resolves feature wizard pages for a given prop type.
/// </summary>
/// <remarks>
/// Reflection runs once at startup in <see cref="Props.Registry.FeatureWizardPageScanner"/>;
/// call sites invoke this interface with no runtime reflection.
/// Pages are instantiated via the DI container and returned in priority order.
/// </remarks>
public interface IFeatureWizardPageResolver
{
    /// <summary>Returns the instantiated feature wizard pages that apply to the specified prop type.</summary>
    /// <param name="propType">The <see cref="Type"/> of the prop for which pages are resolved.</param>
    /// <returns>
    /// A read-only list of <see cref="IWizardPage"/> instances ordered by their declared priority,
    /// covering every feature interface that <paramref name="propType"/> implements.
    /// </returns>
    IReadOnlyList<IWizardPage> GetPagesFor(Type propType);

    /// <summary>
    /// Initializes any feature pages that opt into shared-draft behavior for the current wizard instance.
    /// </summary>
    /// <param name="pages">The resolved feature pages to initialize.</param>
    /// <param name="context">The shared wizard context for the current wizard flow.</param>
    void InitializePages(IReadOnlyList<IWizardPage> pages, FeatureWizardContext context);
}
