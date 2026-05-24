using System.Drawing;
using Props.Abstractions.Features;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Registry;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Wizards.Features.Color.Pages;
using Props.Runtime.Wizards.Features.Color.ViewModels;

namespace PropCentric.Tests.ColorFeature;

/// <summary>
/// Verifies the Color feature page view model surfaces the page state needed by the WPF view.
/// </summary>
public class ColorFeatureWizardPageViewModelTests
{
    [Fact]
    public void Constructor_ExposesInitialModeFlagsAndSingleColorState()
    {
        var page = CreatePage();

        page.LightType = LightType.SingleColor;
        page.SetSingleColor(Color.Magenta);

        var viewModel = new ColorFeatureWizardPageViewModel(page);

        Assert.Equal(LightType.SingleColor, viewModel.LightType);
        Assert.True(viewModel.IsSingleColorMode);
        Assert.False(viewModel.IsMultipleDiscreteColorsMode);
        Assert.False(viewModel.IsFullColorMode);
        Assert.Equal(Color.Magenta.ToArgb(), viewModel.SingleColor.ToArgb());
        Assert.Equal("#FF00FF", viewModel.SingleColorHex);
    }

    [Fact]
    public void LightTypeSetter_UpdatesPageAndModeFlags()
    {
        var page = CreatePage();
        var viewModel = new ColorFeatureWizardPageViewModel(page);

        viewModel.LightType = LightType.FullColor;

        Assert.Equal(LightType.FullColor, page.LightType);
        Assert.False(viewModel.IsSingleColorMode);
        Assert.False(viewModel.IsMultipleDiscreteColorsMode);
        Assert.True(viewModel.IsFullColorMode);
    }

    [Fact]
    public void PageSingleColorUpdates_RefreshViewModelPreviewState()
    {
        var page = CreatePage();
        var viewModel = new ColorFeatureWizardPageViewModel(page);

        page.LightType = LightType.SingleColor;
        page.SetSingleColor(Color.Cyan);

        Assert.Equal(Color.Cyan.ToArgb(), viewModel.SingleColor.ToArgb());
        Assert.Equal("#00FFFF", viewModel.SingleColorHex);
        Assert.True(viewModel.IsSingleColorMode);
    }

    private static ColorFeatureWizardPage CreatePage()
    {
        var draft = new TreePropDraft
        {
            ColorConfiguration = new LightColorConfiguration(
                LightType.MultipleDiscreteColors,
                Color.White,
                new DiscreteColorSetDefinition("RGBW", [Color.Red, Color.Green, Color.Blue, Color.White]),
                new FullColorOrderDefinition(
                    "RGBW",
                    [LightColorChannel.Red, LightColorChannel.Green, LightColorChannel.Blue, LightColorChannel.White]))
        };

        var page = new ColorFeatureWizardPage(new InMemoryColorConfigurationCatalog());
        page.Initialize(draft, new TestPreviewSession(draft));
        return page;
    }

    private sealed class TestPreviewSession(TreePropDraft draft) : IWizardPreviewSession<TreePropDraft>
    {
        public TreePropDraft Draft => draft;

        Props.Abstractions.Setup.IPropDraft IWizardPreviewSession.Draft => Draft;

        public IPropVisualModel BuildPreview() => new TreePropVisualModel { Elements = [] };
    }
}
