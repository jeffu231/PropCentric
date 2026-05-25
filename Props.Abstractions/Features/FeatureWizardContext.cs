using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;

namespace Props.Abstractions.Features;

/// <summary>
/// Represents the shared wizard-scoped services and state passed to a feature wizard page.
/// </summary>
/// <remarks>
/// A single instance is created for each wizard flow. The <see cref="Draft"/> is the canonical
/// backing store for all wizard-editable state, and the <see cref="PreviewSession"/> exposes the
/// shared preview pipeline for pages that need to trigger or host preview updates.
/// </remarks>
public sealed class FeatureWizardContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureWizardContext"/> class.
    /// </summary>
    /// <param name="draft">The shared draft for the current wizard flow.</param>
    /// <param name="previewSession">The shared preview session for the current wizard flow.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="draft"/> or <paramref name="previewSession"/> is <see langword="null"/>.
    /// </exception>
    public FeatureWizardContext(IPropDraft draft, IWizardPreviewSession previewSession)
    {
        Draft = draft ?? throw new ArgumentNullException(nameof(draft));
        PreviewSession = previewSession ?? throw new ArgumentNullException(nameof(previewSession));
    }

    /// <summary>
    /// Gets the shared draft for the current wizard flow.
    /// </summary>
    public IPropDraft Draft { get; }

    /// <summary>
    /// Gets the shared preview session for the current wizard flow.
    /// </summary>
    public IWizardPreviewSession PreviewSession { get; }
}
