using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public interface ILightPropVisualModel: IPropVisualModel
{
    /// <summary>
    /// The canonical reference point of this prop in prop-space, explicitly set by the prop
    /// when building its visual model. The viewer decides how to visually express it
    /// (colored marker, axis indicator, arrow, etc.). Null if the prop has no meaningful reference point.
    /// </summary>
    Vector3? ReferencePoint { get; init; }
    
}