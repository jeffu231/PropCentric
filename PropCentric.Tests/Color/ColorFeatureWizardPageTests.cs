using System.Drawing;
using Props.Abstractions.Features;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Registry;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Wizards.Features.Color.Pages;

namespace PropCentric.Tests.ColorFeature;

/// <summary>
/// Verifies the draft-backed color feature page behavior.
/// </summary>
public class ColorFeatureWizardPageTests
{
    [Fact]
    public void Initialize_BindsToSharedDraftConfiguration()
    {
        var draft = CreateDraft();
        var catalog = new InMemoryColorConfigurationCatalog();
        var page = new ColorFeatureWizardPage(catalog);
        var previewSession = new TestPreviewSession(draft);

        page.Initialize(draft, previewSession);

        Assert.Same(previewSession, page.PreviewSession);
        Assert.Equal(LightType.MultipleDiscreteColors, page.LightType);
        Assert.Equal("RGBW", page.SelectedDiscreteColorSet?.Name);
        Assert.Equal(4, page.WorkingDiscreteColors.Count);
        Assert.Equal("#FFFFFF", page.SingleColorHex);
    }

    [Fact]
    public void SingleColorEdit_UpdatesSharedDraftImmediately()
    {
        var draft = CreateDraft();
        draft.ColorConfiguration = new LightColorConfiguration(
            LightType.SingleColor,
            Color.Red,
            draft.ColorConfiguration.DiscreteColorSet,
            draft.ColorConfiguration.FullColorOrder);

        var page = new ColorFeatureWizardPage(new InMemoryColorConfigurationCatalog());
        page.Initialize(draft, new TestPreviewSession(draft));

        page.SetSingleColor(Color.Cyan);

        Assert.Equal(LightType.SingleColor, draft.ColorConfiguration.LightType);
        Assert.Equal(Color.Cyan.ToArgb(), draft.ColorConfiguration.SingleColor.ToArgb());
    }

    [Fact]
    public void CustomDiscreteColorSetSave_UpdatesDraftAndCatalog()
    {
        var draft = CreateDraft();
        var catalog = new InMemoryColorConfigurationCatalog();
        var page = new ColorFeatureWizardPage(catalog);
        page.Initialize(draft, new TestPreviewSession(draft));

        page.LightType = LightType.MultipleDiscreteColors;
        page.NewDiscreteColorSetName = "Sunset";
        page.SetWorkingDiscreteColor(page.WorkingDiscreteColors[0], Color.Orange);
        page.SetWorkingDiscreteColor(page.WorkingDiscreteColors[1], Color.DeepPink);
        page.RemoveSelectedWorkingDiscreteColor();
        page.SaveCustomDiscreteColorSet();

        Assert.Equal("Sunset", draft.ColorConfiguration.DiscreteColorSet?.Name);
        Assert.Contains(catalog.GetDiscreteColorSets(), set => set.Name == "Sunset");
        Assert.Equal("Sunset", page.SelectedDiscreteColorSet?.Name);
    }

    [Fact]
    public void FullColorSelection_UpdatesSharedDraftImmediately()
    {
        var draft = CreateDraft();
        var page = new ColorFeatureWizardPage(new InMemoryColorConfigurationCatalog());
        page.Initialize(draft, new TestPreviewSession(draft));

        page.LightType = LightType.FullColor;
        page.SelectedFullColorOrder = page.AvailableFullColorOrders.Single(order => order.Name == "GRWB");

        Assert.Equal(LightType.FullColor, draft.ColorConfiguration.LightType);
        Assert.Equal("GRWB", draft.ColorConfiguration.FullColorOrder?.Name);
    }

    private static TreePropDraft CreateDraft()
    {
        return new TreePropDraft
        {
            ColorConfiguration = new LightColorConfiguration(
                LightType.MultipleDiscreteColors,
                Color.White,
                new DiscreteColorSetDefinition("RGBW", [Color.Red, Color.Green, Color.Blue, Color.White]),
                new FullColorOrderDefinition(
                    "RGBW",
                    [LightColorChannel.Red, LightColorChannel.Green, LightColorChannel.Blue, LightColorChannel.White]))
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
