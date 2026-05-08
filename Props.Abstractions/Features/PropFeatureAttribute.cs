namespace Props.Abstractions.Features;

/// <summary>
/// Marks a feature interface with the <see cref="PropFeatureFlags"/> value it represents.
/// </summary>
/// <remarks>
/// <see cref="Props.Registry.PropFeatureInferrer"/> reads this attribute at startup to automatically
/// compute the combined feature flags for any prop type without manual registration.
/// Apply this attribute to each <c>IHas*</c> feature interface.
/// </remarks>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public sealed class PropFeatureAttribute : Attribute
{
    /// <summary>Gets the feature flag associated with the decorated interface.</summary>
    /// <value>One of the <see cref="PropFeatureFlags"/> enumeration values that identifies the feature.</value>
    public PropFeatureFlags Flag { get; }

    /// <summary>Initializes a new instance of the <see cref="PropFeatureAttribute"/> class.</summary>
    /// <param name="flag">One of the enumeration values that specifies the feature this interface represents.</param>
    public PropFeatureAttribute(PropFeatureFlags flag)
    {
        Flag = flag;
    }
}