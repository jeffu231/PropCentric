using System.Numerics;

namespace Props.Abstractions.Props;

/// <summary>
/// Represents a single immutable physical segment of a prop in normalized model space.
/// </summary>
public sealed record Segment(
    Vector2 Start,
    Vector2 End,
    int PointCount);
