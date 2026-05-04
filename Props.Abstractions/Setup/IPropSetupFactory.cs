using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

public interface IPropSetupFactory
{
    IPropSetup Create(Guid id);
    IPropSetup CreateFromCatalogItem(IPropCatalogItem item);
}