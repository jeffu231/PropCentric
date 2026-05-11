using Props.Abstractions.Visuals;

namespace Props.Runtime.PolyLine.Visuals;

/// <summary>
/// Projects a <see cref="PolyLineProp"/> onto a <see cref="PolyLineVisualInput"/> record for runtime rendering.
/// </summary>
public sealed class PolyLinePropToVisualInputMapper : IVisualInputMapper<PolyLineProp, PolyLineVisualInput>
{
    public PolyLineVisualInput Map(PolyLineProp source) => new(
        source.Segments,
        source.LightSize,
        source.AxisRotations);
}
