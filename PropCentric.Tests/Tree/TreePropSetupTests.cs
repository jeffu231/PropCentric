using System.Drawing;
using System.Globalization;
using Catel.IoC;
using Catel.Services;
using Orc.Theming;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Visuals;
using Props.Runtime.Wizards.Features.Color.Pages;
using Props.Runtime.Wizards.Features.Dimming.Pages;

namespace PropCentric.Tests.Tree;

/// <summary>
/// Verifies tree setup wrapper behavior for color-focused edit flows.
/// </summary>
public class TreePropSetupTests : IDisposable
{
    private static readonly object RegistrationTag = typeof(TreePropSetupTests);

    public TreePropSetupTests()
    {
        RegisterGlobalServices(new SequencedWizardService());
    }

    [Fact]
    public async Task EditAsync_ColorPage_RoundTripsCustomDiscreteColorSetAcrossReopen()
    {
        await RunInStaAsync(async () =>
        {
            var catalog = new Props.Registry.InMemoryColorConfigurationCatalog();
            var resolver = new TestFeatureWizardPageResolver(() => [new ColorFeatureWizardPage(catalog)]);
            var wizardService = new SequencedWizardService(
                wizard =>
                {
                    var colorPage = Assert.IsType<ColorFeatureWizardPage>(wizard.Pages.Single(page => page is ColorFeatureWizardPage));
                    colorPage.LightType = LightType.MultipleDiscreteColors;
                    colorPage.NewDiscreteColorSetName = "Holiday";
                    colorPage.SetWorkingDiscreteColor(colorPage.WorkingDiscreteColors[0], Color.Red);
                    colorPage.SetWorkingDiscreteColor(colorPage.WorkingDiscreteColors[1], Color.Lime);
                    colorPage.SetWorkingDiscreteColor(colorPage.WorkingDiscreteColors[2], Color.Blue);
                    colorPage.SetWorkingDiscreteColor(colorPage.WorkingDiscreteColors[3], Color.White);
                    colorPage.SaveCustomDiscreteColorSet();
                },
                wizard =>
                {
                    var colorPage = Assert.IsType<ColorFeatureWizardPage>(wizard.Pages.Single(page => page is ColorFeatureWizardPage));
                    Assert.Equal(LightType.MultipleDiscreteColors, colorPage.LightType);
                    Assert.Equal("Holiday", colorPage.SelectedDiscreteColorSet?.Name);
                    Assert.Equal(new[] { Color.Red, Color.Lime, Color.Blue, Color.White }.Select(color => color.ToArgb()),
                        colorPage.WorkingDiscreteColors.Select(item => item.Color.ToArgb()));
                });

            RegisterGlobalServices(wizardService);

            var setup = new TreePropSetup(
                resolver,
                new TestPropFactory(),
                new TreePropDraftMapper(),
                new TreeWizardPreviewCoordinator(new TreeDraftToVisualInputMapper(), new TreeVisualModelBuilder()),
                (draft, featurePages) =>
                {
                    var previewCoordinator = new TreeWizardPreviewCoordinator(new TreeDraftToVisualInputMapper(), new TreeVisualModelBuilder());
                    var messageService = new TestMessageService();
                    var wizard = new Props.Runtime.Tree.Wizard.TreePropWizard(
                        TypeFactory.Default,
                        messageService,
                        new Props.Runtime.Tree.Wizard.Pages.TreePropWizardPage(draft, previewCoordinator));
                    foreach (var page in featurePages)
                    {
                        wizard.AddPage(page);
                    }

                    return wizard;
                },
                async wizard => (await wizardService.ShowWizardAsync(wizard)).DialogResult);
            var prop = TreeTestData.CreateTreeProp();

            await setup.EditAsync(prop);

            Assert.Equal(LightType.MultipleDiscreteColors, prop.ColorConfiguration.LightType);
            Assert.Equal("Holiday", prop.ColorConfiguration.DiscreteColorSet?.Name);
            Assert.Equal(new[] { Color.Red, Color.Lime, Color.Blue, Color.White }.Select(color => color.ToArgb()),
                prop.ColorConfiguration.DiscreteColorSet!.Colors.Select(color => color.ToArgb()));
            Assert.Contains("Light Type:</b> Multiple discrete colors", prop.GetSummary());
            Assert.Contains("Color Set:</b> Holiday", prop.GetSummary());
            Assert.Contains("#FF0000, #00FF00, #0000FF, #FFFFFF", prop.GetSummary());

            await setup.EditAsync(prop);
        });
    }

    [Fact]
    public async Task EditAsync_DimmingPage_RoundTripsDraftBackedDimmingAcrossReopen()
    {
        await RunInStaAsync(async () =>
        {
            var wizardService = new SequencedWizardService(
                wizard =>
                {
                    var dimmingPage = Assert.IsType<DimmingFeatureWizardPage>(wizard.Pages.Single(page => page is DimmingFeatureWizardPage));
                    dimmingPage.Brightness = 63;
                    dimmingPage.Gamma = 2.1;
                },
                wizard =>
                {
                    var dimmingPage = Assert.IsType<DimmingFeatureWizardPage>(wizard.Pages.Single(page => page is DimmingFeatureWizardPage));
                    Assert.Equal(63, dimmingPage.Brightness);
                    Assert.Equal(2.1, dimmingPage.Gamma);
                });

            RegisterGlobalServices(wizardService);

            var setup = new TreePropSetup(
                new TestFeatureWizardPageResolver(() => [new DimmingFeatureWizardPage()]),
                new TestPropFactory(),
                new TreePropDraftMapper(),
                new TreeWizardPreviewCoordinator(new TreeDraftToVisualInputMapper(), new TreeVisualModelBuilder()),
                (draft, featurePages) =>
                {
                    var previewCoordinator = new TreeWizardPreviewCoordinator(new TreeDraftToVisualInputMapper(), new TreeVisualModelBuilder());
                    var messageService = new TestMessageService();
                    var wizard = new Props.Runtime.Tree.Wizard.TreePropWizard(
                        TypeFactory.Default,
                        messageService,
                        new Props.Runtime.Tree.Wizard.Pages.TreePropWizardPage(draft, previewCoordinator));
                    foreach (var page in featurePages)
                    {
                        wizard.AddPage(page);
                    }

                    return wizard;
                },
                async wizard => (await wizardService.ShowWizardAsync(wizard)).DialogResult);
            var prop = TreeTestData.CreateTreeProp();

            await setup.EditAsync(prop);

            Assert.Equal(63, prop.Brightness);
            Assert.Equal(2.1, prop.Gamma);

            await setup.EditAsync(prop);
        });
    }

    public void Dispose()
    {
        var serviceLocator = ServiceLocator.Default;
        serviceLocator.RemoveType(typeof(IWizardService), RegistrationTag);
        serviceLocator.RemoveType(typeof(IMessageService), RegistrationTag);
        serviceLocator.RemoveType(typeof(IBaseColorSchemeService), RegistrationTag);
        serviceLocator.RemoveType(typeof(ILanguageService), RegistrationTag);
    }

    private static void RegisterGlobalServices(IWizardService wizardService)
    {
        var serviceLocator = ServiceLocator.Default;
        serviceLocator.RemoveType(typeof(IWizardService), RegistrationTag);
        serviceLocator.RemoveType(typeof(IMessageService), RegistrationTag);
        serviceLocator.RemoveType(typeof(IBaseColorSchemeService), RegistrationTag);
        serviceLocator.RemoveType(typeof(ILanguageService), RegistrationTag);

        serviceLocator.RegisterInstance(typeof(IWizardService), wizardService, RegistrationTag);
        serviceLocator.RegisterInstance(typeof(IMessageService), new TestMessageService(), RegistrationTag);
        serviceLocator.RegisterInstance(typeof(IBaseColorSchemeService), new TestBaseColorSchemeService(), RegistrationTag);
        serviceLocator.RegisterInstance(typeof(ILanguageService), new TestLanguageService(), RegistrationTag);
    }

    private static Task RunInStaAsync(Func<Task> action)
    {
        var completionSource = new TaskCompletionSource();
        var thread = new Thread(() =>
        {
            try
            {
                action().GetAwaiter().GetResult();
                completionSource.SetResult();
            }
            catch (Exception ex)
            {
                completionSource.SetException(ex);
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return completionSource.Task;
    }

    private sealed class TestFeatureWizardPageResolver(Func<IReadOnlyList<IWizardPage>> pageFactory) : IFeatureWizardPageResolver
    {
        public IReadOnlyList<IWizardPage> GetPagesFor(Type propType) => pageFactory();

        public IReadOnlyList<IFeatureWizardDataMapper> GetMappersFor(IReadOnlyList<IWizardPage> requestedPages) => [];

        public void InitializePages(IReadOnlyList<IWizardPage> requestedPages, FeatureWizardContext context)
        {
            foreach (var page in requestedPages.OfType<IFeatureWizardDraftPage>())
            {
                page.Initialize(context);
            }
        }
    }

    private sealed class TestPropFactory : IPropFactory
    {
        public IProp Create(Guid id) => Create<TreeProp>();

        public TProp Create<TProp>() where TProp : IProp
        {
            if (typeof(TProp) != typeof(TreeProp))
            {
                throw new InvalidOperationException($"Unsupported prop type {typeof(TProp).Name}.");
            }

            return (TProp)(IProp)new TreeProp(new TreePropToVisualInputMapper(), new TreeVisualModelBuilder());
        }
    }

    private sealed class SequencedWizardService(params Action<IWizard>[] callbacks) : IWizardService
    {
        private readonly Queue<Action<IWizard>> _callbacks = new(callbacks);

        public Task<UIVisualizerResult> ShowWizardAsync(IWizard wizard)
        {
            if (_callbacks.Count > 0)
            {
                _callbacks.Dequeue().Invoke(wizard);
            }

            return Task.FromResult(new UIVisualizerResult(true, new UIVisualizerContext(), new object()));
        }
    }

    private sealed class TestMessageService : IMessageService
    {
        public Task<MessageResult> ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowErrorAsync(string message, string caption)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowErrorAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowInformationAsync(string message, string caption)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowInformationAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowWarningAsync(string message, string caption)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowWarningAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);
    }

    private sealed class TestBaseColorSchemeService : IBaseColorSchemeService
    {
        public event EventHandler<EventArgs>? BaseColorSchemeChanged;

        public IReadOnlyList<string> GetAvailableBaseColorSchemes() => ["Dark", "Light"];

        public string GetBaseColorScheme() => "Dark";

        public bool SetBaseColorScheme(string baseColorScheme)
        {
            BaseColorSchemeChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }

    private sealed class TestLanguageService : ILanguageService
    {
        public CultureInfo FallbackCulture { get; set; } = CultureInfo.InvariantCulture;

        public CultureInfo PreferredCulture { get; set; } = CultureInfo.InvariantCulture;

        public bool CacheResults { get; set; }

        public event EventHandler<EventArgs>? LanguageUpdated
        {
            add { }
            remove { }
        }

        public void RegisterLanguageSource(ILanguageSource languageSource) { }

        public string GetString(string resource) => resource;

        public string GetString(string resource, CultureInfo culture) => resource;

        public string GetString(ILanguageSource languageSource, string resource, CultureInfo culture) => resource;

        public string GetString(string resource, object argument0, CultureInfo? culture = null) => string.Format(culture ?? CultureInfo.InvariantCulture, resource, argument0);

        public string GetString(string resource, object[] arguments, CultureInfo? culture = null) => string.Format(culture ?? CultureInfo.InvariantCulture, resource, arguments);

        public void PreloadLanguageSources() { }

        public void ClearLanguageResources() { }
    }
}
