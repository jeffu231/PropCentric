using Props.Abstractions.Features;

namespace Props.Registry;

public sealed record FeatureWizardPageDescriptor
{
    public required Type PageType { get; init; }
    public required Type FeatureInterface { get; init; }
    public Type? MapperType { get; init; }
    public required int Priority { get; init; }
}
