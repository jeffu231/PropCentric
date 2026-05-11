using Props.Abstractions.Props;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;

namespace Props.Runtime.PolyLine.Visuals;

/// <summary>
/// Projects a <see cref="PolyLinePropDraft"/> onto a <see cref="PolyLineVisualInput"/> record for wizard previews.
/// </summary>
public sealed class PolyLinePropDraftToVisualInputMapper : IVisualInputMapper<PolyLinePropDraft, PolyLineVisualInput>
{
    public PolyLineVisualInput Map(PolyLinePropDraft source) => new(
        source.Segments
            .Select(segment => new Segment(segment.Start, segment.End, segment.PointCount))
            .ToArray(),
        source.LightSize,
        source.AxisRotations);
}
