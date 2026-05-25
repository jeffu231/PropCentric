using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup;

namespace Props.Abstractions.Visuals;

/// <summary>
/// Exposes a shared draft and preview builder for a single wizard instance.
/// </summary>
public interface IWizardPreviewSession
{
    /// <summary>Gets the shared draft that backs the current wizard flow.</summary>
    IPropDraft Draft { get; }

    /// <summary>Builds the current preview model from the shared draft.</summary>
    Task<IPropVisualModel> BuildPreviewAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Exposes a strongly typed shared draft and preview builder for a single wizard instance.
/// </summary>
/// <typeparam name="TDraft">The concrete draft type for the wizard flow.</typeparam>
public interface IWizardPreviewSession<out TDraft> : IWizardPreviewSession
    where TDraft : class, IPropDraft
{
    /// <summary>Gets the shared draft that backs the current wizard flow.</summary>
    new TDraft Draft { get; }
}
