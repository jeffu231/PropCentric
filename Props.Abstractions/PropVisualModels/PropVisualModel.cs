using System.Collections.ObjectModel;
using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

/// <summary>
/// Base implementation of <see cref="IPropVisualModel"/> that provides identity, elements,
/// axis rotations, and an optional reference point.
/// </summary>
public abstract class PropVisualModel : IPropVisualModel
{
    /// <inheritdoc/>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <inheritdoc/>
    public IReadOnlyList<IVisualElement> Elements { get; init; } = [];

    /// <inheritdoc/>
    public ObservableCollection<AxisRotationModel> AxisRotations { get; set; }

    /// <inheritdoc/>
    public Vector3? ReferencePoint { get; init; }
}
