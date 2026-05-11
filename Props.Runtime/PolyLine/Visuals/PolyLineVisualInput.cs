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
    IReadOnlyList<AxisRotationModel> AxisRotations);
