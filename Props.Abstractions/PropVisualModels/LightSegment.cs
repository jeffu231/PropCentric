using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// An ordered sequence of individually addressable lights along a physical linear path.
/// Start and End define the physical extent of the segment — the viewer draws the
/// segment line between these two points. Lights are the individually addressable
/// points distributed along that path.
///
/// Use for rope lights, roof edges, window frames, LED strips.
/// Compose multiple LightSegments in PropVisualModel.Elements for multi-segment props
/// (e.g. 4 segments for the 4 sides of a window frame).
/// </summary>
public sealed class LightSegment : IVisualElement
{
    public Vector3 Start { get; init; }
    public Vector3 End { get; init; }
    public IReadOnlyList<LightPoint> Lights { get; init; } = [];
    public float PointSize { get; init; } = 2f;
}
