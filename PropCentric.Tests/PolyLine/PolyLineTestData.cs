using System.Collections.ObjectModel;
using System.Numerics;
using Props.Abstractions.Props;
using Props.Runtime.PolyLine;
using Props.Runtime.PolyLine.Visuals;

namespace PropCentric.Tests.PolyLine;

public class PolyLineTestData
{
    /// <summary>
    /// Creates a configured <see cref="PolyLineProp"/> that can be reused across mapping and builder tests.
    /// </summary>
    /// <returns>A configured <see cref="PolyLineProp"/> instance.</returns>
    internal static PolyLineProp CreateTreeProp()
    {
        var prop = new PolyLineProp(new PolyLinePropToVisualInputMapper(), new PolyLineVisualModelBuilder())
        {
            Name = "Configured PolyLine",
            LightSize = 2
        };
        
        prop.ReplaceSegments(CreateSegments());

        return prop;
    }

    public static ObservableCollection<Segment> CreateSegments()
    {
        return new ObservableCollection<Segment>()
        {
            new Segment(new Vector2(20), new Vector2(30), 50),
            new Segment(new Vector2(25), new Vector2(35), 30),
        };
    }
}
