using Vixen.Core;

namespace Props.Abstractions.Features;

[PropFeature(PropFeatureFlags.Segments)]
public interface IHasSegments
{
    IReadOnlyList<Segment> Segments { get; }
    void AddSegment(Segment segment);
}