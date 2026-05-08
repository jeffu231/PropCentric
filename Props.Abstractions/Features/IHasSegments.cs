using Props.Abstractions.Props;

namespace Props.Abstractions.Features;

/// <summary>
/// Marks a prop as being composed of discrete physical segments.
/// </summary>
[PropFeature(PropFeatureFlags.Segments)]
public interface IHasSegments
{
    /// <summary>Gets the ordered list of physical segments that make up the prop.</summary>
    /// <value>A read-only list of <see cref="Segment"/> instances.</value>
    IReadOnlyList<Segment> Segments { get; }

    /// <summary>Appends a segment to the prop's segment collection.</summary>
    /// <param name="segment">The segment to add.</param>
    void AddSegment(Segment segment);
}