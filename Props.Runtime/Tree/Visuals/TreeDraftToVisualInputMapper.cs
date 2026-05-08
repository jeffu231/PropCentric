using Props.Abstractions.Visuals;
using Props.Runtime.Tree.Setup;

namespace Props.Runtime.Tree.Visuals;

/// <summary>
/// Projects a <see cref="TreePropDraft"/> onto a <see cref="TreeVisualInput"/> record for use
/// by the wizard preview coordinator.
/// </summary>
public sealed class TreeDraftToVisualInputMapper : IVisualInputMapper<TreePropDraft, TreeVisualInput>
{
    public TreeVisualInput Map(TreePropDraft d) => new(
        d.Strings,
        d.NodesPerString,
        d.LightSize,
        d.DegreesCoverage,
        d.DegreeOffset,
        d.BaseHeight,
        d.TopHeight,
        d.TopWidth,
        d.StartLocation,
        d.ZigZag,
        d.ZigZagOffset,
        d.TopRadius,
        d.BottomRadius,
        d.AxisRotations.ToList()
    );
}
