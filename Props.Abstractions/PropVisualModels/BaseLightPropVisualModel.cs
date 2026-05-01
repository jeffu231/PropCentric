using System.Collections.ObjectModel;
using System.Numerics;

namespace Props.Abstractions.PropVisualModels;

public class BaseLightPropVisualModel: ILightPropVisualModel
{
    public IReadOnlyList<IVisualElement> Elements { get; init; }
    public ObservableCollection<AxisRotationModel> AxisRotations { get; set; }
    public Vector3? ReferencePoint { get; init; }
}