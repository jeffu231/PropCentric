using Props.Abstractions.Features;

namespace Props.Registry;

/// <summary>
/// Holds the registration metadata for a feature wizard page discovered by <see cref="FeatureWizardPageScanner"/>.
/// </summary>
public sealed record FeatureWizardPageDescriptor
{
    /// <summary>Gets the wizard page type decorated with <see cref="FeatureWizardPageAttribute"/>.</summary>
    /// <value>The <see cref="Type"/> of the page class.</value>
    public required Type PageType { get; init; }

    /// <summary>Gets the feature interface this page targets.</summary>
    /// <value>The <see cref="Type"/> of the feature interface (e.g., <c>typeof(IHasDimming)</c>).</value>
    public required Type FeatureInterface { get; init; }

    /// <summary>Gets the companion mapper type, or <see langword="null"/> if no mapper was declared.</summary>
    /// <value>The <see cref="Type"/> of an <see cref="IFeatureWizardDataMapper"/> implementation, or <see langword="null"/>.</value>
    public Type? MapperType { get; init; }

    /// <summary>Gets the display order of this page within the wizard.</summary>
    /// <value>A lower value causes the page to appear earlier in the wizard sequence.</value>
    public required int Priority { get; init; }
}
