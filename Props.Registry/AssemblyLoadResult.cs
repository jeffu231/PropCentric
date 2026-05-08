using System.Reflection;

namespace Props.Registry;

/// <summary>
/// Captures the outcome of a plugin assembly load operation, separating successfully loaded assemblies
/// from those that could not be loaded.
/// </summary>
public sealed record AssemblyLoadResult
{
    /// <summary>Gets the assemblies that were successfully loaded.</summary>
    /// <value>A read-only list of <see cref="Assembly"/> instances, including the entry assembly and all plugins. The default is an empty list.</value>
    public IReadOnlyList<Assembly> Loaded { get; init; } = [];

    /// <summary>Gets the files that failed to load, along with the corresponding exceptions.</summary>
    /// <value>
    /// A read-only list of tuples where <c>File</c> is the full path of the DLL and <c>Error</c> is
    /// the exception thrown during load. The default is an empty list.
    /// </value>
    public IReadOnlyList<(string File, Exception Error)> Failures { get; init; } = [];
}
