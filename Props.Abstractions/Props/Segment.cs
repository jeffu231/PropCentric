using System.Drawing;

namespace Props.Abstractions.Props;

public class Segment
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public int PointCount { get; set; }
}