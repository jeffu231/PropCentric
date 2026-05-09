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

namespace Props.Runtime.Tree;

/// <summary>
/// Setup wrapper around the TreePropWizard.
/// </summary>
public class TreePropSetup(
    IFeatureWizardPageResolver featurePageResolver,
    IPropFactory propFactory,
    IPropDraftMapper<TreePropDraft, TreeProp> draftMapper,
    IWizardPreviewCoordinator<TreePropDraft> previewCoordinator) : IPropSetup
{
    public async Task<IProp> EditAsync(IProp existing)
    {
        var treeProp = existing as TreeProp
            ?? throw new ArgumentException($"Expected {nameof(TreeProp)}", nameof(existing));
        await EditWizard(treeProp);
        return treeProp;
    }

    public Task<IPropGroup?> CreateAsync()
    {
        return CreatePropGroup();
    }

    private async Task<IPropGroup?> CreatePropGroup()
    {
        var treeProp = propFactory.Create<TreeProp>();
        var draft = new TreePropDraft();
        draftMapper.PopulateDraft(draft, treeProp);

        var featurePages = featurePageResolver.GetPagesFor(typeof(TreeProp));
        var featureMappers = featurePageResolver.GetMappersFor(featurePages);
        var treeWizard = CreateTreeWizard(draft, featurePages);

        PopulateWizardFromDraft(draft, treeWizard, treeProp, featureMappers);

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
            return await BuildPropGroupAsync(treeProp, draft, featureMappers);

        return null;
    }

    private async Task EditWizard(TreeProp treeProp)
    {
        var draft = new TreePropDraft();
        draftMapper.PopulateDraft(draft, treeProp);

        var featurePages = featurePageResolver.GetPagesFor(typeof(TreeProp));
        var featureMappers = featurePageResolver.GetMappersFor(featurePages);
        var treeWizard = CreateTreeWizard(draft, featurePages);

        PopulateWizardFromDraft(draft, treeWizard, treeProp, featureMappers);

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
        {
            draftMapper.ApplyDraft(draft, treeProp);
            foreach (var mapper in featureMappers) mapper.ApplyTo(treeProp);
            await treeProp.CommitAsync();
        }
    }

    private async Task<IPropGroup> BuildPropGroupAsync(TreeProp treeProp, TreePropDraft draft,
        IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        draftMapper.ApplyDraft(draft, treeProp);
        foreach (var mapper in mappers) mapper.ApplyTo(treeProp);
        await treeProp.CommitAsync();

        var propGroup = new PropGroup();
        propGroup.Props.Add(treeProp);
        //TODO check grouping page results to see if we need to create more of the same Prop
        // The grouping wizard page is not here and beyond this POC
        return propGroup;
    }

    // Seeds the two parent-class Catel-stored properties (Name, LightSize) and all feature pages.
    // Tree-specific properties are read directly from the draft by the page — no explicit seeding needed.
    private static void PopulateWizardFromDraft(TreePropDraft draft, IWizard wizard, TreeProp treeProp,
        IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        var page = (TreePropWizardPage)wizard.Pages.Single(p => p is TreePropWizardPage);
        page.Name = draft.Name;
        page.LightSize = draft.LightSize;

        foreach (var mapper in mappers) mapper.PopulateFrom(treeProp);
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

        var treePropPage = new TreePropWizardPage(draft, previewCoordinator);
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
