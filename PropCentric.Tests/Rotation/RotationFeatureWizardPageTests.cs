using Props.Abstractions.Features;
using System.Collections.ObjectModel;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Wizards.Features.Rotation.Pages;

namespace PropCentric.Tests.Rotation;

/// <summary>
/// Verifies the draft-backed rotation feature page behavior.
/// </summary>
public class RotationFeatureWizardPageTests
{
    [Fact]
    public void Initialize_BindsToSharedDraftRotations()
    {
        var draft = CreateDraft();
        var previewSession = new TestPreviewSession(draft);
        var page = new RotationFeatureWizardPage();

        page.Initialize(new FeatureWizardContext(draft, previewSession));

        Assert.Same(previewSession, page.PreviewSession);
        Assert.Equal(draft.AxisRotations.Count, page.Rotations.Count);
        Assert.Equal("X", page.Rotations[0].Axis);
        Assert.Equal(15, page.Rotations[0].RotationAngle);
        Assert.Contains("X: 15", page.RotationSummary);
    }

    [Fact]
    public void RotationEdits_UpdateSharedDraftImmediately()
    {
        var draft = CreateDraft();
        var page = new RotationFeatureWizardPage();

        page.Initialize(new FeatureWizardContext(draft, new TestPreviewSession(draft)));
        page.Rotations[0].RotationAngle = 42;
        page.Rotations[1].Axis = "Z";

        Assert.Equal(42, draft.AxisRotations[0].RotationAngle);
        Assert.Equal(Axis.ZAxis, draft.AxisRotations[1].Axis);
        Assert.Contains("X: 42", page.RotationSummary);
        Assert.Contains("Z: -30", page.RotationSummary);
    }

    private static TreePropDraft CreateDraft()
    {
        return new TreePropDraft
        {
            AxisRotations =
            [
                new AxisRotationModel { Axis = Axis.XAxis, RotationAngle = 15 },
                new AxisRotationModel { Axis = Axis.YAxis, RotationAngle = -30 },
                new AxisRotationModel { Axis = Axis.ZAxis, RotationAngle = 90 }
            ]
        };
    }

    private sealed class TestPreviewSession(TreePropDraft draft) : IWizardPreviewSession<TreePropDraft>
    {
        public TreePropDraft Draft => draft;

        Props.Abstractions.Setup.IPropDraft IWizardPreviewSession.Draft => Draft;

        public Task<IPropVisualModel> BuildPreviewAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IPropVisualModel>(new TreePropVisualModel { Elements = [] });
    }
}
