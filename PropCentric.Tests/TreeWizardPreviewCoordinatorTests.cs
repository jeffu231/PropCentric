using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Visuals;

namespace PropCentric.Tests;

/// <summary>
/// Verifies the current preview coordinator rebuild behavior.
/// </summary>
public class TreeWizardPreviewCoordinatorTests
{
    [Fact]
    public void TreeWizardPreviewCoordinator_BuildPreview_WithSameDraftValues_ReturnsCachedModel()
    {
        var coordinator = new TreeWizardPreviewCoordinator(
            new TreeDraftToVisualInputMapper(),
            new TreeVisualModelBuilder());
        var draft = new TreePropDraft
        {
            Strings = 8,
            NodesPerString = 10,
            AxisRotations = TreeTestData.CreateRotations((Props.Abstractions.PropVisualModels.Axis.XAxis, 0))
        };

        var first = coordinator.BuildPreview(draft);
        var second = coordinator.BuildPreview(draft);

        Assert.Same(first, second);
    }
    
    [Fact]
    public void TreeWizardPreviewCoordinator_BuildPreview_WithDifferentDraftValues_ReturnsDifferentModel()
    {
        var coordinator = new TreeWizardPreviewCoordinator(
            new TreeDraftToVisualInputMapper(),
            new TreeVisualModelBuilder());
        var draft = new TreePropDraft
        {
            Strings = 8,
            NodesPerString = 10,
            AxisRotations = TreeTestData.CreateRotations((Props.Abstractions.PropVisualModels.Axis.XAxis, 0))
        };
        
        var draft2 = new TreePropDraft
        {
            Strings = 16,
            NodesPerString = 50,
            AxisRotations = TreeTestData.CreateRotations((Props.Abstractions.PropVisualModels.Axis.XAxis, 0))
        };

        var first = coordinator.BuildPreview(draft);
        var second = coordinator.BuildPreview(draft2);

        Assert.NotSame(first, second);
    }
}
