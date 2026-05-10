using System.Numerics;
using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

/// <summary>
/// Normalizes captured world-space segment geometry into ordered model-space segments.
/// </summary>
public sealed class SegmentCaptureNormalizer : ISegmentCaptureNormalizer
{
    private const float RelativeTolerance = 1e-4f;

    /// <inheritdoc />
    public IReadOnlyList<Segment> Normalize(SegmentCaptureSetupContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Segments.Count == 0)
        {
            throw new ArgumentException("At least one captured segment is required.", nameof(context));
        }

        var worldMin = context.Transform.WorldMin;
        var worldMax = context.Transform.WorldMax;
        var width = worldMax.X - worldMin.X;
        var height = worldMax.Y - worldMin.Y;

        if (width <= 0f || height <= 0f)
        {
            throw new ArgumentException(
                "World-to-model transform must define a non-zero positive width and height.",
                nameof(context));
        }

        var continuityTolerance = MathF.Max(width, height) * RelativeTolerance;
        Segment? previousNormalizedSegment = null;
        var normalizedSegments = new List<Segment>(context.Segments.Count);

        for (var index = 0; index < context.Segments.Count; index++)
        {
            var capturedSegment = context.Segments[index];

            if (capturedSegment.PointCount <= 0)
            {
                throw new ArgumentException(
                    $"Captured segment at index {index} must have a positive point count.",
                    nameof(context));
            }

            if (Vector2.Distance(capturedSegment.Start, capturedSegment.End) <= continuityTolerance)
            {
                throw new ArgumentException(
                    $"Captured segment at index {index} must have non-zero length.",
                    nameof(context));
            }

            if (index > 0)
            {
                var previousCapturedSegment = context.Segments[index - 1];
                if (Vector2.Distance(previousCapturedSegment.End, capturedSegment.Start) > continuityTolerance)
                {
                    throw new ArgumentException(
                        $"Captured segments at indexes {index - 1} and {index} are not continuous.",
                        nameof(context));
                }
            }

            var normalizedSegment = new Segment(
                NormalizePoint(capturedSegment.Start, worldMin, width, height),
                NormalizePoint(capturedSegment.End, worldMin, width, height),
                capturedSegment.PointCount);

            if (previousNormalizedSegment is not null &&
                Vector2.Distance(previousNormalizedSegment.End, normalizedSegment.Start) > RelativeTolerance)
            {
                throw new ArgumentException(
                    $"Normalized segments at indexes {index - 1} and {index} are not continuous.",
                    nameof(context));
            }

            normalizedSegments.Add(normalizedSegment);
            previousNormalizedSegment = normalizedSegment;
        }

        return normalizedSegments;
    }

    private static Vector2 NormalizePoint(Vector2 point, Vector2 worldMin, float width, float height)
    {
        return new Vector2(
            (point.X - worldMin.X) / width,
            (point.Y - worldMin.Y) / height);
    }
}
