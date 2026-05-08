using System.Reflection;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;

namespace Props.Registry;

/// <summary>
/// Scans assemblies for concrete classes decorated with <see cref="PropDescriptorAttribute"/>
/// that implement <see cref="IProp"/>, and returns a descriptor for each one.
/// </summary>
/// <remarks>Reflection runs once at startup; no runtime scanning occurs after initialization.</remarks>
public static class PropScanner
{
    /// <summary>Scans the provided assemblies and returns a descriptor for each discovered prop type.</summary>
    /// <param name="assemblies">The assemblies to inspect for decorated prop classes.</param>
    /// <returns>A read-only list of <see cref="PropDescriptor"/> records for all valid discovered props.</returns>
    public static IReadOnlyList<PropDescriptor> Scan(IEnumerable<Assembly> assemblies)
    {
        var descriptors = new List<PropDescriptor>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in SafeGetTypes(assembly))
            {
                if (!IsConcrete(type))
                    continue;
                
                if (!typeof(IProp).IsAssignableFrom(type))
                    continue;
                
                var attribute = type.GetCustomAttribute<PropDescriptorAttribute>();
                if (attribute != null)
                {
                    Validate(type, attribute);
                    descriptors.Add(new PropDescriptor
                    {
                        Id = attribute.Id,
                        Name = attribute.Name,
                        Icon = attribute.Icon,
                        PropType = type,
                        SetupType = attribute.SetupType
                    });
                    
                }
            }
        }

        return descriptors;
    }
    
    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
    }

    private static bool IsConcrete(Type t) => t is { IsAbstract: false, IsInterface: false };

    private static void Validate(Type type, Attribute attribute)
    {
        if (attribute is not PropDescriptorAttribute descriptor)
            throw new Exception("Invalid attribute");
        if (!typeof(IPropSetup).IsAssignableFrom(descriptor.SetupType))
        {
            throw new Exception($"Invalid WizardType in {type.Name}: {descriptor.SetupType?.Name}");
        }
    }
    
}