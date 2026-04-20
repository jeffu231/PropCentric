using Props.Abstractions.Features;

namespace Props.Abstractions.Props;

public interface IPropCatalogItem
{
    Guid Id { get; }
    string Name { get; }
    string Icon { get; }
    Type WizardType { get; init; }
    Type PropType { get; init; }
    PropFeatureFlags Features { get; }
    bool SupportsEditing { get; init; }
}