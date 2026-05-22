using System.Collections.ObjectModel;
using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Features;

/// <summary>
/// Marks a prop as supporting wizard-editable axis rotations and exposes the persisted rotation state.
/// </summary>
[PropFeature(PropFeatureFlags.Rotation)]
public interface ICanRotate
{
    /// <summary>
    /// Gets or sets the ordered axis rotations owned by the prop.
    /// </summary>
    ObservableCollection<AxisRotationModel> AxisRotations { get; set; }
}
