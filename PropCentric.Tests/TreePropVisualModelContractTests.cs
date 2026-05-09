using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Props.Runtime.Tree;

namespace PropCentric.Tests;

/// <summary>
/// Verifies the remaining prop-to-visual-model contract expected by the design goals.
/// </summary>
public class TreePropVisualModelContractTests
{
    [Fact]
    public async Task TreeProp_VisualModel_AfterGeometryChange_RebuildsWithUpdatedGeometry()
    {
        var prop = TreeTestData.CreateTreeProp();

        var firstModel = Assert.IsType<TreePropVisualModel>(prop.PropVisualModel);
        var firstCloudCount = firstModel.Elements.OfType<LightPointCloud>().Count();

        prop.Strings = prop.Strings + 4;
        await prop.CommitAsync();

        var secondModel = Assert.IsType<TreePropVisualModel>(prop.PropVisualModel);
        var secondCloudCount = secondModel.Elements.OfType<LightPointCloud>().Count();

        Assert.NotSame(firstModel, secondModel);
        Assert.Equal(prop.Strings, secondCloudCount);
        Assert.NotEqual(firstCloudCount, secondCloudCount);
    }

    [Fact]
    public async Task IProp_PropVisualModel_ReturnsDerivedRenderableModel()
    {
        TreeProp prop = TreeTestData.CreateTreeProp();
        await prop.CommitAsync();
        IProp iProp = prop;

        var model = iProp.PropVisualModel;

        Assert.NotEmpty(model.Elements);
        Assert.Same(prop.PropVisualModel, model);
    }
}
