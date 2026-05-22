using System.Collections.ObjectModel;

namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// Creates standard axis-rotation collections for props, drafts, and visual inputs.
/// </summary>
public static class AxisRotationCollectionFactory
{
    /// <summary>
    /// Creates the standard X/Y/Z zero-degree rotation collection used by rotating props.
    /// </summary>
    public static ObservableCollection<AxisRotationModel> CreateDefaultAxisRotations()
    {
        return
        [
            new AxisRotationModel { Axis = Axis.XAxis, RotationAngle = 0 },
            new AxisRotationModel { Axis = Axis.YAxis, RotationAngle = 0 },
            new AxisRotationModel { Axis = Axis.ZAxis, RotationAngle = 0 }
        ];
    }

    /// <summary>
    /// Clones a sequence of axis rotations into a new mutable collection with distinct model instances.
    /// </summary>
    public static ObservableCollection<AxisRotationModel> Clone(IEnumerable<AxisRotationModel> rotations)
    {
        ArgumentNullException.ThrowIfNull(rotations);

        return new ObservableCollection<AxisRotationModel>(
            rotations.Select(rotation => new AxisRotationModel
            {
                Axis = rotation.Axis,
                RotationAngle = rotation.RotationAngle
            }));
    }
}
