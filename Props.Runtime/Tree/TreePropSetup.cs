using Catel.IoC;
using Catel.Services;
using Orc.Theming;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
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
    IPropDraftMapper<TreePropDraft, TreeProp> draftMapper) : IPropSetup
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
        var treeWizard = CreateTreeWizard(featurePages);

        PopulateWizardFromDraft(draft, treeWizard, treeProp, featureMappers);

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
            return BuildPropGroup(treeProp, draft, treeWizard, featureMappers);

        return null;
    }

    private async Task EditWizard(TreeProp treeProp)
    {
        var draft = new TreePropDraft();
        draftMapper.PopulateDraft(draft, treeProp);

        var featurePages = featurePageResolver.GetPagesFor(typeof(TreeProp));
        var featureMappers = featurePageResolver.GetMappersFor(featurePages);
        var treeWizard = CreateTreeWizard(featurePages);

        PopulateWizardFromDraft(draft, treeWizard, treeProp, featureMappers);

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
        {
            ReadWizardIntoDraft(draft, treeWizard);
            draftMapper.ApplyDraft(draft, treeProp);
            foreach (var mapper in featureMappers) mapper.ApplyTo(treeProp);
        }
    }

    private IPropGroup BuildPropGroup(TreeProp treeProp, TreePropDraft draft, IPropWizard wizard,
        IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        ReadWizardIntoDraft(draft, wizard);
        draftMapper.ApplyDraft(draft, treeProp);
        foreach (var mapper in mappers) mapper.ApplyTo(treeProp);

        var propGroup = new PropGroup();
        propGroup.Props.Add(treeProp);
        return propGroup;
    }

    private static void PopulateWizardFromDraft(TreePropDraft draft, IWizard wizard, TreeProp treeProp,
        IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        var page = (TreePropWizardPage)wizard.Pages.Single(p => p is TreePropWizardPage);
        page.Name = draft.Name;
        page.Strings = draft.Strings;
        page.NodesPerString = draft.NodesPerString;
        page.LightSize = draft.LightSize;
        page.DegreeOffset = draft.DegreeOffset;
        page.DegreesCoverage = draft.DegreesCoverage;
        page.BaseHeight = draft.BaseHeight;
        page.TopHeight = draft.TopHeight;
        page.TopWidth = draft.TopWidth;
        page.StartLocation = draft.StartLocation;
        page.ZigZag = draft.ZigZag;
        page.ZigZagOffset = draft.ZigZagOffset;
        page.TopRadius = draft.TopRadius;
        page.BottomRadius = draft.BottomRadius;

        foreach (var mapper in mappers) mapper.PopulateFrom(treeProp);
    }

    private static void ReadWizardIntoDraft(TreePropDraft draft, IWizard wizard)
    {
        var page = (TreePropWizardPage)wizard.Pages.Single(p => p is TreePropWizardPage);
        draft.Name = page.Name;
        draft.Strings = page.Strings;
        draft.NodesPerString = page.NodesPerString;
        draft.LightSize = page.LightSize;
        draft.DegreeOffset = page.DegreeOffset;
        draft.DegreesCoverage = page.DegreesCoverage;
        draft.BaseHeight = page.BaseHeight;
        draft.TopHeight = page.TopHeight;
        draft.TopWidth = page.TopWidth;
        draft.StartLocation = page.StartLocation;
        draft.ZigZag = page.ZigZag;
        draft.ZigZagOffset = page.ZigZagOffset;
        draft.TopRadius = page.TopRadius;
        draft.BottomRadius = page.BottomRadius;
    }

    private TreePropWizard CreateTreeWizard(IReadOnlyList<IWizardPage> featurePages)
    {
        IDependencyResolver dependencyResolver = this.GetDependencyResolver();
        IMessageService? ms = dependencyResolver.Resolve<IMessageService>();
        IBaseColorSchemeService? baseColorService =
            (IBaseColorSchemeService?)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));
        ITypeFactory typeFactory = this.GetTypeFactory();

        ArgumentNullException.ThrowIfNull(ms);
        ArgumentNullException.ThrowIfNull(baseColorService);

        baseColorService.SetBaseColorScheme("Dark");

        var wizard = new TreePropWizard(typeFactory, ms);

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
