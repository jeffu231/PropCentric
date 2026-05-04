using System.Reflection;
using Props.Abstractions.Features;

namespace Props.Registry;

public static class FeatureWizardPageScanner
{
    public static IReadOnlyList<FeatureWizardPageRegistration> Scan(IEnumerable<Assembly> assemblies)
    {
        var results = new List<FeatureWizardPageRegistration>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                var attr = type.GetCustomAttribute<FeatureWizardPageAttribute>();
                if (attr is null)
                    continue;

                if (!attr.FeatureInterface.IsInterface)
                    throw new InvalidOperationException(
                        $"[FeatureWizardPage] on '{type.FullName}': '{attr.FeatureInterface.FullName}' must be an interface.");

                results.Add(new FeatureWizardPageRegistration
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
