using System.Drawing;
using PropCentric.Tests.Common;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Visuals;

namespace PropCentric.Tests.Tree;

/// <summary>
/// Verifies that tree setup data moves correctly between draft, prop, and visual input layers.
/// </summary>
public class TreeDraftMappingTests
{
    [Fact]
    public void TreePropDraftMapper_PopulateDraft_CopiesConfiguredFields()
    {
        var prop = TreeTestData.CreateTreeProp();
        var mapper = new TreePropDraftMapper();
        var draft = new TreePropDraft();

        mapper.PopulateDraft(draft, prop);

        Assert.Equal(prop.Name, draft.Name);
        Assert.Equal(prop.Strings, draft.Strings);
        Assert.Equal(prop.NodesPerString, draft.NodesPerString);
        Assert.Equal(prop.LightSize, draft.LightSize);
        Assert.Equal(prop.Brightness, draft.Brightness);
        Assert.Equal(prop.Gamma, draft.Gamma);
        AssertColorConfigurationsEqual(prop.ColorConfiguration, draft.ColorConfiguration);
        Assert.Equal(prop.DegreesCoverage, draft.DegreesCoverage);
        Assert.Equal(prop.DegreeOffset, draft.DegreeOffset);
        Assert.Equal(prop.BaseHeight, draft.BaseHeight);
        Assert.Equal(prop.TopHeight, draft.TopHeight);
        Assert.Equal(prop.TopWidth, draft.TopWidth);
        Assert.Equal(prop.StartLocation, draft.StartLocation);
        Assert.Equal(prop.ZigZag, draft.ZigZag);
        Assert.Equal(prop.ZigZagOffset, draft.ZigZagOffset);
        Assert.Equal(prop.TopRadius, draft.TopRadius);
        Assert.Equal(prop.BottomRadius, draft.BottomRadius);
        AssertRotationsEqual(prop.AxisRotations, draft.AxisRotations);
        AssertRotationInstancesAreDistinct(prop.AxisRotations, draft.AxisRotations);
    }

    [Fact]
    public void TreePropDraftMapper_ApplyDraft_CopiesConfiguredFieldsBackToProp()
    {
        var prop = TreeTestData.CreateTreeProp();
        var mapper = new TreePropDraftMapper();
        var draft = new TreePropDraft
        {
            Name = "Tree Draft",
            Strings = 24,
            NodesPerString = 75,
            LightSize = 3,
            Brightness = 72.5,
            Gamma = 1.8,
            ColorConfiguration = new LightColorConfiguration(
                LightType.SingleColor,
                Color.Orange,
                new DiscreteColorSetDefinition("RGB", [Color.Red, Color.Green, Color.Blue]),
                new FullColorOrderDefinition(
                    "BGR",
                    [LightColorChannel.Blue, LightColorChannel.Green, LightColorChannel.Red])),
            DegreesCoverage = 180,
            DegreeOffset = 45,
            BaseHeight = 30,
            TopHeight = 10,
            TopWidth = 15,
            StartLocation = StartLocation.TopRight,
            ZigZag = true,
            ZigZagOffset = 12,
            TopRadius = 22,
            BottomRadius = 88,
            AxisRotations = TestDataHelper.CreateRotations((Axis.XAxis, 10), (Axis.YAxis, 20), (Axis.ZAxis, 30))
        };

        mapper.ApplyDraft(draft, prop);

        Assert.Equal(draft.Name, prop.Name);
        Assert.Equal(draft.Strings, prop.Strings);
        Assert.Equal(draft.NodesPerString, prop.NodesPerString);
        Assert.Equal(draft.LightSize, prop.LightSize);
        Assert.Equal(draft.Brightness, prop.Brightness);
        Assert.Equal(draft.Gamma, prop.Gamma);
        AssertColorConfigurationsEqual(draft.ColorConfiguration, prop.ColorConfiguration);
        Assert.Equal(draft.DegreesCoverage, prop.DegreesCoverage);
        Assert.Equal(draft.DegreeOffset, prop.DegreeOffset);
        Assert.Equal(draft.BaseHeight, prop.BaseHeight);
        Assert.Equal(draft.TopHeight, prop.TopHeight);
        Assert.Equal(draft.TopWidth, prop.TopWidth);
        Assert.Equal(draft.StartLocation, prop.StartLocation);
        Assert.Equal(draft.ZigZag, prop.ZigZag);
        Assert.Equal(draft.ZigZagOffset, prop.ZigZagOffset);
        Assert.Equal(draft.TopRadius, prop.TopRadius);
        Assert.Equal(draft.BottomRadius, prop.BottomRadius);
        AssertRotationsEqual(draft.AxisRotations, prop.AxisRotations);
        AssertRotationInstancesAreDistinct(draft.AxisRotations, prop.AxisRotations);
    }

    [Fact]
    public void TreeDraftToVisualInputMapper_Map_ProjectsDraftIntoVisualInput()
    {
        var mapper = new TreeDraftToVisualInputMapper();
        var draft = new TreePropDraft
        {
            Strings = 12,
            NodesPerString = 30,
            LightSize = 4,
            ColorConfiguration = new LightColorConfiguration(
                LightType.SingleColor,
                Color.DeepPink,
                new DiscreteColorSetDefinition("RGB", [Color.Red, Color.Green, Color.Blue]),
                new FullColorOrderDefinition(
                    "RGB",
                    [LightColorChannel.Red, LightColorChannel.Green, LightColorChannel.Blue])),
            DegreesCoverage = 270,
            DegreeOffset = 15,
            BaseHeight = 25,
            TopHeight = 8,
            TopWidth = 11,
            StartLocation = StartLocation.BottomRight,
            TopRadius = 9,
            BottomRadius = 70,
            AxisRotations = TestDataHelper.CreateRotations((Axis.XAxis, 5), (Axis.ZAxis, 15))
        };

        TreeVisualInput input = mapper.Map(draft);

        Assert.Equal(draft.Strings, input.Strings);
        Assert.Equal(draft.NodesPerString, input.NodesPerString);
        Assert.Equal(draft.LightSize, input.LightSize);
        Assert.Equal(draft.DegreesCoverage, input.DegreesCoverage);
        Assert.Equal(draft.DegreeOffset, input.DegreeOffset);
        Assert.Equal(draft.TopRadius, input.TopRadius);
        Assert.Equal(draft.BottomRadius, input.BottomRadius);
        AssertRotationsEqual(draft.AxisRotations, input.AxisRotations);
        AssertRotationInstancesAreDistinct(draft.AxisRotations, input.AxisRotations);
    }

    [Fact]
    public void TreePropToVisualInputMapper_Map_ProjectsPropIntoVisualInputWithRotationSnapshot()
    {
        var mapper = new TreePropToVisualInputMapper();
        var prop = TreeTestData.CreateTreeProp();

        TreeVisualInput input = mapper.Map(prop);

        Assert.Equal(prop.Strings, input.Strings);
        Assert.Equal(prop.NodesPerString, input.NodesPerString);
        Assert.Equal(prop.LightSize, input.LightSize);
        Assert.Equal(prop.DegreesCoverage, input.DegreesCoverage);
        Assert.Equal(prop.DegreeOffset, input.DegreeOffset);
        Assert.Equal(prop.TopRadius, input.TopRadius);
        Assert.Equal(prop.BottomRadius, input.BottomRadius);
        AssertRotationsEqual(prop.AxisRotations, input.AxisRotations);
        AssertRotationInstancesAreDistinct(prop.AxisRotations, input.AxisRotations);
    }

    private static void AssertRotationsEqual(
        IReadOnlyList<AxisRotationModel> expected,
        IReadOnlyList<AxisRotationModel> actual)
    {
        Assert.Equal(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Axis, actual[i].Axis);
            Assert.Equal(expected[i].RotationAngle, actual[i].RotationAngle);
        }
    }

    private static void AssertRotationInstancesAreDistinct(
        IReadOnlyList<AxisRotationModel> expected,
        IReadOnlyList<AxisRotationModel> actual)
    {
        Assert.NotSame(expected, actual);

        for (var index = 0; index < expected.Count; index++)
        {
            Assert.NotSame(expected[index], actual[index]);
        }
    }

    private static void AssertColorConfigurationsEqual(
        LightColorConfiguration expected,
        LightColorConfiguration actual)
    {
        Assert.Equal(expected.LightType, actual.LightType);
        Assert.Equal(expected.SingleColor, actual.SingleColor);
        Assert.Equal(expected.DiscreteColorSet?.Name, actual.DiscreteColorSet?.Name);
        Assert.Equal(expected.FullColorOrder?.Name, actual.FullColorOrder?.Name);

        Assert.Equal(expected.DiscreteColorSet?.Colors.Count ?? 0, actual.DiscreteColorSet?.Colors.Count ?? 0);
        if (expected.DiscreteColorSet is not null && actual.DiscreteColorSet is not null)
        {
            for (int i = 0; i < expected.DiscreteColorSet.Colors.Count; i++)
            {
                Assert.Equal(expected.DiscreteColorSet.Colors[i], actual.DiscreteColorSet.Colors[i]);
            }
        }

        Assert.Equal(expected.FullColorOrder?.Channels.Count ?? 0, actual.FullColorOrder?.Channels.Count ?? 0);
        if (expected.FullColorOrder is not null && actual.FullColorOrder is not null)
        {
            for (int i = 0; i < expected.FullColorOrder.Channels.Count; i++)
            {
                Assert.Equal(expected.FullColorOrder.Channels[i], actual.FullColorOrder.Channels[i]);
            }
        }
    }
}
