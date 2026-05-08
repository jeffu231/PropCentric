namespace Props.Abstractions.Props;

/// <summary>
/// Marks a class as a discoverable prop and provides its catalog metadata.
/// </summary>
/// <remarks>
/// The plugin scanner discovers classes decorated with this attribute at startup via reflection.
/// No manual registration is required; the prop is automatically added to the catalog.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PropDescriptorAttribute : Attribute
{
    /// <summary>Gets the unique identifier for the prop type.</summary>
    /// <value>The parsed <see cref="Guid"/> representation of the string passed to the constructor.</value>
    public Guid Id { get; }

    /// <summary>Gets the human-readable display name of the prop.</summary>
    /// <value>The name shown in the prop catalog UI.</value>
    public string Name { get; }

    /// <summary>Gets the icon resource key or path for the prop.</summary>
    /// <value>An icon identifier, or an empty string if no icon was specified.</value>
    public string Icon { get; }

    /// <summary>Gets the type of the setup class that drives the prop's wizard.</summary>
    /// <value>The <see cref="Type"/> of an <see cref="IPropSetup"/> implementation.</value>
    public Type SetupType { get; }

    /// <summary>Initializes a new instance of the <see cref="PropDescriptorAttribute"/> class.</summary>
    /// <param name="id">A GUID string that uniquely identifies the prop type.</param>
    /// <param name="name">The human-readable display name shown in the catalog.</param>
    /// <param name="setupType">The setup class type that drives the prop's configuration wizard.</param>
    /// <param name="icon">An optional icon resource key or path.</param>
    public PropDescriptorAttribute(string id, string name, Type setupType, string? icon = null)
    {
        Id = Guid.Parse(id);
        Name = name;
        SetupType = setupType;
        Icon = icon ?? string.Empty;
    }
}