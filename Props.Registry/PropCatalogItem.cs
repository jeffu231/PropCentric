using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public sealed record PropCatalogItem:IPropCatalogItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "Unnamed Prop";
    public string Icon { get; init; }
    public required Type WizardType { get; init; }
    public required Type PropType { get; init; }
    public PropFeatureFlags Features { get; init; }
    public bool SupportsEditing { get; init; }
}