using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;

namespace Props.Runtime.PolyLine.Visuals;

/// <summary>
/// Projects a <see cref="PolyLinePropDraft"/> onto a <see cref="PolyLineVisualInput"/> record for wizard previews.
/// </summary>
public sealed class PolyLineDraftToVisualInputMapper : IVisualInputMapper<PolyLinePropDraft, PolyLineVisualInput>
{
    public PolyLineVisualInput Map(PolyLinePropDraft source) => new(
        SnapshotSegments(source.Segments),
        source.LightSize);

    private static IReadOnlyList<Segment> SnapshotSegments(IEnumerable<SegmentDraftState> segments)
    {
        return segments.Select(segment => new Segment(segment.Start, segment.End, segment.PointCount)).ToArray();
    }

}
