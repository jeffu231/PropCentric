using Props.Abstractions.Visuals;
using Props.Runtime.Tree.Setup;

namespace Props.Runtime.Tree.Visuals;

/// <summary>
/// Projects a <see cref="TreePropDraft"/> onto a <see cref="TreeVisualInput"/> record for use
/// by the wizard preview coordinator.
/// </summary>
public sealed class TreeDraftToVisualInputMapper : IVisualInputMapper<TreePropDraft, TreeVisualInput>
{
    public TreeVisualInput Map(TreePropDraft prop) => new(
        prop.Strings,
        prop.NodesPerString,
        prop.LightSize,
        prop.DegreesCoverage,
        prop.DegreeOffset,
        prop.TopRadius,
        prop.BottomRadius,
        prop.AxisRotations);
}
