using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;

namespace Props.Runtime.Wizards.Core.ViewModels;

public class PropBaseWizardPageViewModel<TWizardPage> : GraphicsWizardPageViewModelBase<TWizardPage>
    where TWizardPage : class, IWizardPage
{
    protected PropBaseWizardPageViewModel(TWizardPage wizardPage) : base(wizardPage)
    {
    }

    #region Properties

    /// <summary>
    /// Gets or sets the prop name.
    /// </summary>
    [ViewModelToModel]
    public string Name
    {
        get { return GetValue<string>(NameProperty); }
        set { SetValue(NameProperty, value); }
    }

    private static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

    #endregion

    /// <summary>
    /// Performs validation on the properties.
    /// </summary>
    /// <param name="validationResults"></param>
    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);

        if (string.IsNullOrWhiteSpace(Name))
        {
            validationResults.Add(FieldValidationResult.CreateError("Name", "Name is required"));
        }
    }
}
