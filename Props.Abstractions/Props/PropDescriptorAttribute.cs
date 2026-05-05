namespace Props.Abstractions.Props;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PropDescriptorAttribute : Attribute 
{
    public Guid Id { get; }
    public string Name { get; }
    public string Icon { get; }
    public Type SetupType { get; }

    public PropDescriptorAttribute(string id, string name, Type setupType, string? icon = null)
    {
        Id = Guid.Parse(id);
        Name = name;
        SetupType = setupType;
        Icon = icon ?? string.Empty;
    }
}