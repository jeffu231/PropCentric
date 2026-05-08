using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

/// <summary>
/// Immutable catalog entry that exposes discovery metadata for a single prop type.
/// </summary>
public sealed record PropCatalogItem : IPropCatalogItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "Unnamed Prop";
    public string Icon { get; init; } = string.Empty;
    public required Type WizardType { get; init; }
    public required Type PropType { get; init; }
    public PropFeatureFlags Features { get; init; }
    public bool SupportsEditing { get; init; }
}