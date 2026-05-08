using System.Reflection;

namespace Props.Registry;

/// <summary>
/// Loads plugin assemblies from a directory and combines them with assemblies already in the current process.
/// </summary>
public static class AssemblyLoader
{
    /// <summary>
    /// Loads all <c>.dll</c> files from the specified directory and merges them with already-loaded assemblies.
    /// </summary>
    /// <param name="directory">The path to the plugin directory. Non-existent directories are silently ignored.</param>
    /// <returns>
    /// An <see cref="AssemblyLoadResult"/> containing the distinct set of successfully loaded assemblies
    /// and any files that could not be loaded.
    /// </returns>
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