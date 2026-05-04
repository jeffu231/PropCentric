using System.Collections.ObjectModel;
using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public abstract class PropVisualModel : IPropVisualModel
{
    public Guid Id { get; init; }  = Guid.NewGuid();
    public IReadOnlyList<IVisualElement> Elements { get; init; } = [];
    public ObservableCollection<AxisRotationModel> AxisRotations { get; set; }
    public Vector3? ReferencePoint { get; init; }
}
