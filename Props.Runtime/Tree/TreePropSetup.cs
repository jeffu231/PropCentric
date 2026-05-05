using Catel.IoC;
using Catel.Services;
using Orc.Theming;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Runtime.Tree.Wizard;
using Props.Runtime.Tree.Wizard.Pages;
using Props.Runtime.Wizards;

namespace Props.Runtime.Tree;

/// <summary>
/// Setup wrapper around the TreePropWizard
/// </summary>
/// <param name="featurePageResolver"></param>
/// <param name="propFactory"></param>
public class TreePropSetup(IFeatureWizardPageResolver featurePageResolver, IPropFactory propFactory) : IPropSetup
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
        var featurePages = featurePageResolver.GetPagesFor(typeof(TreeProp));
        var featureMappers = featurePageResolver.GetMappersFor(featurePages);
        var treeWizard = CreateTreeWizard(featurePages);
        
        var treeProp = propFactory.Create<TreeProp>();
        //Initialize wizard with defaults from TreeProp
        PopulateWizardFromProp(treeProp, treeWizard, featureMappers);

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
            return BuildPropGroup(treeProp, treeWizard, featureMappers);

        return null;
    }

    private async Task EditWizard(TreeProp treeProp)
    {
        var featurePages = featurePageResolver.GetPagesFor(typeof(TreeProp));
        var featureMappers = featurePageResolver.GetMappersFor(featurePages);
        var treeWizard = CreateTreeWizard(featurePages);

        PopulateWizardFromProp(treeProp, treeWizard, featureMappers);

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
        {
            //Apply the edits back to the prop
            ApplyWizardDataToProp(treeProp, treeWizard, featureMappers);
        }
    }

    private TreePropWizard CreateTreeWizard(IReadOnlyList<IWizardPage> featurePages)
    {
        IDependencyResolver dependencyResolver = this.GetDependencyResolver();
        IMessageService? ms = dependencyResolver.Resolve<IMessageService>();
        IBaseColorSchemeService? baseColorService = (IBaseColorSchemeService?)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));
        ITypeFactory typeFactory = this.GetTypeFactory();

        ArgumentNullException.ThrowIfNull(ms);
        ArgumentNullException.ThrowIfNull(baseColorService);

        baseColorService.SetBaseColorScheme("Dark");

        var wizard = new TreePropWizard(typeFactory, ms);

        foreach (var page in featurePages)
            wizard.AddPage(page);

        SummaryWizardPage summaryPage = wizard.AddPage<SummaryWizardPage>();
        summaryPage.Description = $"Below is a summary of the {wizard.Title} selections.";

        var navController = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PropWizardNavigationController>(wizard);
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

    private IPropGroup BuildPropGroup(TreeProp treeProp, IPropWizard wizard, IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        ApplyWizardDataToProp(treeProp, wizard, mappers);

        var propGroup = new PropGroup();
        propGroup.Props.Add(treeProp);
        // TODO logic to determine if we need to populate more items in the group obtained from the group page
        return propGroup;
    }

    private static void PopulateWizardFromProp(TreeProp treeProp, IWizard wizard,
        IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        var page = (TreePropWizardPage)wizard.Pages.Single(p => p is TreePropWizardPage);
        // fill page with current values 
        page.Name = treeProp.Name;
        page.Strings = treeProp.Strings;
        page.NodesPerString = treeProp.NodesPerString;
        page.LightSize = treeProp.LightSize;
        page.DegreeOffset = treeProp.DegreeOffset;
        page.DegreesCoverage = treeProp.DegreesCoverage;
        page.BaseHeight = treeProp.BaseHeight;
        page.TopHeight = treeProp.TopHeight;
        page.TopWidth = treeProp.TopWidth;
        page.StartLocation = treeProp.StartLocation;
        page.ZigZag = treeProp.ZigZag;
        page.ZigZagOffset = treeProp.ZigZagOffset;
        page.TopRadius = treeProp.TopRadius;
        page.BottomRadius = treeProp.BottomRadius;
       
        //Fill the feature pages
        foreach (var mapper in mappers) mapper.PopulateFrom(treeProp);
    }

    private static void ApplyWizardDataToProp(TreeProp treeProp, IPropWizard wizard, IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        var page = (TreePropWizardPage)wizard.Pages.Single(p => p is TreePropWizardPage);
        treeProp.Name = page.Name;
        treeProp.Strings = page.Strings;
        treeProp.NodesPerString = page.NodesPerString;
        treeProp.LightSize = page.LightSize;
        treeProp.DegreeOffset = page.DegreeOffset;
        treeProp.DegreesCoverage = page.DegreesCoverage;
        treeProp.BaseHeight = page.BaseHeight;
        treeProp.TopHeight = page.TopHeight;
        treeProp.TopWidth = page.TopWidth;
        treeProp.StartLocation = page.StartLocation;
        treeProp.ZigZag = page.ZigZag;
        treeProp.ZigZagOffset = page.ZigZagOffset;
        treeProp.TopRadius = page.TopRadius;
        treeProp.BottomRadius = page.BottomRadius;
        //Map feature page data into prop
        foreach (var mapper in mappers) mapper.ApplyTo(treeProp);
    }
}
