using Props.Abstractions.Visuals;
using Props.Abstractions.PropVisualModels;

namespace Props.Runtime.Tree.Visuals;

/// <summary>
/// Projects a <see cref="TreeProp"/> onto a <see cref="TreeVisualInput"/> record for use
/// by <c>BuildVisualModel()</c> at runtime.
/// </summary>
public sealed class TreePropToVisualInputMapper : IVisualInputMapper<TreeProp, TreeVisualInput>
{
    public TreeVisualInput Map(TreeProp prop) => new(
        prop.Strings,
        prop.NodesPerString,
        prop.LightSize,
        prop.DegreesCoverage,
        prop.DegreeOffset,
        prop.TopRadius,
        prop.BottomRadius,
        SnapshotRotations(prop.AxisRotations));

    private static IReadOnlyList<AxisRotationModel> SnapshotRotations(IEnumerable<AxisRotationModel> rotations)
    {
        return rotations.Select(rotation => new AxisRotationModel
        {
            Axis = rotation.Axis,
            RotationAngle = rotation.RotationAngle
        }).ToArray();
    }
}
