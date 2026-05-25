using System.Drawing;
using Catel.Services;
using Props.Abstractions.Features;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Registry;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Wizards.Features.Color.Pages;
using Props.Runtime.Wizards.Features.Color.Services;
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

    [Fact]
    public void PickSingleColorCommand_UsesInteractionServiceAndUpdatesPage()
    {
        var page = CreatePage();
        page.LightType = LightType.SingleColor;
        var interactionService = new TestColorFeatureWizardInteractionService
        {
            NextColor = Color.Orange
        };
        var viewModel = new ColorFeatureWizardPageViewModel(page, interactionService, new TestMessageService());

        viewModel.PickSingleColorCommand.Execute();

        Assert.Equal(Color.White.ToArgb(), interactionService.PickedColors.Single().ToArgb());
        Assert.Equal(Color.Orange.ToArgb(), page.SingleColor.ToArgb());
    }

    [Fact]
    public void EditWorkingDiscreteColorCommand_UsesInteractionServiceAndUpdatesItem()
    {
        var page = CreatePage();
        var interactionService = new TestColorFeatureWizardInteractionService
        {
            NextColor = Color.Gold
        };
        var viewModel = new ColorFeatureWizardPageViewModel(page, interactionService, new TestMessageService());
        var item = page.WorkingDiscreteColors[1];

        viewModel.EditWorkingDiscreteColorCommand.Execute(item);

        Assert.Equal(Color.Green.ToArgb(), interactionService.PickedColors.Single().ToArgb());
        Assert.Equal(Color.Gold.ToArgb(), item.Color.ToArgb());
    }

    [Fact]
    public void AddAndRemoveCommands_UpdateWorkingDiscreteColors()
    {
        var page = CreatePage();
        var viewModel = new ColorFeatureWizardPageViewModel(page);
        var originalCount = page.WorkingDiscreteColors.Count;

        viewModel.AddWorkingDiscreteColorCommand.Execute();

        Assert.Equal(originalCount + 1, page.WorkingDiscreteColors.Count);
        Assert.NotNull(page.SelectedWorkingDiscreteColor);

        viewModel.RemoveWorkingDiscreteColorCommand.Execute();

        Assert.Equal(originalCount, page.WorkingDiscreteColors.Count);
    }

    [Fact]
    public async Task SaveCustomSetCommand_ShowsWarningWhenPageRejectsSave()
    {
        var page = CreatePage();
        page.NewDiscreteColorSetName = string.Empty;
        var interactionService = new TestColorFeatureWizardInteractionService();
        var messageService = new TestMessageService();
        var viewModel = new ColorFeatureWizardPageViewModel(page, interactionService, messageService);

        viewModel.SaveCustomSetCommand.Execute();
        await WaitForAsync(() => messageService.WarningMessage is not null);

        Assert.Equal("Color Set", messageService.WarningCaption);
        Assert.Equal("Custom color set names must be provided before saving.", messageService.WarningMessage);
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
        page.Initialize(new FeatureWizardContext(draft, new TestPreviewSession(draft)));
        return page;
    }

    private static async Task WaitForAsync(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(10);
        }
    }

    private sealed class TestPreviewSession(TreePropDraft draft) : IWizardPreviewSession<TreePropDraft>
    {
        public TreePropDraft Draft => draft;

        Props.Abstractions.Setup.IPropDraft IWizardPreviewSession.Draft => Draft;

        public Task<IPropVisualModel> BuildPreviewAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IPropVisualModel>(new TreePropVisualModel { Elements = [] });
    }

    private sealed class TestColorFeatureWizardInteractionService : IColorFeatureWizardInteractionService
    {
        public List<Color> PickedColors { get; } = [];

        public Color? NextColor { get; init; }

        public Color? PickColor(Color initialColor)
        {
            PickedColors.Add(initialColor);
            return NextColor;
        }
    }

    private sealed class TestMessageService : IMessageService
    {
        public string? WarningMessage { get; private set; }

        public string? WarningCaption { get; private set; }

        public Task<MessageResult> ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowErrorAsync(string message, string caption)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowErrorAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowWarningAsync(string message, string caption)
        {
            WarningMessage = message;
            WarningCaption = caption;
            return Task.FromResult(MessageResult.OK);
        }

        public Task<MessageResult> ShowWarningAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageResult defaultResult = MessageResult.None)
        {
            WarningMessage = message;
            WarningCaption = caption;
            return Task.FromResult(MessageResult.OK);
        }

        public Task<MessageResult> ShowInformationAsync(string message, string caption)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowInformationAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);
    }
}
