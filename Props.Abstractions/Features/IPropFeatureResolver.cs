using Props.Abstractions.Props;

namespace Props.Abstractions.Features;

/// <summary>
/// Queries the feature capabilities of a prop instance.
/// </summary>
public interface IPropFeatureResolver
{
    /// <summary>Determines whether the prop implements the specified feature.</summary>
    /// <param name="prop">The prop to evaluate.</param>
    /// <param name="feature">A bitwise combination of the enumeration values that specifies the features to check.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="prop"/> supports all flags in <paramref name="feature"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool HasFeature(IProp prop, PropFeatureFlags feature);

    /// <summary>Returns the combined feature flags for the prop.</summary>
    /// <param name="prop">The prop to evaluate.</param>
    /// <returns>A bitwise combination of the enumeration values that specifies all features the prop supports.</returns>
    PropFeatureFlags GetFeatures(IProp prop);
}