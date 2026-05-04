using Orc.Wizard;

namespace Props.Abstractions.Features;

public interface IFeatureWizardPageResolver
{
    IReadOnlyList<IWizardPage> GetPagesFor(Type propType);
    IReadOnlyList<IFeatureWizardDataMapper> GetMappersFor(IReadOnlyList<IWizardPage> pages);
}
