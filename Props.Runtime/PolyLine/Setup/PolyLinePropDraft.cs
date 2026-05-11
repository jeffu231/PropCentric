using System.Collections.ObjectModel;
using Props.Abstractions.Features;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup;

namespace Props.Runtime.PolyLine.Setup;

/// <summary>
/// Wizard-owned draft that holds user-entered field values for a <see cref="PolyLineProp"/> during setup.
/// </summary>
public sealed class PolyLinePropDraft : IPropDraft, IHasSegmentsDraft
{
    /// <summary>Gets or sets the display name of the prop.</summary>
    public string Name { get; set; } = "PolyLine 1";

    /// <summary>Gets or sets the rendered diameter of each light node in pixels.</summary>
    public int LightSize { get; set; } = 2;

    /// <summary>Gets or sets the ordered draft segments being edited by the wizard.</summary>
    public ObservableCollection<SegmentDraftState> Segments { get; set; } = [];

    /// <summary>Gets or sets the 3-D axis rotation states for the preview rendering.</summary>
    public ObservableCollection<AxisRotationModel> AxisRotations { get; set; } = [];
}
