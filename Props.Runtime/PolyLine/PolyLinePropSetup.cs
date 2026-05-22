using Catel.IoC;
using Catel.Services;
using Orc.Theming;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.PolyLine.Wizard;
using Props.Runtime.PolyLine.Wizard.Pages;
using Props.Runtime.Wizards.Core.Preview;
using Props.Runtime.Wizards.Core;

namespace Props.Runtime.PolyLine;

/// <summary>
/// Setup wrapper around the polyline prop wizard.
/// </summary>
public sealed class PolyLinePropSetup : IPropSetup
{
    private readonly IFeatureWizardPageResolver _featurePageResolver;
    private readonly IPropFactory _propFactory;
    private readonly IPropDraftMapper<PolyLinePropDraft, PolyLineProp> _draftMapper;
    private readonly IWizardPreviewCoordinator<PolyLinePropDraft> _previewCoordinator;
    private readonly ISegmentCaptureNormalizer _segmentCaptureNormalizer;
    private readonly Func<PolyLinePropDraft, IReadOnlyList<IWizardPage>, PolyLinePropWizard> _wizardFactory;
    private readonly Func<PolyLinePropWizard, Task<bool?>> _wizardPresenter;

    public PolyLinePropSetup(
        IFeatureWizardPageResolver featurePageResolver,
        IPropFactory propFactory,
        IPropDraftMapper<PolyLinePropDraft, PolyLineProp> draftMapper,
        IWizardPreviewCoordinator<PolyLinePropDraft> previewCoordinator,
        ISegmentCaptureNormalizer segmentCaptureNormalizer)
        : this(featurePageResolver, propFactory, draftMapper, previewCoordinator, segmentCaptureNormalizer, null, null)
    {
    }

    public PolyLinePropSetup(
        IFeatureWizardPageResolver featurePageResolver,
        IPropFactory propFactory,
        IPropDraftMapper<PolyLinePropDraft, PolyLineProp> draftMapper,
        IWizardPreviewCoordinator<PolyLinePropDraft> previewCoordinator,
        ISegmentCaptureNormalizer segmentCaptureNormalizer,
        Func<PolyLinePropDraft, IReadOnlyList<IWizardPage>, PolyLinePropWizard>? wizardFactory,
        Func<PolyLinePropWizard, Task<bool?>>? wizardPresenter)
    {
        _featurePageResolver = featurePageResolver;
        _propFactory = propFactory;
        _draftMapper = draftMapper;
        _previewCoordinator = previewCoordinator;
        _segmentCaptureNormalizer = segmentCaptureNormalizer;
        _wizardFactory = wizardFactory ?? CreateWizard;
        _wizardPresenter = wizardPresenter ?? ShowWizard;
    }

    public Task<IPropGroup?> CreateAsync(IPropSetupContext? context = null)
    {
        return CreatePropGroup(context);
    }

    public async Task<IProp> EditAsync(IProp existing, IPropSetupContext? context = null)
    {
        var polyLineProp = existing as PolyLineProp
            ?? throw new ArgumentException($"Expected {nameof(PolyLineProp)}", nameof(existing));

        await EditWizard(polyLineProp, context);
        return polyLineProp;
    }

    private async Task<IPropGroup?> CreatePropGroup(IPropSetupContext? context)
    {
        var polyLineProp = _propFactory.Create<PolyLineProp>();
        ApplyCapturedSegmentsIfPresent(polyLineProp, context);

        var draft = new PolyLinePropDraft();
        _draftMapper.PopulateDraft(draft, polyLineProp);
        var previewSession = new WizardPreviewSession<PolyLinePropDraft>(draft, _previewCoordinator);

        var featurePages = _featurePageResolver.GetPagesFor(typeof(PolyLineProp));
        _featurePageResolver.InitializePages(featurePages, draft, previewSession);
        var featureMappers = _featurePageResolver.GetMappersFor(featurePages);
        var wizard = _wizardFactory(draft, featurePages);

        foreach (var mapper in featureMappers)
        {
            mapper.PopulateFrom(polyLineProp);
        }

        var result = await _wizardPresenter(wizard);
        if (result == true)
        {
            return await BuildPropGroupAsync(polyLineProp, draft, featureMappers);
        }

        return null;
    }

    private async Task EditWizard(PolyLineProp polyLineProp, IPropSetupContext? context)
    {
        ApplyCapturedSegmentsIfPresent(polyLineProp, context);

        var draft = new PolyLinePropDraft();
        _draftMapper.PopulateDraft(draft, polyLineProp);
        var previewSession = new WizardPreviewSession<PolyLinePropDraft>(draft, _previewCoordinator);

        var featurePages = _featurePageResolver.GetPagesFor(typeof(PolyLineProp));
        _featurePageResolver.InitializePages(featurePages, draft, previewSession);
        var featureMappers = _featurePageResolver.GetMappersFor(featurePages);
        var wizard = _wizardFactory(draft, featurePages);

        foreach (var mapper in featureMappers)
        {
            mapper.PopulateFrom(polyLineProp);
        }

        var result = await _wizardPresenter(wizard);
        if (result == true)
        {
            _draftMapper.ApplyDraft(draft, polyLineProp);
            foreach (var mapper in featureMappers)
            {
                mapper.ApplyTo(polyLineProp);
            }

            await polyLineProp.CommitAsync();
        }
    }

    private async Task<IPropGroup> BuildPropGroupAsync(
        PolyLineProp polyLineProp,
        PolyLinePropDraft draft,
        IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        _draftMapper.ApplyDraft(draft, polyLineProp);
        foreach (var mapper in mappers)
        {
            mapper.ApplyTo(polyLineProp);
        }

        await polyLineProp.CommitAsync();

        var propGroup = new PropGroup();
        propGroup.Props.Add(polyLineProp);
        return propGroup;
    }

    private PolyLinePropWizard CreateWizard(PolyLinePropDraft draft, IReadOnlyList<IWizardPage> featurePages)
    {
        var dependencyResolver = this.GetDependencyResolver();
        var messageService = dependencyResolver.Resolve<IMessageService>();
        var baseColorService = (IBaseColorSchemeService?)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));
        var typeFactory = this.GetTypeFactory();

        ArgumentNullException.ThrowIfNull(messageService);
        ArgumentNullException.ThrowIfNull(baseColorService);

        baseColorService.SetBaseColorScheme("Dark");

        var polyLinePage = new PolyLinePropWizardPage(draft, _previewCoordinator);
        var wizard = new PolyLinePropWizard(typeFactory, polyLinePage);

        foreach (var page in featurePages)
        {
            wizard.AddPage(page);
        }

        var summaryPage = wizard.AddPage<SummaryWizardPage>();
        summaryPage.Description = $"Below is a summary of the {wizard.Title} selections.";

        var navController = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PropWizardNavigationController>(wizard);
        ArgumentNullException.ThrowIfNull(navController);
        wizard.NavigationControllerWrapper = navController;

        return wizard;
    }

    private async Task<bool?> ShowWizard(PolyLinePropWizard wizard)
    {
        var dependencyResolver = this.GetDependencyResolver();
        var wizardService = dependencyResolver.Resolve<IWizardService>();
        ArgumentNullException.ThrowIfNull(wizardService);
        return (await wizardService.ShowWizardAsync(wizard)).DialogResult;
    }

    private void ApplyCapturedSegmentsIfPresent(PolyLineProp polyLineProp, IPropSetupContext? context)
    {
        switch (context)
        {
            case null:
                return;
            case SegmentCaptureSetupContext segmentCaptureContext:
                polyLineProp.ReplaceSegments(_segmentCaptureNormalizer.Normalize(segmentCaptureContext));
                return;
            default:
                throw new ArgumentException(
                    $"Unsupported setup context type '{context.GetType().Name}' for {nameof(PolyLineProp)}.",
                    nameof(context));
        }
    }
}
