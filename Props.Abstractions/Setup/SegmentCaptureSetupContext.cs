namespace Props.Abstractions.Setup;

/// <summary>
/// Context for transferring the captured segments into setup.
/// </summary>
/// <param name="Segments"></param>
/// <param name="Transform"></param>
public record SegmentCaptureSetupContext(IReadOnlyList<CapturedWorldSegment> Segments,
    WorldToModelTransform Transform) : IPropSetupContext;