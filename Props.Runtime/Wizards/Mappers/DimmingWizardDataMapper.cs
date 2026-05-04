using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Runtime.Wizards.Pages;

namespace Props.Runtime.Wizards.Mappers;

public class DimmingWizardDataMapper(DimmingWizardPage page) : IFeatureWizardDataMapper
{
    public void ApplyTo(IProp prop)
    {
        if (prop is IHasDimming dimming)
        {
            dimming.Brightness = Math.Clamp(page.Brightness, 0, 100) / 100.0;
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
            page.Brightness = (int)Math.Round(Math.Clamp(dimming.Brightness, 0.0, 1.0) * 100);
            page.Gamma = dimming.Gamma;
        }
        else
        {
            throw new InvalidOperationException($"Prop {prop.GetType()} does not implement IHasDimming.");
        }
    }
}
