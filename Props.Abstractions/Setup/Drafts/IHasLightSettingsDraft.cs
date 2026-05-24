namespace Props.Abstractions.Setup.Drafts;

/// <summary>
/// Exposes mutable light-related draft state for wizard pages that edit shared light settings.
/// </summary>
public interface IHasLightSettingsDraft : IHasNameDraft
{
    /// <summary>Gets or sets the rendered diameter of each light node in pixels.</summary>
    int LightSize { get; set; }
}
