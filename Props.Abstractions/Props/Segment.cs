using System.Drawing;

namespace Props.Abstractions.Props;

/// <summary>
/// Represents a single physical segment of a prop, defined by two endpoints and a light count.
/// </summary>
public class Segment
{
    /// <summary>Gets or sets the starting endpoint of the segment in prop-space.</summary>
    /// <value>The 2-D coordinate of the segment's start point.</value>
    public Point Start { get; set; }

    /// <summary>Gets or sets the ending endpoint of the segment in prop-space.</summary>
    /// <value>The 2-D coordinate of the segment's end point.</value>
    public Point End { get; set; }

    /// <summary>Gets or sets the number of individually addressable light points along the segment.</summary>
    /// <value>A non-negative integer representing the light count.</value>
    public int PointCount { get; set; }
}