using System.Numerics;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;

namespace Props.Runtime.PolyLine.Visuals;

/// <summary>
/// Builds rendered segment geometry from <see cref="PolyLineVisualInput"/>.
/// </summary>
public sealed class PolyLineVisualModelBuilder : IPropVisualModelBuilder<PolyLineVisualInput, PolyLinePropVisualModel>
{
    private const float PositionTolerance = 1e-5f;

    public PolyLinePropVisualModel Create(PolyLineVisualInput input)
    {
        var segments = BuildSegments(input);
        var rotatedSegments = ApplyRotations(segments, input.AxisRotations);

        return new PolyLinePropVisualModel
        {
            StartingLightPoint = rotatedSegments.FirstOrDefault()?.Lights.FirstOrDefault(),
            Elements = rotatedSegments
        };
    }

    private static IReadOnlyList<LightSegment> BuildSegments(PolyLineVisualInput input)
    {
        var segments = new List<LightSegment>(input.Segments.Count);

        for (var index = 0; index < input.Segments.Count; index++)
        {
            var logicalSegment = input.Segments[index];
            var lights = CreateLightPoints(logicalSegment, input.LightSize);

            if (index > 0 && lights.Count > 0 && segments.Count > 0)
            {
                var previousLastLight = segments[^1].Lights.LastOrDefault();
                if (IsSamePosition(previousLastLight.Position, lights[0].Position))
                {
                    lights.RemoveAt(0);
                }
            }

            segments.Add(new LightSegment
            {
                Start = ToVector3(logicalSegment.Start),
                End = ToVector3(logicalSegment.End),
                Lights = lights,
                PointSize = input.LightSize
            });
        }

        return segments;
    }

    private static List<LightPoint> CreateLightPoints(Props.Abstractions.Props.Segment segment, int lightSize)
    {
        var points = new List<LightPoint>(segment.PointCount);
        var start = ToVector3(segment.Start);
        var end = ToVector3(segment.End);

        if (segment.PointCount == 1)
        {
            points.Add(new LightPoint
            {
                Position = Vector3.Lerp(start, end, 0.5f),
                PointSize = lightSize,
                ElementId = Guid.NewGuid()
            });

            return points;
        }

        for (var index = 0; index < segment.PointCount; index++)
        {
            var t = index / (float)(segment.PointCount - 1);
            points.Add(new LightPoint
            {
                Position = Vector3.Lerp(start, end, t),
                PointSize = lightSize,
                ElementId = Guid.NewGuid()
            });
        }

        return points;
    }

    private static IReadOnlyList<LightSegment> ApplyRotations(
        IReadOnlyList<LightSegment> segments,
        IReadOnlyList<AxisRotationModel> rotations)
    {
        if (rotations.Count == 0)
        {
            return segments;
        }

        var quaternion = rotations.Aggregate(Quaternion.Identity, (current, rotation) =>
            Quaternion.Normalize(
                current * Quaternion.CreateFromAxisAngle(
                    rotation.Axis switch
                    {
                        Axis.YAxis => Vector3.UnitY,
                        Axis.ZAxis => Vector3.UnitZ,
                        _ => Vector3.UnitX
                    },
                    rotation.RotationAngle * (MathF.PI / 180f))));

        return segments.Select(segment => new LightSegment
        {
            Start = Vector3.Transform(segment.Start, quaternion),
            End = Vector3.Transform(segment.End, quaternion),
            PointSize = segment.PointSize,
            Lights = segment.Lights.Select(light => new LightPoint
            {
                Position = Vector3.Transform(light.Position, quaternion),
                PointSize = light.PointSize,
                ElementId = light.ElementId
            }).ToArray()
        }).ToArray();
    }

    private static Vector3 ToVector3(Vector2 point) => new(point.X, point.Y, 0f);

    private static bool IsSamePosition(Vector3 left, Vector3 right) =>
        Vector3.Distance(left, right) <= PositionTolerance;
}
