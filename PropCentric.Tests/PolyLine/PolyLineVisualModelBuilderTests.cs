using System.Numerics;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Props;
using Props.Runtime.PolyLine.Visuals;

namespace PropCentric.Tests.PolyLine;

/// <summary>
/// Verifies polyline visual-model generation behavior.
/// </summary>
public class PolyLineVisualModelBuilderTests
{
    [Fact]
    public void Create_BuildsOneLightSegmentPerLogicalSegment()
    {
        var builder = new PolyLineVisualModelBuilder();
        var input = new PolyLineVisualInput(
            PolyLineTestData.CreateSegments(),
            LightSize: 3);

        var model = Assert.IsType<PolyLinePropVisualModel>(builder.Create(input));
        var segments = model.Elements.OfType<LightSegment>().ToList();

        Assert.Equal(input.Segments.Count, segments.Count);
        Assert.NotNull(model.StartingLightPoint);
    }

    [Fact]
    public void Create_UsesConfiguredPointCountsPerSegment()
    {
        var builder = new PolyLineVisualModelBuilder();
        var input = new PolyLineVisualInput(
        [
            new Segment(new Vector2(0f, 0f), new Vector2(1f, 0f), 4),
            new Segment(new Vector2(1f, 0f), new Vector2(1f, 1f), 3)
        ],
            LightSize: 2);

        var model = Assert.IsType<PolyLinePropVisualModel>(builder.Create(input));
        var segments = model.Elements.OfType<LightSegment>().ToList();

        Assert.Equal(4, segments[0].Lights.Count);
        Assert.Equal(2, segments[1].Lights.Count);
        Assert.All(segments.SelectMany(segment => segment.Lights), light => Assert.Equal(2f, light.PointSize));
    }

    [Fact]
    public void Create_DeduplicatesSharedCornerBetweenAdjacentSegments()
    {
        var builder = new PolyLineVisualModelBuilder();
        var input = new PolyLineVisualInput(
        [
            new Segment(new Vector2(0f, 0f), new Vector2(1f, 0f), 3),
            new Segment(new Vector2(1f, 0f), new Vector2(1f, 1f), 3)
        ],
            LightSize: 2);

        var model = Assert.IsType<PolyLinePropVisualModel>(builder.Create(input));
        var segments = model.Elements.OfType<LightSegment>().ToList();

        Assert.Equal(new Vector3(1f, 0f, 0f), segments[0].Lights[^1].Position);
        Assert.Equal(2, segments[1].Lights.Count);
        Assert.Equal(new Vector3(1f, 0.5f, 0f), segments[1].Lights[0].Position);
        Assert.DoesNotContain(segments[1].Lights, light => light.Position == new Vector3(1f, 0f, 0f));
    }

}
