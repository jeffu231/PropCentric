using Props.Abstractions.Props;

namespace Props.Abstractions.Features;

public interface IFeatureWizardDataMapper
{
    void ApplyTo(IProp prop);
    void PopulateFrom(IProp prop);
}
