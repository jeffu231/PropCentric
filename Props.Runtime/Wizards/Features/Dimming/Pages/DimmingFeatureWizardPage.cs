using Catel.Data;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.Setup.Drafts;

namespace Props.Runtime.Wizards.Features.Dimming.Pages;

/// <summary>
/// Wizard page that captures brightness and gamma settings for props that implement <see cref="IHasDimming"/>.
/// </summary>
[FeatureWizardPage(typeof(IHasDimming), priority: 100)]
public class DimmingFeatureWizardPage : WizardPageBase, IFeatureWizardDraftPage
{
    private IHasDimmingSettingsDraft? _draft;

    /// <summary>
    /// Initializes a new instance of the <see cref="DimmingFeatureWizardPage"/> class.
    /// </summary>
    public DimmingFeatureWizardPage()
    {
        Title = "Brightness Level";
        Description = "Configure the brightness level";

        Brightness = 100;
        Gamma = 1;
    }

    /// <summary>Gets or sets the maximum brightness level as an integer percentage (0-100).</summary>
    public int Brightness
    {
        get => _draft is null ? GetValue<int>(BrightnessProperty) : (int)Math.Round(_draft.Brightness);
        set
        {
            var clamped = Math.Clamp(value, 0, 100);
            if (_draft is null)
            {
                SetValue(BrightnessProperty, clamped);
                return;
            }

            if (Math.Abs(_draft.Brightness - clamped) < double.Epsilon)
            {
                return;
            }

            _draft.Brightness = clamped;
            RaisePropertyChanged(nameof(Brightness));
        }
    }

    private static readonly IPropertyData BrightnessProperty = RegisterProperty<int>(nameof(Brightness));

    /// <summary>Gets or sets the gamma correction factor applied to the light output.</summary>
    public double Gamma
    {
        get => _draft?.Gamma ?? GetValue<double>(GammaProperty);
        set
        {
            if (_draft is null)
            {
                SetValue(GammaProperty, value);
                return;
            }

            if (Math.Abs(_draft.Gamma - value) < double.Epsilon)
            {
                return;
            }

            _draft.Gamma = value;
            RaisePropertyChanged(nameof(Gamma));
        }
    }

    private static readonly IPropertyData GammaProperty = RegisterProperty<double>(nameof(Gamma));

    /// <summary>
    /// Initializes the page with the shared dimming draft for the current wizard flow.
    /// </summary>
    /// <param name="context">The shared wizard context for the current wizard flow.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The shared draft does not implement <see cref="IHasDimmingSettingsDraft"/>.
    /// </exception>
    public void Initialize(FeatureWizardContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Draft is not IHasDimmingSettingsDraft dimmingDraft)
        {
            throw new InvalidOperationException(
                $"Draft {context.Draft.GetType()} does not implement {nameof(IHasDimmingSettingsDraft)}.");
        }

        _draft = dimmingDraft;
        RaisePropertyChanged(nameof(Brightness));
        RaisePropertyChanged(nameof(Gamma));
    }

    /// <inheritdoc />
    public override ISummaryItem GetSummary()
    {
        return new SummaryItem
        {
            Title = Title,
            Summary = $"Maximum Brightness: {Brightness}%\nGamma: {Gamma:0.0}"
        };
    }
}
