using System.Collections.ObjectModel;
using System.Numerics;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;

namespace Props.Runtime.Tree.Visuals;

public sealed class TreeVisualModelFactory : IPropVisualModelFactory<TreeVisualInput>
{
    public IPropVisualModel Create(TreeVisualInput input)
    {
        var points = GeneratePoints(input);
        var clouds = points
            .Select(p => new LightPointCloud { Points = p, PointSize = input.LightSize })
            .ToList();

        return new TreePropVisualModel
        {
            ReferencePoint = points[0][0].Position,
            Elements = clouds,
            AxisRotations = new ObservableCollection<AxisRotationModel>(input.AxisRotations)
        };
    }

    private static IReadOnlyList<List<LightPoint>> GeneratePoints(TreeVisualInput input)
    {
        var points = new List<List<LightPoint>>();
        for (int i = 0; i < input.Strings; i++)
        {
            points.Add(Enumerable.Range(0, input.NodesPerString)
                .Select(n => new LightPoint
                {
                    Position = new Vector3(0, n * 2, 0),
                    ElementId = Guid.NewGuid()
                })
                .ToList()
            );
        }
        return points;
    }
}
