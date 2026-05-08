using Props.Abstractions.Visuals;

namespace Props.Runtime.Tree.Visuals;

/// <summary>
/// Projects a <see cref="TreeProp"/> onto a <see cref="TreeVisualInput"/> record for use
/// by <c>BuildVisualModel()</c> at runtime.
/// </summary>
public sealed class TreePropToVisualInputMapper : IVisualInputMapper<TreeProp, TreeVisualInput>
{
    public TreeVisualInput Map(TreeProp p) => new(
        p.Strings,
        p.NodesPerString,
        p.LightSize,
        p.DegreesCoverage,
        p.DegreeOffset,
        p.BaseHeight,
        p.TopHeight,
        p.TopWidth,
        p.StartLocation,
        p.ZigZag,
        p.ZigZagOffset,
        p.TopRadius,
        p.BottomRadius,
        p.AxisRotations.ToList()
    );
}
