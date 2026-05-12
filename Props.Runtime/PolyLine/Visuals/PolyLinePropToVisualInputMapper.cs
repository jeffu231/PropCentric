using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;

namespace Props.Runtime.PolyLine.Visuals;

/// <summary>
/// Projects a <see cref="PolyLineProp"/> onto a <see cref="PolyLineVisualInput"/> record for runtime rendering.
/// </summary>
public sealed class PolyLinePropToVisualInputMapper : IVisualInputMapper<PolyLineProp, PolyLineVisualInput>
{
    public PolyLineVisualInput Map(PolyLineProp source) => new(
        SnapshotSegments(source.Segments),
        source.LightSize,
        SnapshotRotations(source.AxisRotations));

    private static IReadOnlyList<Segment> SnapshotSegments(IEnumerable<Segment> segments)
    {
        return segments.Select(segment => new Segment(segment.Start, segment.End, segment.PointCount)).ToArray();
    }

    private static IReadOnlyList<AxisRotationModel> SnapshotRotations(IEnumerable<AxisRotationModel> rotations)
    {
        return rotations.Select(rotation => new AxisRotationModel
        {
            Axis = rotation.Axis,
            RotationAngle = rotation.RotationAngle
        }).ToArray();
    }
}
