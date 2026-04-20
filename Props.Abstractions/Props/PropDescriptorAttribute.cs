namespace Props.Abstractions.Props;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PropDescriptorAttribute : Attribute 
{
    public Guid Id { get; }
    public string Name { get; }
    public string Icon { get; }
    public Type WizardType { get; }

    public PropDescriptorAttribute(string id, string name, Type wizardType, string? icon = null)
    {
        Id = Guid.Parse(id);
        Name = name;
        WizardType = wizardType;
        Icon = icon ?? string.Empty;
    }
}