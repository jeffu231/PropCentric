using System.Collections.ObjectModel;
using Props.Abstractions.Features;
using Props.Abstractions.Setup;
using Props.Abstractions.Setup.Drafts;

namespace Props.Runtime.PolyLine.Setup;

/// <summary>
/// Wizard-owned draft that holds user-entered field values for a <see cref="PolyLineProp"/> during setup.
/// </summary>
public sealed class PolyLinePropDraft : IPropDraft, IHasSegmentsDraft, IHasColorSettingsDraft, IHasLightSettingsDraft
{
    /// <summary>Gets or sets the display name of the prop.</summary>
    public string Name { get; set; } = "PolyLine 1";

    /// <summary>Gets or sets the rendered diameter of each light node in pixels.</summary>
    public int LightSize { get; set; } = 2;

    /// <summary>Gets or sets the color configuration for the draft.</summary>
    public LightColorConfiguration ColorConfiguration { get; set; } = LightColorConfiguration.CreateDefault();

    /// <summary>Gets or sets the ordered draft segments being edited by the wizard.</summary>
    public ObservableCollection<SegmentDraftState> Segments { get; set; } = [];
}
