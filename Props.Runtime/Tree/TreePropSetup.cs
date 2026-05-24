using Catel.IoC;
using Catel.Services;
using Orc.Theming;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Wizard;
using Props.Runtime.Tree.Wizard.Pages;
using Props.Runtime.Wizards;
using Props.Runtime.Wizards.Core;
using Props.Runtime.Wizards.Core.Preview;

namespace Props.Runtime.Tree;

/// <summary>
/// Setup wrapper around the TreePropWizard.
/// </summary>
public sealed class TreePropSetup : IPropSetup
{
    private readonly IFeatureWizardPageResolver _featurePageResolver;
    private readonly IPropFactory _propFactory;
    private readonly IPropDraftMapper<TreePropDraft, TreeProp> _draftMapper;
    private readonly IWizardPreviewCoordinator<TreePropDraft> _previewCoordinator;
    private readonly Func<TreePropDraft, IReadOnlyList<IWizardPage>, TreePropWizard> _wizardFactory;
    private readonly Func<TreePropWizard, Task<bool?>> _wizardPresenter;

    public TreePropSetup(
        IFeatureWizardPageResolver featurePageResolver,
        IPropFactory propFactory,
        IPropDraftMapper<TreePropDraft, TreeProp> draftMapper,
        IWizardPreviewCoordinator<TreePropDraft> previewCoordinator)
        : this(featurePageResolver, propFactory, draftMapper, previewCoordinator, null, null)
    {
    }

    public TreePropSetup(
        IFeatureWizardPageResolver featurePageResolver,
        IPropFactory propFactory,
        IPropDraftMapper<TreePropDraft, TreeProp> draftMapper,
        IWizardPreviewCoordinator<TreePropDraft> previewCoordinator,
        Func<TreePropDraft, IReadOnlyList<IWizardPage>, TreePropWizard>? wizardFactory,
        Func<TreePropWizard, Task<bool?>>? wizardPresenter)
    {
        _featurePageResolver = featurePageResolver;
        _propFactory = propFactory;
        _draftMapper = draftMapper;
        _previewCoordinator = previewCoordinator;
        _wizardFactory = wizardFactory ?? CreateTreeWizard;
        _wizardPresenter = wizardPresenter ?? ShowWizard;
    }

    public async Task<IProp> EditAsync(IProp existing, IPropSetupContext? context = null)
    {
        var treeProp = existing as TreeProp
            ?? throw new ArgumentException($"Expected {nameof(TreeProp)}", nameof(existing));
        await EditWizard(treeProp);
        return treeProp;
    }

    public Task<IPropGroup?> CreateAsync(IPropSetupContext? context = null)
    {
        return CreatePropGroup();
    }

    private async Task<IPropGroup?> CreatePropGroup()
    {
        var treeProp = _propFactory.Create<TreeProp>();
        var draft = new TreePropDraft();
        _draftMapper.PopulateDraft(draft, treeProp);
        var previewSession = new WizardPreviewSession<TreePropDraft>(draft, _previewCoordinator);

        var featurePages = _featurePageResolver.GetPagesFor(typeof(TreeProp));
        _featurePageResolver.InitializePages(featurePages, draft, previewSession);
        var featureMappers = _featurePageResolver.GetMappersFor(featurePages);
        var treeWizard = _wizardFactory(draft, featurePages);

        foreach (var mapper in featureMappers)
        {
            mapper.PopulateFrom(treeProp);
        }

        bool? result = await _wizardPresenter(treeWizard);
        if (result.HasValue && result.Value)
            return await BuildPropGroupAsync(treeProp, draft, featureMappers);

        return null;
    }

    private async Task EditWizard(TreeProp treeProp)
    {
        var draft = new TreePropDraft();
        _draftMapper.PopulateDraft(draft, treeProp);
        var previewSession = new WizardPreviewSession<TreePropDraft>(draft, _previewCoordinator);

        var featurePages = _featurePageResolver.GetPagesFor(typeof(TreeProp));
        _featurePageResolver.InitializePages(featurePages, draft, previewSession);
        var featureMappers = _featurePageResolver.GetMappersFor(featurePages);
        var treeWizard = _wizardFactory(draft, featurePages);

        foreach (var mapper in featureMappers) mapper.PopulateFrom(treeProp);

        bool? result = await _wizardPresenter(treeWizard);
        if (result.HasValue && result.Value)
        {
            _draftMapper.ApplyDraft(draft, treeProp);
            foreach (var mapper in featureMappers) mapper.ApplyTo(treeProp);
            await treeProp.CommitAsync();
        }
    }

    private async Task<IPropGroup> BuildPropGroupAsync(TreeProp treeProp, TreePropDraft draft,
        IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        _draftMapper.ApplyDraft(draft, treeProp);
        foreach (var mapper in mappers) mapper.ApplyTo(treeProp);
        await treeProp.CommitAsync();

        var propGroup = new PropGroup();
        propGroup.Props.Add(treeProp);
        //TODO check grouping page results to see if we need to create more of the same Prop
        // The grouping wizard page is not here and beyond this POC
        return propGroup;
    }

    private TreePropWizard CreateTreeWizard(TreePropDraft draft, IReadOnlyList<IWizardPage> featurePages)
    {
        IDependencyResolver dependencyResolver = this.GetDependencyResolver();
        IMessageService? ms = dependencyResolver.Resolve<IMessageService>();
        IBaseColorSchemeService? baseColorService =
            (IBaseColorSchemeService?)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));
        ITypeFactory typeFactory = this.GetTypeFactory();

        ArgumentNullException.ThrowIfNull(ms);
        ArgumentNullException.ThrowIfNull(baseColorService);

        baseColorService.SetBaseColorScheme("Dark");

        var treePropPage = new TreePropWizardPage(draft, _previewCoordinator);
        var wizard = new TreePropWizard(typeFactory, ms, treePropPage);

        foreach (var page in featurePages)
            wizard.AddPage(page);

        SummaryWizardPage summaryPage = wizard.AddPage<SummaryWizardPage>();
        summaryPage.Description = $"Below is a summary of the {wizard.Title} selections.";

        var navController =
            typeFactory.CreateInstanceWithParametersAndAutoCompletion<PropWizardNavigationController>(wizard);
        ArgumentNullException.ThrowIfNull(navController);
        wizard.NavigationControllerWrapper = navController;

        return wizard;
    }

    private async Task<bool?> ShowWizard(TreePropWizard wizard)
    {
        IDependencyResolver dependencyResolver = this.GetDependencyResolver();
        IWizardService? ws = dependencyResolver.Resolve<IWizardService>();
        ArgumentNullException.ThrowIfNull(ws);
        return (await ws.ShowWizardAsync(wizard)).DialogResult;
    }
}
