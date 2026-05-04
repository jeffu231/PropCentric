namespace Props.Abstractions.Features;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FeatureWizardPageAttribute : Attribute
{
    public Type FeatureInterface { get; }
    public Type? MapperType { get; }
    public int Priority { get; }

    public FeatureWizardPageAttribute(Type featureInterface, Type? mapperType = null, int priority = 0)
    {
        FeatureInterface = featureInterface;
        MapperType = mapperType;
        Priority = priority;
    }
}
