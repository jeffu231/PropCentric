using System.Reflection;

namespace Props.Registry;

public static class AssemblyLoader
{
    public static IReadOnlyList<Assembly> LoadAll(string directory)
    {
        var assemblies = new List<Assembly>();

        // 1. Entry assembly (your app)
        assemblies.Add(Assembly.GetExecutingAssembly());

        // 2. Already loaded (optional but useful)
        assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());

        // 3. Plugins
        if (Directory.Exists(directory))
        {
            Console.WriteLine($"Loading Props from '{directory}'");
            foreach (var file in Directory.GetFiles(directory, "*.dll"))
            {
                try
                {
                    var asm = Assembly.LoadFrom(file);
                    assemblies.Add(asm);
                }
                catch
                {
                    // log + continue (never crash startup on bad plugin)
                }
            }
        }

        return assemblies
            .Distinct()
            .ToList();
    }
}