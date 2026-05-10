using System.Numerics;

namespace Props.Abstractions.Setup;

/// <summary>
/// 
/// </summary>
/// <param name="Start"></param>
/// <param name="End"></param>
/// <param name="PointCount"></param>
public record CapturedWorldSegment(
    Vector2 Start,
    Vector2 End,
    int PointCount);