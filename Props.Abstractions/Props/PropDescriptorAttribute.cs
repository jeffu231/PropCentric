namespace Props.Abstractions.Props;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PropDescriptorAttribute : Attribute 
{
    public Guid Id { get; }
    public string Name { get; }
    public string Icon { get; }
    public Type WizardType { get; }
    public Type VisualModelType { get; }

    public PropDescriptorAttribute(string id, string name, Type wizardType, Type visualModelType, string? icon = null)
    {
        Id = Guid.Parse(id);
        Name = name;
        WizardType = wizardType;
        VisualModelType = visualModelType;
        Icon = icon??string.Empty;
    }
}