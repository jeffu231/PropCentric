using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Abstractions.Wizards;

public interface IWizardFactory
{
    IPropSetupWizard Create(Guid id);
    IPropSetupWizard CreateWizard(IPropCatalogItem item);
    IPropSetupWizard CreateWizardFor(IProp prop);
    IPropSetupWizard<TProp> CreateWizard<TProp>() where TProp : IProp;
    IEnumerable<IPropCatalogItem> GetPropCatalog();
    IEnumerable<IPropCatalogItem> GetPropCatalogByFeature(PropFeatureFlags flags);
}