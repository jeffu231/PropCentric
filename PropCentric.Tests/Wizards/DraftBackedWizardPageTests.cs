using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.PolyLine.Wizard.Pages;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Wizard.Pages;
using Vixen.Sys.Props;

namespace PropCentric.Tests.Wizards;

/// <summary>
/// Verifies that shared wizard page fields are sourced directly from drafts.
/// </summary>
public class DraftBackedWizardPageTests
{
    [Fact]
    public void TreePropWizardPage_UsesDraftValuesForSharedFields()
    {
        var draft = new TreePropDraft
        {
            Name = "Existing Tree",
            LightSize = 7,
            StringType = StringTypes.SingleColor
        };

        var page = new TreePropWizardPage(draft, new StubPreviewCoordinator<TreePropDraft>());

        Assert.Equal(draft.Name, page.Name);
        Assert.Equal(draft.LightSize, page.LightSize);
        Assert.Equal(draft.StringType, page.StringType);
    }

    [Fact]
    public void TreePropWizardPage_SharedFieldChanges_UpdateDraftImmediately()
    {
        var draft = new TreePropDraft();
        var page = new TreePropWizardPage(draft, new StubPreviewCoordinator<TreePropDraft>());

        page.Name = "Updated Tree";
        page.LightSize = 9;
        page.StringType = StringTypes.MultiColor;

        Assert.Equal("Updated Tree", draft.Name);
        Assert.Equal(9, draft.LightSize);
        Assert.Equal(StringTypes.MultiColor, draft.StringType);
    }

    [Fact]
    public void PolyLinePropWizardPage_UsesDraftValuesForSharedFields()
    {
        var draft = new PolyLinePropDraft
        {
            Name = "Existing PolyLine",
            LightSize = 5,
            StringType = StringTypes.SingleColor
        };

        var page = new PolyLinePropWizardPage(draft, new StubPreviewCoordinator<PolyLinePropDraft>());

        Assert.Equal(draft.Name, page.Name);
        Assert.Equal(draft.LightSize, page.LightSize);
        Assert.Equal(draft.StringType, page.StringType);
    }

    [Fact]
    public void PolyLinePropWizardPage_SharedFieldChanges_UpdateDraftImmediately()
    {
        var draft = new PolyLinePropDraft();
        var page = new PolyLinePropWizardPage(draft, new StubPreviewCoordinator<PolyLinePropDraft>());

        page.Name = "Updated PolyLine";
        page.LightSize = 6;
        page.StringType = StringTypes.MultiColor;

        Assert.Equal("Updated PolyLine", draft.Name);
        Assert.Equal(6, draft.LightSize);
        Assert.Equal(StringTypes.MultiColor, draft.StringType);
    }

    private sealed class StubPreviewCoordinator<TDraft> : IWizardPreviewCoordinator<TDraft>
        where TDraft : class, IPropDraft
    {
        public IPropVisualModel BuildPreview(TDraft draft)
        {
            return new StubPropVisualModel
            {
                Id = Guid.NewGuid(),
                Elements = []
            };
        }
    }

    private sealed class StubPropVisualModel : IPropVisualModel
    {
        public Guid Id { get; init; }

        public IReadOnlyList<IVisualElement> Elements { get; init; } = [];
    }
}
