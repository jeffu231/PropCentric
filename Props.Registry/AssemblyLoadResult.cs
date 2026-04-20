using System.Reflection;

namespace Props.Registry;

public sealed record AssemblyLoadResult
{
    public IReadOnlyList<Assembly> Loaded { get; init; } = [];
    public IReadOnlyList<(string File, Exception Error)> Failures { get; init; } = [];
}
