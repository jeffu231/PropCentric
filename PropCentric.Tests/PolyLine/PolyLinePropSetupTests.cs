using System.Globalization;
using Catel.IoC;
using Catel.Services;
using Orc.Theming;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.PolyLine.Visuals;
using Props.Runtime.PolyLine.Wizard;
using Props.Runtime.PolyLine.Wizard.Pages;
using Props.Runtime.Wizards.Features.Segments.Pages;

namespace PropCentric.Tests.PolyLine;

/// <summary>
/// Verifies polyline setup wrapper behavior for create and edit flows.
/// </summary>
public class PolyLinePropSetupTests : IDisposable
{
    private static readonly object RegistrationTag = typeof(PolyLinePropSetupTests);

    public PolyLinePropSetupTests()
    {
        RegisterGlobalServices(new TestWizardService());
    }

    [Fact]
    public async Task CreateAsync_WithContext_SeedsSegmentsBeforeWizard()
    {
        await RunInStaAsync(async () =>
        {
            var context = new SegmentCaptureSetupContext(
            [
                new CapturedWorldSegment(new(10f, 10f), new(20f, 10f), 5),
                new CapturedWorldSegment(new(20f, 10f), new(30f, 20f), 7)
            ],
                new WorldToModelTransform(new(0f, 0f), new(40f, 40f)));

            var wizardService = new TestWizardService();
            RegisterGlobalServices(wizardService);

            var setup = CreateSetup(wizardService);

            var result = await setup.CreateAsync(context);

            var group = Assert.IsType<PropGroup>(result);
            var prop = Assert.IsType<PolyLineProp>(Assert.Single(group.Props));
            Assert.Equal(2, prop.Segments.Count);
            Assert.Equal(new Segment(new(0.25f, 0.25f), new(0.5f, 0.25f), 5), prop.Segments[0]);
            Assert.Equal(new Segment(new(0.5f, 0.25f), new(0.75f, 0.5f), 7), prop.Segments[1]);
        });
    }

    [Fact]
    public async Task EditAsync_WithoutContext_SeedsFeaturePageFromDraftState()
    {
        await RunInStaAsync(async () =>
        {
            var page = new SegmentsFeatureWizardPage();
            var wizardService = new TestWizardService(wizard =>
            {
                var segmentsPage = Assert.IsType<SegmentsFeatureWizardPage>(wizard.Pages.Single(p => p is SegmentsFeatureWizardPage));
                Assert.Equal(2, segmentsPage.Segments.Count);
                Assert.Equal(80, segmentsPage.TotalPoints);
                Assert.NotNull(segmentsPage.PreviewSession);

                segmentsPage.Segments[0].PointCount = 12;
                segmentsPage.Segments[1].PointCount = 34;
            });
            RegisterGlobalServices(wizardService);

            var setup = CreateSetup(
                wizardService,
                new TestFeatureWizardPageResolver([page], []));
            var prop = PolyLineTestData.CreateTreeProp();

            await setup.EditAsync(prop);

            Assert.Equal(12, prop.Segments[0].PointCount);
            Assert.Equal(34, prop.Segments[1].PointCount);
            Assert.Equal(new System.Numerics.Vector2(20f, 20f), prop.Segments[0].Start);
            Assert.Equal(new System.Numerics.Vector2(30f, 30f), prop.Segments[0].End);
        });
    }

    [Fact]
    public async Task EditAsync_WithContext_ReplacesGeometryBeforeWizard()
    {
        await RunInStaAsync(async () =>
        {
            var recaptureContext = new SegmentCaptureSetupContext(
            [
                new CapturedWorldSegment(new(0f, 0f), new(20f, 0f), 8),
                new CapturedWorldSegment(new(20f, 0f), new(20f, 20f), 9)
            ],
                new WorldToModelTransform(new(0f, 0f), new(40f, 40f)));

            var page = new SegmentsFeatureWizardPage();
            var wizardService = new TestWizardService(wizard =>
            {
                var segmentsPage = Assert.IsType<SegmentsFeatureWizardPage>(wizard.Pages.Single(p => p is SegmentsFeatureWizardPage));
                Assert.Equal("(0, 0)", segmentsPage.Segments[0].StartDisplay);
                Assert.Equal("(0.5, 0)", segmentsPage.Segments[0].EndDisplay);
                Assert.Equal("(0.5, 0)", segmentsPage.Segments[1].StartDisplay);
                Assert.Equal("(0.5, 0.5)", segmentsPage.Segments[1].EndDisplay);

                segmentsPage.Segments[0].PointCount = 10;
                segmentsPage.Segments[1].PointCount = 11;
            });
            RegisterGlobalServices(wizardService);

            var setup = CreateSetup(
                wizardService,
                new TestFeatureWizardPageResolver([page], []));
            var prop = PolyLineTestData.CreateTreeProp();

            await setup.EditAsync(prop, recaptureContext);

            Assert.Equal(new Segment(new(0f, 0f), new(0.5f, 0f), 10), prop.Segments[0]);
            Assert.Equal(new Segment(new(0.5f, 0f), new(0.5f, 0.5f), 11), prop.Segments[1]);
        });
    }

    [Fact]
    public async Task EditAsync_InitializesDraftBackedFeaturePages()
    {
        await RunInStaAsync(async () =>
        {
            var page = new TestDraftBackedFeatureWizardPage();
            var wizardService = new TestWizardService(wizard =>
            {
                var featurePage = Assert.IsType<TestDraftBackedFeatureWizardPage>(
                    wizard.Pages.Single(p => p is TestDraftBackedFeatureWizardPage));
                Assert.NotNull(featurePage.InitializedDraft);
                Assert.NotNull(featurePage.PreviewSession);
                Assert.Same(featurePage.InitializedDraft, featurePage.PreviewSession!.Draft);
            });
            RegisterGlobalServices(wizardService);

            var setup = CreateSetup(
                wizardService,
                new TestFeatureWizardPageResolver([page], []));
            var prop = PolyLineTestData.CreateTreeProp();

            await setup.EditAsync(prop);

            var typedDraft = Assert.IsType<PolyLinePropDraft>(page.InitializedDraft);
            Assert.Equal(prop.Segments.Count, typedDraft.Segments.Count);
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

    private static PolyLinePropSetup CreateSetup(
        TestWizardService wizardService,
        IFeatureWizardPageResolver? featureResolver = null)
    {
        RegisterGlobalServices(wizardService);
        var previewCoordinator = new PolyLineWizardPreviewCoordinator(
            new PolyLineDraftToVisualInputMapper(),
            new PolyLineVisualModelBuilder());

        return new PolyLinePropSetup(
            featureResolver ?? new TestFeatureWizardPageResolver([], []),
            new TestPropFactory(),
            new PolyLinePropDraftMapper(),
            previewCoordinator,
            new SegmentCaptureNormalizer(),
            (draft, featurePages) =>
            {
                var wizard = new PolyLinePropWizard(TypeFactory.Default, new PolyLinePropWizardPage(draft, previewCoordinator));
                foreach (var page in featurePages)
                {
                    wizard.AddPage(page);
                }

                return wizard;
            },
            async wizard =>
            {
                return (await wizardService.ShowWizardAsync(wizard)).DialogResult;
            });
    }

    private static void RegisterGlobalServices(TestWizardService wizardService)
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

    private sealed class TestFeatureWizardPageResolver(
        IReadOnlyList<IWizardPage> pages,
        IReadOnlyList<IFeatureWizardDataMapper> mappers) : IFeatureWizardPageResolver
    {
        public IReadOnlyList<IWizardPage> GetPagesFor(Type propType) => pages;

        public IReadOnlyList<IFeatureWizardDataMapper> GetMappersFor(IReadOnlyList<IWizardPage> requestedPages) => mappers;

        public void InitializePages(IReadOnlyList<IWizardPage> requestedPages, IPropDraft draft, IWizardPreviewSession previewSession)
        {
            foreach (var page in requestedPages.OfType<IFeatureWizardDraftPage>())
            {
                page.Initialize(draft, previewSession);
            }
        }
    }

    private sealed class TestDraftBackedFeatureWizardPage : WizardPageBase, IFeatureWizardDraftPage
    {
        public IPropDraft? InitializedDraft { get; private set; }

        public IWizardPreviewSession? PreviewSession { get; private set; }

        public void Initialize(IPropDraft draft, IWizardPreviewSession previewSession)
        {
            InitializedDraft = draft;
            PreviewSession = previewSession;
        }
    }

    private sealed class TestPropFactory : IPropFactory
    {
        public IProp Create(Guid id) => Create<PolyLineProp>();

        public TProp Create<TProp>() where TProp : IProp
        {
            if (typeof(TProp) != typeof(PolyLineProp))
            {
                throw new InvalidOperationException($"Unsupported prop type {typeof(TProp).Name}.");
            }

            return (TProp)(IProp)new PolyLineProp(new PolyLinePropToVisualInputMapper(), new PolyLineVisualModelBuilder());
        }
    }

    private sealed class TestWizardService(Action<IWizard>? onShow = null) : IWizardService
    {
        public Task<UIVisualizerResult> ShowWizardAsync(IWizard wizard)
        {
            onShow?.Invoke(wizard);
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
