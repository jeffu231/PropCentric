using System.Collections.ObjectModel;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Abstractions.Setup.Drafts;
using Vixen.Sys.Props;

namespace Props.Runtime.Tree.Setup;

/// <summary>
/// Wizard-owned draft that holds user-entered field values for a <see cref="TreeProp"/> during setup.
/// </summary>
public sealed class TreePropDraft : IPropDraft, IHasAxisRotationsDraft
{
    /// <summary>Gets or sets the display name of the prop.</summary>
    public string Name { get; set; } = "Tree 1";

    /// <summary>Gets or sets the number of light strings on the tree.</summary>
    public int Strings { get; set; } = 16;

    /// <summary>Gets or sets the number of individually addressable nodes per string.</summary>
    public int NodesPerString { get; set; } = 50;

    /// <summary>Gets or sets the rendered diameter of each light node in pixels.</summary>
    public int LightSize { get; set; } = 2;
    
    public StringTypes StringType { get; set; } = StringTypes.ColorMixingRGB;

    /// <summary>Gets or sets the arc of the tree covered by strings, in degrees (1–360).</summary>
    public int DegreesCoverage { get; set; } = 360;

    /// <summary>Gets or sets the rotational offset of string 1 from the default position, in degrees.</summary>
    public int DegreeOffset { get; set; }

    /// <summary>Gets or sets the visual height of the tree base as a percentage.</summary>
    public int BaseHeight { get; set; } = 40;

    /// <summary>Gets or sets the visual height of the tree top as a percentage.</summary>
    public int TopHeight { get; set; } = 20;

    /// <summary>Gets or sets the visual width of the tree top as a percentage.</summary>
    public int TopWidth { get; set; } = 20;

    /// <summary>Gets or sets the corner from which element patching begins.</summary>
    public StartLocation StartLocation { get; set; } = StartLocation.BottomLeft;

    /// <summary>Gets or sets a value that indicates whether the patching order alternates direction between strings.</summary>
    /// <value><see langword="true"/> if zig-zag patching is enabled; otherwise, <see langword="false"/>.</value>
    public bool ZigZag { get; set; }

    /// <summary>Gets or sets the number of elements per string before the zig-zag direction reverses.</summary>
    public int ZigZagOffset { get; set; } = 50;

    /// <summary>Gets or sets the radius at the top of the tree as a percentage of the maximum width.</summary>
    public float TopRadius { get; set; } = 10;

    /// <summary>Gets or sets the radius at the base of the tree as a percentage of the maximum width.</summary>
    public float BottomRadius { get; set; } = 100;

    /// <summary>Gets or sets the 3-D axis rotation states for the preview rendering.</summary>
    public ObservableCollection<AxisRotationModel> AxisRotations { get; set; } = [];
}
