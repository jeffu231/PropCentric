using System.Numerics;
using PropCentric.Tests.Common;
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
            LightSize: 3,
            AxisRotations: TestDataHelper.CreateRotations((Axis.XAxis, 0)));

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
            LightSize: 2,
            AxisRotations: TestDataHelper.CreateRotations((Axis.XAxis, 0)));

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
            LightSize: 2,
            AxisRotations: TestDataHelper.CreateRotations((Axis.XAxis, 0)));

        var model = Assert.IsType<PolyLinePropVisualModel>(builder.Create(input));
        var segments = model.Elements.OfType<LightSegment>().ToList();

        Assert.Equal(new Vector3(1f, 0f, 0f), segments[0].Lights[^1].Position);
        Assert.Equal(2, segments[1].Lights.Count);
        Assert.Equal(new Vector3(1f, 0.5f, 0f), segments[1].Lights[0].Position);
        Assert.DoesNotContain(segments[1].Lights, light => light.Position == new Vector3(1f, 0f, 0f));
    }

    [Fact]
    public void Create_WithRotationInput_TransformsSegmentAndLightPositions()
    {
        var builder = new PolyLineVisualModelBuilder();
        var baseInput = new PolyLineVisualInput(
        [
            new Segment(new Vector2(0f, 0f), new Vector2(1f, 0f), 3)
        ],
            LightSize: 2,
            AxisRotations: TestDataHelper.CreateRotations((Axis.ZAxis, 0)));
        var rotatedInput = baseInput with
        {
            AxisRotations = TestDataHelper.CreateRotations((Axis.ZAxis, 90))
        };

        var baseModel = Assert.IsType<PolyLinePropVisualModel>(builder.Create(baseInput));
        var rotatedModel = Assert.IsType<PolyLinePropVisualModel>(builder.Create(rotatedInput));
        var baseSegment = Assert.IsType<LightSegment>(Assert.Single(baseModel.Elements));
        var rotatedSegment = Assert.IsType<LightSegment>(Assert.Single(rotatedModel.Elements));

        Assert.NotEqual(baseSegment.End, rotatedSegment.End);
        AssertVectorEqual(new Vector3(0f, 1f, 0f), rotatedSegment.End);
        Assert.NotEqual(baseSegment.Lights[1].Position, rotatedSegment.Lights[1].Position);
    }

    private static void AssertVectorEqual(Vector3 expected, Vector3 actual, float tolerance = 1e-5f)
    {
        Assert.True(MathF.Abs(expected.X - actual.X) <= tolerance);
        Assert.True(MathF.Abs(expected.Y - actual.Y) <= tolerance);
        Assert.True(MathF.Abs(expected.Z - actual.Z) <= tolerance);
    }
}
