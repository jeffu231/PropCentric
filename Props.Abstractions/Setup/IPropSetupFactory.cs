using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

public interface IPropSetupFactory
{
    IPropSetup Create(Guid id);
    IPropSetup CreateSetup(IPropCatalogItem item);
    IPropSetup CreateSetupFor(IProp prop);
}