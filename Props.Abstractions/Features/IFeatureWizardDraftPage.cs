using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;

namespace Props.Abstractions.Features;

/// <summary>
/// Optional capability for feature wizard pages that need direct access to the shared draft and preview session.
/// </summary>
public interface IFeatureWizardDraftPage
{
    /// <summary>
    /// Initializes the feature page with the shared wizard draft and preview session.
    /// </summary>
    /// <param name="draft">The shared draft for the current wizard flow.</param>
    /// <param name="previewSession">The shared preview session for the current wizard flow.</param>
    void Initialize(IPropDraft draft, IWizardPreviewSession previewSession);
}
