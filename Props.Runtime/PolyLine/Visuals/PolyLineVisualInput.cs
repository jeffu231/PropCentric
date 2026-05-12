using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;

namespace Props.Runtime.PolyLine.Visuals;

/// <summary>
/// Captures the geometry-relevant fields of a polyline prop in an immutable record.
/// </summary>
/// <remarks>
/// Structural equality (provided by <c>record</c>) lets a preview coordinator skip rebuilds
/// when no geometry-affecting field has changed.
/// </remarks>
/// <param name="Segments">The ordered normalized segment geometry to render.</param>
/// <param name="LightSize">The rendered diameter of each light node in pixels.</param>
/// <param name="AxisRotations">The current 3-D axis rotation states.</param>
public sealed record PolyLineVisualInput(
    IReadOnlyList<Segment> Segments,
    int LightSize,
    IReadOnlyList<AxisRotationModel> AxisRotations)
{
    public bool Equals(PolyLineVisualInput? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null)
        {
            return false;
        }

        return LightSize == other.LightSize
            && SegmentsEqual(Segments, other.Segments)
            && RotationsEqual(AxisRotations, other.AxisRotations);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(LightSize);

        foreach (var segment in Segments)
        {
            hash.Add(segment);
        }

        foreach (var rotation in AxisRotations)
        {
            hash.Add(rotation.Axis);
            hash.Add(rotation.RotationAngle);
        }

        return hash.ToHashCode();
    }

    private static bool SegmentsEqual(IReadOnlyList<Segment> left, IReadOnlyList<Segment> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (left[index] != right[index])
            {
                return false;
            }
        }

        return true;
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
