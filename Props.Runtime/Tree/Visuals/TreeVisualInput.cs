using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Props;

namespace Props.Runtime.Tree.Visuals;

/// <summary>
/// Captures the geometry-relevant fields of a tree prop in an immutable record.
/// </summary>
/// <remarks>
/// Structural equality (provided free by <c>record</c>) lets <see cref="TreeWizardPreviewCoordinator"/>
/// skip factory calls when no geometry-affecting field has changed.
/// </remarks>
/// <param name="Strings">The number of light strings on the tree.</param>
/// <param name="NodesPerString">The number of nodes per string.</param>
/// <param name="LightSize">The rendered diameter of each light node in pixels.</param>
/// <param name="DegreesCoverage">The arc covered by strings, in degrees.</param>
/// <param name="DegreeOffset">The rotational offset of string 1, in degrees.</param>
/// <param name="BaseHeight">The visual height of the tree base as a percentage.</param>
/// <param name="TopHeight">The visual height of the tree top as a percentage.</param>
/// <param name="TopWidth">The visual width of the tree top as a percentage.</param>
/// <param name="StartLocation">The corner from which element patching begins.</param>
/// <param name="ZigZag"><see langword="true"/> if zig-zag patching is enabled; otherwise, <see langword="false"/>.</param>
/// <param name="ZigZagOffset">The number of elements per string before the zig-zag direction reverses.</param>
/// <param name="TopRadius">The radius at the top of the tree as a percentage of the maximum width.</param>
/// <param name="BottomRadius">The radius at the base of the tree as a percentage of the maximum width.</param>
/// <param name="AxisRotations">The current 3-D axis rotation states.</param>
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
