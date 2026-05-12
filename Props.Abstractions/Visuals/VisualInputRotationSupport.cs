using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Visuals;

/// <summary>
/// Shared helpers for visual-input pipelines that include axis rotation state.
/// </summary>
public static class VisualInputRotationSupport
{
    /// <summary>
    /// Creates a value snapshot of the supplied rotation models so later in-place edits
    /// do not mutate an existing visual-input record.
    /// </summary>
    public static IReadOnlyList<AxisRotationModel> SnapshotRotations(IEnumerable<AxisRotationModel> rotations)
    {
        return rotations.Select(rotation => new AxisRotationModel
        {
            Axis = rotation.Axis,
            RotationAngle = rotation.RotationAngle
        }).ToArray();
    }

    /// <summary>
    /// Compares two ordered rotation collections by axis and angle values.
    /// </summary>
    public static bool RotationsEqual(
        IReadOnlyList<AxisRotationModel> left,
        IReadOnlyList<AxisRotationModel> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (left[index].Axis != right[index].Axis || left[index].RotationAngle != right[index].RotationAngle)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Adds ordered rotation values to an existing hash-code accumulator.
    /// </summary>
    public static void AddRotationsToHashCode(ref HashCode hash, IReadOnlyList<AxisRotationModel> rotations)
    {
        foreach (var rotation in rotations)
        {
            hash.Add(rotation.Axis);
            hash.Add(rotation.RotationAngle);
        }
    }
}
