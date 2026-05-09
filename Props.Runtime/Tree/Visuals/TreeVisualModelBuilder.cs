using System.Numerics;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;

namespace Props.Runtime.Tree.Visuals;

/// <summary>
/// Generates a <see cref="TreePropVisualModel"/> from a <see cref="TreeVisualInput"/> by
/// computing the 3-D positions of all light points arranged in a conical tree shape.
/// </summary>
public sealed class TreeVisualModelBuilder : IPropVisualModelBuilder<TreeVisualInput, TreePropVisualModel>
{
    public TreePropVisualModel Create(TreeVisualInput input)
    {
        var points = GeneratePoints(input);
        var clouds = points
            .Select(p => new LightPointCloud { Points = p })
            .ToList();
        ApplyRotations(clouds, input.AxisRotations);
        return new TreePropVisualModel
        {
            StartingLightPoint = points[0][0],
            Elements = clouds
        };
    }

    private static IReadOnlyList<List<LightPoint>> GeneratePoints(TreeVisualInput input)
    {
        //TODO Determine what to do if this is a 2D vs 3D model. The TopWidth, BaseHeight and TopHeight are used to
        // simulate a 3D view in the current Vixen preview.
        const double maxWidth = 0.5;
        double topRadius    = input.TopRadius    / 100.0 * maxWidth;
        double bottomRadius = input.BottomRadius / 100.0 * maxWidth;
        double radiusDelta  = (bottomRadius - topRadius) / input.NodesPerString;

        var points = new List<List<LightPoint>>(input.Strings);
        for (int i = 0; i < input.Strings; i++)
        {
            double angle = (double)input.DegreesCoverage / input.Strings * i + input.DegreeOffset;
            points.Add(CreateStrand(input.NodesPerString, angle, bottomRadius, radiusDelta,
                                    yStart: -0.5, yDelta: 1.0 / input.NodesPerString, input.LightSize));
        }
        return points;
    }

    private static List<LightPoint> CreateStrand(
        int count, double angle, double startRadius, double radiusDelta,
        double yStart, double yDelta, float lightSize)
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
                PointSize = lightSize,
                ElementId = Guid.NewGuid()
            });
            radius -= radiusDelta;
            y      += yDelta;
        }
        return strand;
    }
    
    /// <summary>
    /// Apply rotation transforms to the model. 
    /// </summary>
    /// <param name="points"></param>
    /// <param name="rotations"></param>
    private void ApplyRotations(List<LightPointCloud> points, IReadOnlyList<AxisRotationModel> rotations)
    {
        //TODO Determine if this can be extracted to a base coordinator class and reused over
        // many model types.
        foreach (var rotation in rotations)
        {
            //TODO Transform the model geometry using the rotation model
        }
    }
}
