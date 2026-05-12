using System.Numerics;
using Props.Abstractions.Setup;

namespace PropCentric.Tests.Segments;

/// <summary>
/// Verifies captured segment normalization behavior.
/// </summary>
public class SegmentCaptureNormalizerTests
{
    private readonly ISegmentCaptureNormalizer _normalizer = new SegmentCaptureNormalizer();

    [Fact]
    public void Normalize_ValidContext_ReturnsNormalizedOrderedSegments()
    {
        var context = new SegmentCaptureSetupContext(
        [
            new CapturedWorldSegment(new Vector2(10f, 20f), new Vector2(20f, 20f), 5),
            new CapturedWorldSegment(new Vector2(20f, 20f), new Vector2(20f, 40f), 7)
        ],
            new WorldToModelTransform(new Vector2(10f, 20f), new Vector2(30f, 60f)));

        var segments = _normalizer.Normalize(context);

        Assert.Collection(
            segments,
            segment =>
            {
                Assert.Equal(new Vector2(0f, 0f), segment.Start);
                Assert.Equal(new Vector2(0.5f, 0f), segment.End);
                Assert.Equal(5, segment.PointCount);
            },
            segment =>
            {
                Assert.Equal(new Vector2(0.5f, 0f), segment.Start);
                Assert.Equal(new Vector2(0.5f, 0.5f), segment.End);
                Assert.Equal(7, segment.PointCount);
            });
    }

    [Fact]
    public void Normalize_EmptySegments_ThrowsArgumentException()
    {
        var context = new SegmentCaptureSetupContext(
            [],
            new WorldToModelTransform(Vector2.Zero, Vector2.One));

        Assert.Throws<ArgumentException>(() => _normalizer.Normalize(context));
    }

    [Fact]
    public void Normalize_NonPositivePointCount_ThrowsArgumentException()
    {
        var context = new SegmentCaptureSetupContext(
        [
            new CapturedWorldSegment(Vector2.Zero, Vector2.UnitX, 0)
        ],
            new WorldToModelTransform(Vector2.Zero, Vector2.One));

        Assert.Throws<ArgumentException>(() => _normalizer.Normalize(context));
    }

    [Fact]
    public void Normalize_ZeroSizeTransform_ThrowsArgumentException()
    {
        var context = new SegmentCaptureSetupContext(
        [
            new CapturedWorldSegment(Vector2.Zero, Vector2.UnitX, 3)
        ],
            new WorldToModelTransform(Vector2.Zero, new Vector2(0f, 1f)));

        Assert.Throws<ArgumentException>(() => _normalizer.Normalize(context));
    }

    [Fact]
    public void Normalize_ZeroLengthSegment_ThrowsArgumentException()
    {
        var context = new SegmentCaptureSetupContext(
        [
            new CapturedWorldSegment(Vector2.One, Vector2.One, 3)
        ],
            new WorldToModelTransform(Vector2.Zero, new Vector2(2f, 2f)));

        Assert.Throws<ArgumentException>(() => _normalizer.Normalize(context));
    }

    [Fact]
    public void Normalize_DiscontinuousSegments_ThrowsArgumentException()
    {
        var context = new SegmentCaptureSetupContext(
        [
            new CapturedWorldSegment(Vector2.Zero, Vector2.UnitX, 3),
            new CapturedWorldSegment(new Vector2(2f, 0f), new Vector2(3f, 0f), 4)
        ],
            new WorldToModelTransform(Vector2.Zero, new Vector2(3f, 1f)));

        Assert.Throws<ArgumentException>(() => _normalizer.Normalize(context));
    }
}
