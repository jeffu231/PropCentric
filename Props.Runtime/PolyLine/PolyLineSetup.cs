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
using Props.Runtime.Wizards.Core;

namespace Props.Runtime.PolyLine;

/// <summary>
/// Setup wrapper around the polyline prop wizard.
/// </summary>
public sealed class PolyLineSetup(
    IFeatureWizardPageResolver featurePageResolver,
    IPropFactory propFactory,
    IPropDraftMapper<PolyLinePropDraft, PolyLineProp> draftMapper,
    IWizardPreviewCoordinator<PolyLinePropDraft> previewCoordinator,
    ISegmentCaptureNormalizer segmentCaptureNormalizer) : IPropSetup
{
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
        var polyLineProp = propFactory.Create<PolyLineProp>();
        ApplyCapturedSegmentsIfPresent(polyLineProp, context);

        var draft = new PolyLinePropDraft();
        draftMapper.PopulateDraft(draft, polyLineProp);

        var featurePages = featurePageResolver.GetPagesFor(typeof(PolyLineProp));
        var featureMappers = featurePageResolver.GetMappersFor(featurePages);
        var wizard = CreateWizard(draft, featurePages);

        PopulateWizardFromDraft(draft, wizard, polyLineProp, featureMappers);

        var result = await ShowWizard(wizard);
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
        draftMapper.PopulateDraft(draft, polyLineProp);

        var featurePages = featurePageResolver.GetPagesFor(typeof(PolyLineProp));
        var featureMappers = featurePageResolver.GetMappersFor(featurePages);
        var wizard = CreateWizard(draft, featurePages);

        PopulateWizardFromDraft(draft, wizard, polyLineProp, featureMappers);

        var result = await ShowWizard(wizard);
        if (result == true)
        {
            draftMapper.ApplyDraft(draft, polyLineProp);
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
        draftMapper.ApplyDraft(draft, polyLineProp);
        foreach (var mapper in mappers)
        {
            mapper.ApplyTo(polyLineProp);
        }

        await polyLineProp.CommitAsync();

        var propGroup = new PropGroup();
        propGroup.Props.Add(polyLineProp);
        return propGroup;
    }

    private static void PopulateWizardFromDraft(
        PolyLinePropDraft draft,
        IWizard wizard,
        PolyLineProp polyLineProp,
        IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        var page = (PolyLinePropWizardPage)wizard.Pages.Single(p => p is PolyLinePropWizardPage);
        page.Name = draft.Name;
        page.LightSize = draft.LightSize;

        foreach (var mapper in mappers)
        {
            mapper.PopulateFrom(polyLineProp);
        }
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

        var polyLinePage = new PolyLinePropWizardPage(draft, previewCoordinator);
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
                polyLineProp.ReplaceSegments(segmentCaptureNormalizer.Normalize(segmentCaptureContext));
                return;
            default:
                throw new ArgumentException(
                    $"Unsupported setup context type '{context.GetType().Name}' for {nameof(PolyLineProp)}.",
                    nameof(context));
        }
    }
}
