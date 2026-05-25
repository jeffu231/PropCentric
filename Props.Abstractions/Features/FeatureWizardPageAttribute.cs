namespace Props.Abstractions.Features;

/// <summary>
/// Marks a wizard page class as a feature-scoped page that is automatically surfaced to any prop
/// that implements the specified feature interface.
/// </summary>
/// <remarks>
/// The <see cref="Props.Registry.FeatureWizardPageScanner"/> discovers decorated classes at startup.
/// Pages are ordered by <see cref="Priority"/> (ascending) within a prop's wizard.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FeatureWizardPageAttribute : Attribute
{
    /// <summary>Gets the feature interface this wizard page targets.</summary>
    /// <value>The <see cref="Type"/> of the feature interface (e.g., <c>typeof(IHasDimming)</c>).</value>
    public Type FeatureInterface { get; }

    /// <summary>Gets the display order of this page within the wizard.</summary>
    /// <value>A lower value causes the page to appear earlier. The default is <c>0</c>.</value>
    public int Priority { get; }

    /// <summary>Initializes a new instance of the <see cref="FeatureWizardPageAttribute"/> class.</summary>
    /// <param name="featureInterface">The feature interface type that this page targets.</param>
    /// <param name="priority">The display order within the wizard. Lower values appear first.</param>
    public FeatureWizardPageAttribute(Type featureInterface, int priority = 0)
    {
        FeatureInterface = featureInterface;
        Priority = priority;
    }
}
