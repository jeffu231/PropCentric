using System.Reflection;
using Props.Abstractions.Features;

namespace Props.Registry;

public static class FeatureWizardPageScanner
{
    public static IReadOnlyList<FeatureWizardPageDescriptor> Scan(IEnumerable<Assembly> assemblies)
    {
        var results = new List<FeatureWizardPageDescriptor>();
        Console.WriteLine($"Scanning {assemblies.Count()} assemblies");
        foreach (var assembly in assemblies)
        {
            if(!assembly.FullName.StartsWith("Props")) continue;
            Console.WriteLine($"Scanning {assembly.FullName}");
            foreach (var type in assembly.GetExportedTypes())
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                var attr = type.GetCustomAttribute<FeatureWizardPageAttribute>();
                if (attr is null)
                    continue;

                if (!attr.FeatureInterface.IsInterface)
                {
                    Console.WriteLine( $"[FeatureWizardPage] on '{type.FullName}': '{attr.FeatureInterface.FullName}' must be an interface.");
                    throw new InvalidOperationException(
                        $"[FeatureWizardPage] on '{type.FullName}': '{attr.FeatureInterface.FullName}' must be an interface.");
                }
                Console.WriteLine($"Adding '{type.FullName}' to '{attr.FeatureInterface.FullName}'");
                results.Add(new FeatureWizardPageDescriptor
                {
                    PageType = type,
                    FeatureInterface = attr.FeatureInterface,
                    MapperType = attr.MapperType,
                    Priority = attr.Priority
                });
            }
        }

        return results;
    }
}
