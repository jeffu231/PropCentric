using System.Reflection;
using Props.Abstractions;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Wizards;

namespace Props.Registry;

public static class PropScanner
{
    public static IReadOnlyList<PropDescriptor> Scan(IEnumerable<Assembly> assemblies)
    {
        var descriptors = new List<PropDescriptor>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in SafeGetTypes(assembly))
            {
                var typesWithAttribute = assembly.GetTypes()
                    .Where(t => t.IsClass 
                                && t.IsDefined(typeof(PropDescriptorAttribute), inherit: false))
                    .ToList();
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
                        WizardType = attribute.WizardType
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
        if (!typeof(IPropSetupWizard).IsAssignableFrom(descriptor.WizardType))
        {
            throw new Exception($"Invalid WizardType in {type.Name}: {descriptor.WizardType?.Name}");
        }

        if (!typeof(IPropVisualModel).IsAssignableFrom(descriptor.VisualModelType))
        {
            throw new Exception($"Invalid VisualModelType in {type.Name}: {descriptor.VisualModelType?.Name}");
        }
    }
    
}