using System.Reflection;
using Props.Abstractions.Features;

namespace Props.Registry;

/// <summary>
/// Scans assemblies for classes decorated with <see cref="FeatureWizardPageAttribute"/> and returns
/// their registration descriptors.
/// </summary>
/// <remarks>Reflection runs once at startup; no runtime scanning occurs after initialization.</remarks>
public static class FeatureWizardPageScanner
{
    /// <summary>Scans the provided assemblies and returns a descriptor for each discovered feature wizard page.</summary>
    /// <param name="assemblies">The assemblies to scan. Only assemblies whose names start with <c>"Props"</c> are inspected.</param>
    /// <returns>A read-only list of <see cref="FeatureWizardPageDescriptor"/> records for all discovered pages.</returns>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="FeatureWizardPageAttribute.FeatureInterface"/> on a decorated class is not an interface type.
    /// </exception>
    public static IReadOnlyList<FeatureWizardPageDescriptor> Scan(IEnumerable<Assembly> assemblies)
    {
        var results = new List<FeatureWizardPageDescriptor>();
        Console.WriteLine("Scanning assemblies for features");
        foreach (var assembly in assemblies)
        {
            if(assembly.FullName != null && !assembly.FullName.StartsWith("Props")) continue;
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
