using Props.Abstractions.Props;
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
        VisualInputRotationSupport.SnapshotRotations(source.AxisRotations));

    private static IReadOnlyList<Segment> SnapshotSegments(IEnumerable<Segment> segments)
    {
        return segments.Select(segment => new Segment(segment.Start, segment.End, segment.PointCount)).ToArray();
    }

}
