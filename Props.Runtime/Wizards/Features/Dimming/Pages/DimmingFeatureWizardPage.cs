using Catel.Data;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Runtime.Wizards.Features.Dimming.Mappers;

namespace Props.Runtime.Wizards.Features.Dimming.Pages;

/// <summary>
/// Wizard page that captures brightness and gamma settings for props that implement <see cref="IHasDimming"/>.
/// </summary>
[FeatureWizardPage(typeof(IHasDimming), mapperType: typeof(DimmingFeatureWizardDataMapper), priority: 100)]
public class DimmingFeatureWizardPage : WizardPageBase
{
    #region Constructors
    public DimmingFeatureWizardPage()
    {
        // Set with some default parameters
        Title = "Brightness Level";
        Description = "Configure the brightness level";
			
        Brightness = 100;
        Gamma = 1;
    }
    #endregion

    #region Public Properties
    /// <summary>Gets or sets the maximum brightness level as an integer percentage (0–100).</summary>
    public int Brightness
    {
        get => GetValue<int>(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }
    private static readonly IPropertyData BrightnessProperty = RegisterProperty<int>(nameof(Brightness));

    /// <summary>Gets or sets the gamma correction factor applied to the light output.</summary>
    public double Gamma
    {
        get => GetValue<double>(GammaProperty);
        set => SetValue(GammaProperty, value);
    }
    private static readonly IPropertyData GammaProperty = RegisterProperty<double>(nameof(Gamma));

    // private PropType _propType;
    // public PropType PropType
    // {
    //     set { 
    //         _propType = value;
    //         Description += $" for {_propType.GetEnumDescription()}";
    //     }
    // }
    #endregion

    #region Base class overrides
    public override ISummaryItem GetSummary()
    {
        return new SummaryItem
        {
            Title = this.Title,
            Summary = $"Maximum Brightness: {Brightness}%\nGamma: {Gamma:0.0}"
        };
    }
    #endregion

}