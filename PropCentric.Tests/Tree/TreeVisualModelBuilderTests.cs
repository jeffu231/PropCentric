using System.Numerics;
using PropCentric.Tests.Common;
using Props.Abstractions.PropVisualModels;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Visuals;
using Vixen.Sys.Props;

namespace PropCentric.Tests.Tree;

/// <summary>
/// Verifies the current tree visual-model generation path independent of any WPF-hosted preview surface.
/// </summary>
public class TreeVisualModelBuilderTests
{
    [Fact]
    public void TreeVisualModelBuilder_Create_BuildsExpectedCloudAndPointCounts()
    {
        var builder = new TreeVisualModelBuilder();
        var input = new TreeVisualInput(
            Strings: 4,
            NodesPerString: 6,
            LightSize: 3,
            StringTypes.ColorMixingRGB,
            DegreesCoverage: 180,
            DegreeOffset: 0,
            TopRadius: 10,
            BottomRadius: 100,
            AxisRotations: TestDataHelper.CreateRotations((Axis.XAxis, 0), (Axis.YAxis, 0), (Axis.ZAxis, 0)));

        var model = Assert.IsType<TreePropVisualModel>(builder.Create(input));
        var clouds = model.Elements.OfType<LightPointCloud>().ToList();

        Assert.Equal(4, clouds.Count);
        Assert.All(clouds, cloud => Assert.Equal(6, cloud.Points.Count));
        Assert.All(clouds.SelectMany(cloud => cloud.Points), point => Assert.Equal(3, point.PointSize));
        Assert.NotNull(model.StartingLightPoint);
    }

    [Fact]
    public void TreeVisualModelBuilder_Create_WithDifferentDegreeOffset_ChangesFirstPointPosition()
    {
        var builder = new TreeVisualModelBuilder();
        var baseInput = new TreeVisualInput(
            Strings: 4,
            NodesPerString: 5,
            LightSize: 2,
            StringTypes.ColorMixingRGB,
            DegreesCoverage: 360,
            DegreeOffset: 0,
            TopRadius: 10,
            BottomRadius: 100,
            AxisRotations: TestDataHelper.CreateRotations((Axis.XAxis, 0)));
        var offsetInput = baseInput with { DegreeOffset = 90 };

        var baseModel = Assert.IsType<TreePropVisualModel>(builder.Create(baseInput));
        var offsetModel = Assert.IsType<TreePropVisualModel>(builder.Create(offsetInput));

        Assert.NotEqual(baseModel.StartingLightPoint?.Position, offsetModel.StartingLightPoint?.Position);
    }

    [Fact]
    public void TreeVisualModelBuilder_Create_WithRotationInput_TransformsPointPositions()
    {
        var builder = new TreeVisualModelBuilder();
        var baseInput = new TreeVisualInput(
            Strings: 4,
            NodesPerString: 5,
            LightSize: 2,
            StringTypes.ColorMixingRGB,
            DegreesCoverage: 360,
            DegreeOffset: 0,
            TopRadius: 10,
            BottomRadius: 100,
            AxisRotations: TestDataHelper.CreateRotations((Axis.ZAxis, 0)));
        var rotatedInput = baseInput with
        {
            AxisRotations = TestDataHelper.CreateRotations((Axis.ZAxis, 90))
        };

        var baseModel = Assert.IsType<TreePropVisualModel>(builder.Create(baseInput));
        var rotatedModel = Assert.IsType<TreePropVisualModel>(builder.Create(rotatedInput));
        var baseCloud = Assert.IsType<LightPointCloud>(baseModel.Elements.First());
        var rotatedCloud = Assert.IsType<LightPointCloud>(rotatedModel.Elements.First());

        Assert.NotEqual(baseModel.StartingLightPoint?.Position, rotatedModel.StartingLightPoint?.Position);
        AssertVectorEqual(new Vector3(0.5f, 0.5f, 0f), rotatedModel.StartingLightPoint!.Value.Position);
        Assert.NotEqual(baseCloud.Points[0].Position, rotatedCloud.Points[0].Position);
    }

    private static void AssertVectorEqual(Vector3 expected, Vector3 actual, float tolerance = 1e-5f)
    {
        Assert.True(MathF.Abs(expected.X - actual.X) <= tolerance);
        Assert.True(MathF.Abs(expected.Y - actual.Y) <= tolerance);
        Assert.True(MathF.Abs(expected.Z - actual.Z) <= tolerance);
    }
}
