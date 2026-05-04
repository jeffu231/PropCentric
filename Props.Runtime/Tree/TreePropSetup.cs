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

public class TreePropSetup(IFeatureWizardPageResolver featurePageResolver) : IPropSetup
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

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
            return BuildPropGroup(treeWizard, featureMappers);

        return null;
    }

    private async Task EditWizard(TreeProp treeProp)
    {
        var featurePages = featurePageResolver.GetPagesFor(typeof(TreeProp));
        var featureMappers = featurePageResolver.GetMappersFor(featurePages);
        var treeWizard = CreateTreeWizard(featurePages);

        var page = (TreePropWizardPage)treeWizard.Pages.Single(p => p is TreePropWizardPage);
        page.Name = treeProp.Name;
        foreach (var mapper in featureMappers) mapper.PopulateFrom(treeProp);

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
            UpdateProp(treeProp, treeWizard, featureMappers);
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

        //TODO Can these be extracted to the base class as a default?
        wizard.ShowInTaskbarWrapper = true;
        wizard.ShowHelpWrapper = true;
        wizard.AllowQuickNavigationWrapper = true;
        wizard.HandleNavigationStatesWrapper = true;
        wizard.CacheViewsWrapper = false;
        var navController = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PropWizardNavigationController>(wizard);
        ArgumentNullException.ThrowIfNull(navController);
        wizard.NavigationControllerWrapper = navController;
        // end of extraction question

        return wizard;
    }

    private async Task<bool?> ShowWizard(TreePropWizard wizard)
    {
        IDependencyResolver dependencyResolver = this.GetDependencyResolver();
        IWizardService? ws = dependencyResolver.Resolve<IWizardService>();
        ArgumentNullException.ThrowIfNull(ws);
        return (await ws.ShowWizardAsync(wizard)).DialogResult;
    }

    private static IPropGroup BuildPropGroup(IPropWizard wizard, IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        var treeProp = new TreeProp();
        UpdateProp(treeProp, wizard, mappers);

        var propGroup = new PropGroup();
        propGroup.Props.Add(treeProp);
        return propGroup;
    }

    private static void UpdateProp(TreeProp treeProp, IPropWizard wizard, IReadOnlyList<IFeatureWizardDataMapper> mappers)
    {
        var page = (TreePropWizardPage)wizard.Pages.Single(p => p is TreePropWizardPage);
        treeProp.Name = page.Name;
        foreach (var mapper in mappers) mapper.ApplyTo(treeProp);
    }
}
