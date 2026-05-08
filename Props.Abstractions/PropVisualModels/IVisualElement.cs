namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// Marker interface for a single renderable element within a <see cref="PropVisualModel"/>.
/// </summary>
/// <remarks>
/// Concrete types include <see cref="LightPointCloud"/>, <see cref="LightSegment"/>, and <see cref="PropMesh"/>.
/// The drawing engine dispatches on the concrete type to select the appropriate rendering path.
/// </remarks>
public interface IVisualElement
{
}