using PropCentric.Tests.Common;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Visuals;

namespace PropCentric.Tests.Tree;

/// <summary>
/// Verifies the current preview coordinator rebuild behavior.
/// </summary>
public class TreeWizardPreviewCoordinatorTests
{
    [Fact]
    public async Task TreeWizardPreviewCoordinator_BuildPreviewAsync_WithSameDraftValues_ReturnsCachedModel()
    {
        var coordinator = new TreeWizardPreviewCoordinator(
            new TreeDraftToVisualInputMapper(),
            new TreeVisualModelBuilder());
        var draft = new TreePropDraft
        {
            Strings = 8,
            NodesPerString = 10,
            AxisRotations = TestDataHelper.CreateRotations((Props.Abstractions.PropVisualModels.Axis.XAxis, 0))
        };

        var first = await coordinator.BuildPreviewAsync(draft);
        var second = await coordinator.BuildPreviewAsync(draft);

        Assert.Same(first, second);
    }
    
    [Fact]
    public async Task TreeWizardPreviewCoordinator_BuildPreviewAsync_WithDifferentDraftValues_ReturnsDifferentModel()
    {
        var coordinator = new TreeWizardPreviewCoordinator(
            new TreeDraftToVisualInputMapper(),
            new TreeVisualModelBuilder());
        var draft = new TreePropDraft
        {
            Strings = 8,
            NodesPerString = 10,
            AxisRotations = TestDataHelper.CreateRotations((Props.Abstractions.PropVisualModels.Axis.XAxis, 0))
        };
        
        var draft2 = new TreePropDraft
        {
            Strings = 16,
            NodesPerString = 50,
            AxisRotations = TestDataHelper.CreateRotations((Props.Abstractions.PropVisualModels.Axis.XAxis, 0))
        };

        var first = await coordinator.BuildPreviewAsync(draft);
        var second = await coordinator.BuildPreviewAsync(draft2);

        Assert.NotSame(first, second);
    }

    [Fact]
    public async Task TreeWizardPreviewCoordinator_BuildPreviewAsync_WhenRotationAngleChangesOnSameDraft_RebuildsModel()
    {
        var coordinator = new TreeWizardPreviewCoordinator(
            new TreeDraftToVisualInputMapper(),
            new TreeVisualModelBuilder());
        var draft = new TreePropDraft
        {
            Strings = 8,
            NodesPerString = 10,
            AxisRotations = TestDataHelper.CreateRotations((Props.Abstractions.PropVisualModels.Axis.ZAxis, 0))
        };

        var first = await coordinator.BuildPreviewAsync(draft);

        draft.AxisRotations[0].RotationAngle = 90;

        var second = await coordinator.BuildPreviewAsync(draft);

        Assert.NotSame(first, second);
    }
}
