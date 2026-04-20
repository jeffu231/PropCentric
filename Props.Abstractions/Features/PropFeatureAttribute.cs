namespace Props.Abstractions.Features;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public sealed class PropFeatureAttribute : Attribute
{
    public PropFeatureFlags Flag { get; }

    public PropFeatureAttribute(PropFeatureFlags flag)
    {
        Flag = flag;
    }
}