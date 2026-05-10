using System.Collections.ObjectModel;
using AxisRotationModel = Props.Abstractions.PropVisualModels.AxisRotationModel;

namespace PropCentric.Tests.Common;

public static class TestDataHelper
{
    /// <summary>
    /// Creates an ordered collection of axis rotations.
    /// </summary>
    /// <param name="rotations">The ordered axis and angle values to include.</param>
    /// <returns>An ordered collection of <see cref="Props.Abstractions.PropVisualModels.AxisRotationModel"/> values.</returns>
    internal static ObservableCollection<AxisRotationModel> CreateRotations(params (Props.Abstractions.PropVisualModels.Axis Axis, int Angle)[] rotations)
    {
        return new ObservableCollection<AxisRotationModel>(
            rotations.Select(rotation => new AxisRotationModel
            {
                Axis = rotation.Axis,
                RotationAngle = rotation.Angle
            }));
    }
}