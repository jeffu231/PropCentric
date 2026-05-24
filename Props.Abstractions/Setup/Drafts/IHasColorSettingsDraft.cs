using Props.Abstractions.Features;

namespace Props.Abstractions.Setup.Drafts;

/// <summary>
/// Exposes mutable draft color settings for wizard flows that edit shared color configuration.
/// </summary>
public interface IHasColorSettingsDraft
{
    /// <summary>
    /// Gets or sets the color configuration being edited for the current wizard flow.
    /// </summary>
    LightColorConfiguration ColorConfiguration { get; set; }
}
