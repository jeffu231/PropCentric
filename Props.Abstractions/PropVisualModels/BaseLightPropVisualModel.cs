namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// Base visual model for props that contain individually addressable light elements.
/// </summary>
/// <remarks>Extends <see cref="PropVisualModel"/> with the <see cref="ILightPropVisualModel"/> contract.</remarks>
public abstract class BaseLightPropVisualModel : PropVisualModel, ILightPropVisualModel
{
    // TODO determine what additional members are needed over PropVisualModel for light props.
}