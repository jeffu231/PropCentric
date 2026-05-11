using System.Collections.ObjectModel;
using System.Numerics;

namespace Props.Abstractions.Features;

/// <summary>
/// Exposes mutable draft segment state for wizard pages that edit segmented props.
/// </summary>
public interface IHasSegmentsDraft
{
    /// <summary>Gets the ordered mutable draft segments for the current wizard flow.</summary>
    ObservableCollection<SegmentDraftState> Segments { get; }
}

/// <summary>
/// Represents a single editable segment entry in shared wizard draft state.
/// </summary>
public sealed class SegmentDraftState
{
    /// <summary>Gets or sets the segment start coordinate in normalized model space.</summary>
    public Vector2 Start { get; set; }

    /// <summary>Gets or sets the segment end coordinate in normalized model space.</summary>
    public Vector2 End { get; set; }

    /// <summary>Gets or sets the number of individually addressable points along the segment.</summary>
    public int PointCount { get; set; }
}
