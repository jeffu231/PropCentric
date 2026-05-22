using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Vixen.Sys.Props;

namespace Props.Runtime.Wizards.Core.ViewModels;

public abstract class LightWizardPageViewModel<TWizardPage> : PropBaseWizardPageViewModel<TWizardPage>
    where TWizardPage : class, IWizardPage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <typeparam name="TWizardPage">Type of wizard page</typeparam>
    protected LightWizardPageViewModel(TWizardPage wizardPage) : base(wizardPage)
    {
    }

    /// <summary>
    /// Gets or sets the size of each light.
    /// </summary>
    /// <remarks>
    /// The size of the lights is constrained by <see cref="LightSizeMinimum"/> and <see cref="LightSizeMaximum"/>, consequently
    /// set these values prior to setting LightSize.
    /// </remarks>
    [ViewModelToModel]
    public int LightSize
    {
        get => GetValue<int>(LightSizeProperty);
        set => SetValue(LightSizeProperty, Math.Clamp(value, LightSizeMinimum, LightSizeMaximum));
    }

    private static readonly IPropertyData LightSizeProperty = RegisterProperty<int>(nameof(LightSize));

    [ViewModelToModel]
    public int LightSizeMinimum
    {
        get => GetValue<int>(LightSizeMinimumProperty);
        set
        {
            if (LightSize < value)
            {
                LightSize = value;
            }

            SetValue(LightSizeMinimumProperty, value);
        }
    }

    private static readonly IPropertyData LightSizeMinimumProperty = RegisterProperty<int>(nameof(LightSizeMinimum));

    [ViewModelToModel]
    public int LightSizeMaximum
    {
        get => GetValue<int>(LightSizeMaximumProperty);
        set
        {
            if (LightSize > value)
            {
                LightSize = value;
            }

            SetValue(LightSizeMaximumProperty, value);
        }
    }

    private static readonly IPropertyData LightSizeMaximumProperty = RegisterProperty<int>(nameof(LightSizeMaximum));
    
    [ViewModelToModel]
    public StringTypes StringType
    {
        get => GetValue<StringTypes>(StringTypeProperty);
        set => SetValue(StringTypeProperty, value);
    }

    private static readonly IPropertyData StringTypeProperty = RegisterProperty<StringTypes>(nameof(StringType));
}
