using System.Reflection;

namespace Props.Registry;

public static class AssemblyLoader
{
    public static AssemblyLoadResult LoadAll(string directory)
    {
        var loaded = new List<Assembly>();
        var failures = new List<(string File, Exception Error)>();

        // 1. Entry assembly (your app)
        loaded.Add(Assembly.GetExecutingAssembly());

        // 2. Already loaded
        loaded.AddRange(AppDomain.CurrentDomain.GetAssemblies());

        // 3. Plugins
        if (Directory.Exists(directory))
        {
            foreach (var file in Directory.GetFiles(directory, "*.dll"))
            {
                try
                {
                    loaded.Add(Assembly.LoadFrom(file));
                }
                catch (Exception ex)
                {
                    failures.Add((file, ex));
                }
            }
        }

        return new AssemblyLoadResult
        {
            Loaded = loaded.Distinct().ToList(),
            Failures = failures
        };
    }
}