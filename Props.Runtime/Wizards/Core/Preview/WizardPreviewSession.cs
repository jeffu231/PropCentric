using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;

namespace Props.Runtime.Wizards.Core.Preview;

/// <summary>
/// Default shared preview-session implementation for a single wizard instance.
/// </summary>
/// <typeparam name="TDraft">The concrete draft type for the wizard flow.</typeparam>
public sealed class WizardPreviewSession<TDraft> : IWizardPreviewSession<TDraft>
    where TDraft : class, IPropDraft
{
    private readonly IWizardPreviewCoordinator<TDraft> _coordinator;

    public WizardPreviewSession(TDraft draft, IWizardPreviewCoordinator<TDraft> coordinator)
    {
        Draft = draft;
        _coordinator = coordinator;
    }

    /// <inheritdoc />
    public TDraft Draft { get; }

    IPropDraft IWizardPreviewSession.Draft => Draft;

    /// <inheritdoc />
    public IPropVisualModel BuildPreview()
    {
        return _coordinator.BuildPreview(Draft);
    }
}
