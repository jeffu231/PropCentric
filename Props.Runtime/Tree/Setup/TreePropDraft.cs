using System.Collections.ObjectModel;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;

namespace Props.Runtime.Tree.Setup;

public sealed class TreePropDraft : IPropDraft
{
    public string Name { get; set; } = "Tree 1";
    public int Strings { get; set; } = 16;
    public int NodesPerString { get; set; } = 50;
    public int LightSize { get; set; } = 2;
    public int DegreesCoverage { get; set; } = 360;
    public int DegreeOffset { get; set; }
    public int BaseHeight { get; set; } = 40;
    public int TopHeight { get; set; } = 20;
    public int TopWidth { get; set; } = 20;
    public StartLocation StartLocation { get; set; } = StartLocation.BottomLeft;
    public bool ZigZag { get; set; }
    public int ZigZagOffset { get; set; } = 50;
    public float TopRadius { get; set; } = 10;
    public float BottomRadius { get; set; } = 100;
    public ObservableCollection<AxisRotationModel> AxisRotations { get; set; } = [];
}
