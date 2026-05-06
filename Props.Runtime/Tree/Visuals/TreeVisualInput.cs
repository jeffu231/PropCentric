using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Props;

namespace Props.Runtime.Tree.Visuals;

public sealed record TreeVisualInput(
    int Strings,
    int NodesPerString,
    int LightSize,
    int DegreesCoverage,
    int DegreeOffset,
    int BaseHeight,
    int TopHeight,
    int TopWidth,
    StartLocation StartLocation,
    bool ZigZag,
    int ZigZagOffset,
    float TopRadius,
    float BottomRadius,
    IReadOnlyList<AxisRotationModel> AxisRotations
);
