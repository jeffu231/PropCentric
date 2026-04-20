using Props.Abstractions.Features;
using Props.Abstractions.Props;

namespace Props.Registry;

public sealed class PropDescriptor
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Icon { get; init; } //Use real icon class
    public required Type PropType { get; init; }
    //public required IPropMetaData Meta { get; init; }
    public required Type WizardType { get; init; }
    public PropFeatureFlags Flags { get; set; } = PropFeatureFlags.None;
}