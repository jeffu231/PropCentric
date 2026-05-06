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
        const double maxWidth = 0.5;
        double topRadius    = input.TopRadius    / 100.0 * maxWidth;
        double bottomRadius = input.BottomRadius / 100.0 * maxWidth;
        double radiusDelta  = (bottomRadius - topRadius) / input.NodesPerString;

        var points = new List<List<LightPoint>>(input.Strings);
        for (int i = 0; i < input.Strings; i++)
        {
            double angle = (double)input.DegreesCoverage / input.Strings * i + input.DegreeOffset;
            points.Add(CreateStrand(input.NodesPerString, angle, bottomRadius, radiusDelta,
                                    yStart: -0.5, yDelta: 1.0 / input.NodesPerString));
        }
        return points;
    }

    private static List<LightPoint> CreateStrand(
        int count, double angle, double startRadius, double radiusDelta,
        double yStart, double yDelta)
    {
        double radians = angle * Math.PI / 180.0;
        double y      = yStart;
        double radius = startRadius;
        var strand = new List<LightPoint>(count);

        for (int p = 0; p < count; p++)
        {
            strand.Add(new LightPoint
            {
                Position  = new Vector3((float)(Math.Cos(radians) * radius),
                                        (float)y,
                                        (float)(Math.Sin(radians) * radius)),
                ElementId = Guid.NewGuid()
            });
            radius -= radiusDelta;
            y      += yDelta;
        }
        return strand;
    }
}
