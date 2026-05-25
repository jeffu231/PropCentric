using Props.Abstractions.Features;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Wizards.Features.Dimming.Pages;

namespace PropCentric.Tests.Dimming;

/// <summary>
/// Verifies the draft-backed dimming feature page behavior.
/// </summary>
public class DimmingFeatureWizardPageTests
{
    [Fact]
    public void Initialize_BindsToSharedDraftDimmingState()
    {
        var draft = new TreePropDraft
        {
            Brightness = 72.5,
            Gamma = 1.8
        };
        var page = new DimmingFeatureWizardPage();

        page.Initialize(new FeatureWizardContext(draft, new TestPreviewSession(draft)));

        Assert.Equal(72, page.Brightness);
        Assert.Equal(1.8, page.Gamma);
    }

    [Fact]
    public void DimmingEdits_UpdateSharedDraftImmediately()
    {
        var draft = new TreePropDraft();
        var page = new DimmingFeatureWizardPage();

        page.Initialize(new FeatureWizardContext(draft, new TestPreviewSession(draft)));
        page.Brightness = 64;
        page.Gamma = 2.4;

        Assert.Equal(64, draft.Brightness);
        Assert.Equal(2.4, draft.Gamma);
    }

    private sealed class TestPreviewSession(TreePropDraft draft) : IWizardPreviewSession<TreePropDraft>
    {
        public TreePropDraft Draft => draft;

        Props.Abstractions.Setup.IPropDraft IWizardPreviewSession.Draft => Draft;

        public Task<IPropVisualModel> BuildPreviewAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IPropVisualModel>(new TreePropVisualModel { Elements = [] });
    }
}
