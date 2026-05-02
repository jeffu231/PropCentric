using Catel.IoC;
using Catel.Services;
using Orc.Theming;
using Orc.Wizard;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Runtime.Tree.Wizard;
using Props.Runtime.Tree.Wizard.Pages;
using Props.Runtime.Wizards;

namespace Props.Runtime.Tree;

public class TreePropSetup : IPropSetup
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
        var treeWizard = CreateTreeWizard();

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
            return BuildPropGroup(treeWizard);

        return null;
    }

    private async Task EditWizard(TreeProp treeProp)
    {
        var treeWizard = CreateTreeWizard();

        // Pre-populate wizard with the existing prop's current values
        var page = (TreePropWizardPage)treeWizard.Pages.Single(p => p is TreePropWizardPage);
        page.Name = treeProp.Name;

        bool? result = await ShowWizard(treeWizard);
        if (result.HasValue && result.Value)
            UpdateProp(treeProp, treeWizard);
    }

    private TreePropWizard CreateTreeWizard()
    {
        IDependencyResolver dependencyResolver = this.GetDependencyResolver();
        IMessageService? ms = dependencyResolver.Resolve<IMessageService>();
        IBaseColorSchemeService? baseColorService = (IBaseColorSchemeService?)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));
        ITypeFactory typeFactory = this.GetTypeFactory();

        ArgumentNullException.ThrowIfNull(ms);
        ArgumentNullException.ThrowIfNull(baseColorService);

        baseColorService.SetBaseColorScheme("Dark");

        var wizard = new TreePropWizard(typeFactory, ms);
        
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

    private static IPropGroup BuildPropGroup(IPropWizard wizard)
    {
        var treeProp = new TreeProp();
        UpdateProp(treeProp, wizard);

        var propGroup = new PropGroup();
        propGroup.Props.Add(treeProp);
        return propGroup;
    }

    private static void UpdateProp(TreeProp treeProp, IPropWizard wizard)
    {
        var page = (TreePropWizardPage)wizard.Pages.Single(p => p is TreePropWizardPage);
        treeProp.Name = page.Name;
    }
}