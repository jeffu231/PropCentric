using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Props;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Visuals;

namespace PropCentric.Tests;

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
            DegreesCoverage: 180,
            DegreeOffset: 0,
            BaseHeight: 40,
            TopHeight: 20,
            TopWidth: 20,
            StartLocation: StartLocation.BottomLeft,
            TopRadius: 10,
            BottomRadius: 100,
            AxisRotations: TreeTestData.CreateRotations((Axis.XAxis, 0), (Axis.YAxis, 0), (Axis.ZAxis, 0)));

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
            DegreesCoverage: 360,
            DegreeOffset: 0,
            BaseHeight: 40,
            TopHeight: 20,
            TopWidth: 20,
            StartLocation: StartLocation.BottomLeft,
            TopRadius: 10,
            BottomRadius: 100,
            AxisRotations: TreeTestData.CreateRotations((Axis.XAxis, 0)));
        var offsetInput = baseInput with { DegreeOffset = 90 };

        var baseModel = Assert.IsType<TreePropVisualModel>(builder.Create(baseInput));
        var offsetModel = Assert.IsType<TreePropVisualModel>(builder.Create(offsetInput));

        Assert.NotEqual(baseModel.StartingLightPoint?.Position, offsetModel.StartingLightPoint?.Position);
    }

    [Fact]
    public void TreeVisualModelBuilder_Create_WithRotationInput_PersistsAxisRotationsOnReturnedModel()
    {
        var builder = new TreeVisualModelBuilder();
        var rotations = TreeTestData.CreateRotations((Axis.XAxis, 15), (Axis.YAxis, 25), (Axis.ZAxis, 35));
        var input = new TreeVisualInput(
            Strings: 4,
            NodesPerString: 5,
            LightSize: 2,
            DegreesCoverage: 360,
            DegreeOffset: 0,
            BaseHeight: 40,
            TopHeight: 20,
            TopWidth: 20,
            StartLocation: StartLocation.BottomLeft,
            TopRadius: 10,
            BottomRadius: 100,
            AxisRotations: rotations);

        var model = Assert.IsType<TreePropVisualModel>(builder.Create(input));

        Assert.NotNull(model.AxisRotations);
        Assert.Equal(rotations.Count, model.AxisRotations.Count);
        for (int i = 0; i < rotations.Count; i++)
        {
            Assert.Equal(rotations[i].Axis, model.AxisRotations[i].Axis);
            Assert.Equal(rotations[i].RotationAngle, model.AxisRotations[i].RotationAngle);
        }
    }

    [Fact]
    public void TreeVisualModelBuilder_Create_WithRotationInput_TransformsPointPositions()
    {
        var builder = new TreeVisualModelBuilder();
        var baseInput = new TreeVisualInput(
            Strings: 4,
            NodesPerString: 5,
            LightSize: 2,
            DegreesCoverage: 360,
            DegreeOffset: 0,
            BaseHeight: 40,
            TopHeight: 20,
            TopWidth: 20,
            StartLocation: StartLocation.BottomLeft,
            TopRadius: 10,
            BottomRadius: 100,
            AxisRotations: TreeTestData.CreateRotations((Axis.ZAxis, 0)));
        var rotatedInput = baseInput with
        {
            AxisRotations = TreeTestData.CreateRotations((Axis.ZAxis, 90))
        };

        var baseModel = Assert.IsType<TreePropVisualModel>(builder.Create(baseInput));
        var rotatedModel = Assert.IsType<TreePropVisualModel>(builder.Create(rotatedInput));

        Assert.NotEqual(baseModel.StartingLightPoint?.Position, rotatedModel.StartingLightPoint?.Position);
    }
}
