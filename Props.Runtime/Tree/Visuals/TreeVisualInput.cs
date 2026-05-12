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
/// <param name="TopRadius">The radius at the top of the tree as a percentage of the maximum width.</param>
/// <param name="BottomRadius">The radius at the base of the tree as a percentage of the maximum width.</param>
/// <param name="AxisRotations">The current 3-D axis rotation states.</param>
public sealed record TreeVisualInput(
    int Strings,
    int NodesPerString,
    int LightSize,
    int DegreesCoverage,
    int DegreeOffset,
    float TopRadius,
    float BottomRadius,
    IReadOnlyList<AxisRotationModel> AxisRotations
)
{
    public bool Equals(TreeVisualInput? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null)
        {
            return false;
        }

        return Strings == other.Strings
            && NodesPerString == other.NodesPerString
            && LightSize == other.LightSize
            && DegreesCoverage == other.DegreesCoverage
            && DegreeOffset == other.DegreeOffset
            && TopRadius.Equals(other.TopRadius)
            && BottomRadius.Equals(other.BottomRadius)
            && RotationsEqual(AxisRotations, other.AxisRotations);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Strings);
        hash.Add(NodesPerString);
        hash.Add(LightSize);
        hash.Add(DegreesCoverage);
        hash.Add(DegreeOffset);
        hash.Add(TopRadius);
        hash.Add(BottomRadius);

        foreach (var rotation in AxisRotations)
        {
            hash.Add(rotation.Axis);
            hash.Add(rotation.RotationAngle);
        }

        return hash.ToHashCode();
    }

    private static bool RotationsEqual(
        IReadOnlyList<AxisRotationModel> left,
        IReadOnlyList<AxisRotationModel> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (left[index].Axis != right[index].Axis || left[index].RotationAngle != right[index].RotationAngle)
            {
                return false;
            }
        }

        return true;
    }
}
