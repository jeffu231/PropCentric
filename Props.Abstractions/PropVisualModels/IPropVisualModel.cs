using System.Collections.ObjectModel;
using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public interface IPropVisualModel
{
    /// <summary>
    /// Unique Id of the prop model.
    /// </summary>
    Guid Id { get; init; }
    
    IReadOnlyList<IVisualElement> Elements { get; init; }
    
    /// <summary>
    /// Collection of axis rotations.
    /// </summary>
    ObservableCollection<AxisRotationModel> AxisRotations { get; set; }
}
