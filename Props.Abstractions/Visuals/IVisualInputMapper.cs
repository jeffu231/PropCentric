namespace Props.Abstractions.Visuals;

/// <summary>
/// Projects a source object onto a visual input record for geometry generation.
/// </summary>
/// <typeparam name="TSource">The source type — either a draft or a prop instance.</typeparam>
/// <typeparam name="TVisualInput">The target visual input record type.</typeparam>
/// <remarks>
/// Two implementations exist for each prop: one mapping from the draft (used by the wizard preview
/// coordinator) and one mapping from the prop (used by <c>BuildVisualModel()</c> at runtime).
/// </remarks>
public interface IVisualInputMapper<in TSource, out TVisualInput>
{
    /// <summary>Projects the source into a visual input record.</summary>
    /// <param name="source">The draft or prop instance to map from.</param>
    /// <returns>A visual input record whose fields drive the geometry factory.</returns>
    TVisualInput Map(TSource source);
}
