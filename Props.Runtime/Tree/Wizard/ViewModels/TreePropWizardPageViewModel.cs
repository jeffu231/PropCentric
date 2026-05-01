using Catel.Data;
using Props.Runtime.Tree.Wizard.Pages;
using Props.Runtime.Wizards.ViewModels;

namespace Props.Runtime.Tree.Wizard.ViewModels;

public class TreePropWizardPageViewModel: LightWizardPageViewModel<TreePropWizardPage, TreePropVisualModel>, IPropWizardPageViewModel
{
    public TreePropWizardPageViewModel(TreePropWizardPage wizardPage) : base(wizardPage)
    {
    }
    
    #region Strings property

    /// <summary>
    /// Gets or sets the Strings value.
    /// </summary>
    //[ViewModelToModel]
    public int Strings
    {
        get { return GetValue<int>(NodeCountProperty); }
        set
        {
            SetValue(NodeCountProperty, value);
            //RefreshGraphics();
        }
    }
    public static readonly IPropertyData NodeCountProperty = RegisterProperty<int>(nameof(Strings), 16);

    #endregion
    
    #region NodesPerString property

    /// <summary>
    /// Gets or sets the NodesPerString value.
    /// </summary>
    //[ViewModelToModel]
    public int NodesPerString
    {
        get { return GetValue<int>(NodesPerStringProperty); }
        set
        {
            SetValue(NodesPerStringProperty, value);
            //RefreshGraphics();
        }
    }

    /// <summary>
    /// NodesPerString property data.
    /// </summary>
    public static readonly IPropertyData NodesPerStringProperty = RegisterProperty<int>(nameof(NodesPerString), 50);

    #endregion
    
    #region Protected Methods

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);
			
        if (Strings <= 0)
        {
            validationResults.Add(
                FieldValidationResult.CreateError(nameof(Strings), "String Count must be greater than 0"));
        }

        if (NodesPerString <= 0)
        {
            validationResults.Add(FieldValidationResult.CreateError(nameof(NodesPerString),
                "Nodes per string must be greater than 0"));

        }
    }
    
    #endregion
}