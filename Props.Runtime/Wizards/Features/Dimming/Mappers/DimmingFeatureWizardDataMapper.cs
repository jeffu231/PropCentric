using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Runtime.Wizards.Features.Dimming.Pages;

namespace Props.Runtime.Wizards.Features.Dimming.Mappers;

/// <summary>
/// Transfers brightness and gamma values between a <see cref="DimmingFeatureWizardPage"/> and an
/// <see cref="IHasDimming"/> prop, converting between the page's integer percentage and the
/// prop's normalized double representation.
/// </summary>
public class DimmingFeatureWizardDataMapper(DimmingFeatureWizardPage page) : IFeatureWizardDataMapper
{
    public void ApplyTo(IProp prop)
    {
        if (prop is IHasDimming dimming)
        {
            dimming.Brightness = Math.Clamp(page.Brightness, 0, 100);
            dimming.Gamma = page.Gamma;
        }
        else
        {
            throw new InvalidOperationException($"Prop {prop.GetType()} does not implement IHasDimming.");
        }
    }

    public void PopulateFrom(IProp prop)
    {
        if (prop is IHasDimming dimming)
        {
            page.Brightness = (int)dimming.Brightness;
            page.Gamma = dimming.Gamma;
        }
        else
        {
            throw new InvalidOperationException($"Prop {prop.GetType()} does not implement IHasDimming.");
        }
    }
}
