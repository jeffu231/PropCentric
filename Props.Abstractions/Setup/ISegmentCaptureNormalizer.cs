using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

/// <summary>
/// Normalizes captured world-space segments into model-space prop segments.
/// </summary>
public interface ISegmentCaptureNormalizer
{
    /// <summary>
    /// Converts captured world-space segment geometry into normalized model-space segments.
    /// </summary>
    /// <param name="context">The captured segments and transform to normalize.</param>
    /// <returns>The ordered normalized segments.</returns>
    IReadOnlyList<Segment> Normalize(SegmentCaptureSetupContext context);
}
