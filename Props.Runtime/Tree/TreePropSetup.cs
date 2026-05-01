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

public class TreePropSetup: IPropSetup<TreeProp>
{
   public Task<TreeProp> EditAsync(TreeProp existing)
    {
        return Task.FromResult(existing);
    }

    async Task<IPropGroup<TreeProp>?> IPropSetup<TreeProp>.CreateAsync()
    {
       return await CreateWizard<TreeProp>();
    }

    public Task<IProp> EditAsync(IProp existing)
    {
        //Do the actual editing here
        return Task.FromResult<IProp>(existing);
    }

    public Task<IPropGroup<IProp>?> CreateAsync()
    {
        return CreateWizard<IProp>();
    }

    private async Task<IPropGroup<T>?> CreateWizard<T>() where T : IProp
    {
        // Get the Catel dependency resolver
        IDependencyResolver dependencyResolver = this.GetDependencyResolver();
        
        // Create the Catel Wizard service
        IWizardService? ws = dependencyResolver.Resolve<IWizardService>();
        IMessageService? ms = dependencyResolver.Resolve<IMessageService>();
        ArgumentNullException.ThrowIfNull(ws);
        ArgumentNullException.ThrowIfNull(ms);
        // Get the Catel type factory
        ITypeFactory typeFactory = this.GetTypeFactory();
        
        // Retrieve the color scheme service
        IBaseColorSchemeService? baseColorService = (IBaseColorSchemeService?)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));

        ArgumentNullException.ThrowIfNull(baseColorService);

        // Select the dark color scheme
        baseColorService.SetBaseColorScheme("Dark");
        var treeWizard = new TreePropWizard(typeFactory, ms);
        
        // Configure the wizard window to show up in the Windows task bar
        treeWizard.ShowInTaskbarWrapper = true;

        // Enable the help button
        treeWizard.ShowHelpWrapper = true;

        // Configure the wizard to allow the user to jump between already visited pages
        treeWizard.AllowQuickNavigationWrapper = true;

        // Allow Catel to help determine when it is safe to transition to the next wizard page
        treeWizard.HandleNavigationStatesWrapper = true;

        // Configure the wizard to NOT cache views
        treeWizard.CacheViewsWrapper = false;
        
        // Configure the wizard with a navigation controller														
        treeWizard.NavigationControllerWrapper = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PropWizardNavigationController>(treeWizard);
        
        // If the Catel Wizard service was successfully created and
        // the prop specific wizard was successfully created then...
        
        // Show the Prop Wizard
        bool? result = (await ws.ShowWizardAsync(treeWizard)).DialogResult;

        // Determine if the wizard was cancelled 
        if (result.HasValue && result.Value)
        {
            // Have the prop factory create the props from the wizard data
            IPropGroup<T> propGroup = GetProps<T>(treeWizard);

            // User did not cancel					
            return propGroup;
        }
        
        // Indicate the user cancelled
        return null;
    }

    private static IPropGroup<T> GetProps<T>(IPropWizard wizard) where T : IProp
    {
        var treeProp = new TreeProp();
        // Transfer the data from the wizard into the tree prop
        UpdateProp(treeProp, wizard);

        // Create the collection of props to return 
        var propGroup = new PropGroup<T>();

        // Add the Tree to the prop collections 
        if (treeProp is T prop)
        {
            propGroup.Props.Add(prop);
        }
        
        //Check to see if we need to create groups of this Prop
        //CreateGroup(treeProp, propGroup);

        // Return the collection of props
        return propGroup;
    }

    private static void UpdateProp(TreeProp treeProp, IPropWizard wizard)
    {
        TreePropWizardPage treePropPage = (TreePropWizardPage)wizard.Pages.Single(page => page is TreePropWizardPage);
        treeProp.Name = treePropPage.Name;
    }
}