namespace Props.Abstractions.Features;

/// <summary>
/// Optional capability for feature wizard pages that need direct access to the shared wizard context.
/// </summary>
public interface IFeatureWizardDraftPage
{
    /// <summary>
    /// Initializes the feature page with the shared wizard context.
    /// </summary>
    /// <param name="context">The shared wizard context for the current wizard flow.</param>
    void Initialize(FeatureWizardContext context);
}
