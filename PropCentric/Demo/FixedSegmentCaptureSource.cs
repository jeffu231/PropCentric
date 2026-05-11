using System.Numerics;
using Props.Abstractions.Setup;

namespace PropCentric.Demo;

/// <summary>
/// Supplies deterministic captured segment data for harness-only polyline setup demos.
/// </summary>
internal static class FixedSegmentCaptureSource
{
    public static SegmentCaptureSetupContext CreateInitialCaptureContext()
    {
        return new SegmentCaptureSetupContext(
        [
            new CapturedWorldSegment(new Vector2(10f, 10f), new Vector2(40f, 10f), 20),
            new CapturedWorldSegment(new Vector2(40f, 10f), new Vector2(65f, 30f), 16),
            new CapturedWorldSegment(new Vector2(65f, 30f), new Vector2(90f, 25f), 14)
        ],
            new WorldToModelTransform(new Vector2(0f, 0f), new Vector2(100f, 50f)));
    }

    public static SegmentCaptureSetupContext CreateRecaptureContext()
    {
        return new SegmentCaptureSetupContext(
        [
            new CapturedWorldSegment(new Vector2(15f, 15f), new Vector2(45f, 15f), 18),
            new CapturedWorldSegment(new Vector2(45f, 15f), new Vector2(70f, 35f), 18),
            new CapturedWorldSegment(new Vector2(70f, 35f), new Vector2(88f, 18f), 12)
        ],
            new WorldToModelTransform(new Vector2(0f, 0f), new Vector2(100f, 50f)));
    }
}
