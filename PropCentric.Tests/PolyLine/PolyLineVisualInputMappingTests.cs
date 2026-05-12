using PropCentric.Tests.Common;
using Props.Abstractions.Features;
using Props.Abstractions.PropVisualModels;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.PolyLine.Visuals;

namespace PropCentric.Tests.PolyLine;

/// <summary>
/// Verifies polyline visual input mapping behavior.
/// </summary>
public class PolyLineVisualInputMappingTests
{
    [Fact]
    public void PolyLinePropToVisualInputMapper_Map_ProjectsConfiguredFields()
    {
        var prop = PolyLineTestData.CreateTreeProp();
        var mapper = new PolyLinePropToVisualInputMapper();

        var input = mapper.Map(prop);

        Assert.Equal(prop.LightSize, input.LightSize);
        AssertSegmentsEqual(prop.Segments, input.Segments);
        Assert.NotSame(prop.Segments, input.Segments);
        AssertRotationsEqual(prop.AxisRotations, input.AxisRotations);
        AssertRotationInstancesAreDistinct(prop.AxisRotations, input.AxisRotations);
    }

    [Fact]
    public void PolyLineDraftToVisualInputMapper_Map_ProjectsSegmentsInOrder()
    {
        var draft = new PolyLinePropDraft
        {
            LightSize = 4,
            Segments = new(
                PolyLineTestData.CreateSegments().Select(segment => new SegmentDraftState
                {
                    Start = segment.Start,
                    End = segment.End,
                    PointCount = segment.PointCount
                })),
            AxisRotations = TestDataHelper.CreateRotations((Axis.XAxis, 15), (Axis.ZAxis, 30))
        };

        var mapper = new PolyLineDraftToVisualInputMapper();

        var input = mapper.Map(draft);

        Assert.Equal(draft.LightSize, input.LightSize);
        Assert.Equal(draft.Segments.Count, input.Segments.Count);
        AssertRotationsEqual(draft.AxisRotations, input.AxisRotations);
        AssertRotationInstancesAreDistinct(draft.AxisRotations, input.AxisRotations);

        for (var index = 0; index < draft.Segments.Count; index++)
        {
            Assert.Equal(draft.Segments[index].Start, input.Segments[index].Start);
            Assert.Equal(draft.Segments[index].End, input.Segments[index].End);
            Assert.Equal(draft.Segments[index].PointCount, input.Segments[index].PointCount);
        }
    }

    private static void AssertSegmentsEqual(
        IReadOnlyList<Props.Abstractions.Props.Segment> expected,
        IReadOnlyList<Props.Abstractions.Props.Segment> actual)
    {
        Assert.Equal(expected.Count, actual.Count);

        for (var index = 0; index < expected.Count; index++)
        {
            Assert.Equal(expected[index], actual[index]);
        }
    }

    private static void AssertRotationsEqual(
        IReadOnlyList<AxisRotationModel> expected,
        IReadOnlyList<AxisRotationModel> actual)
    {
        Assert.Equal(expected.Count, actual.Count);

        for (var index = 0; index < expected.Count; index++)
        {
            Assert.Equal(expected[index].Axis, actual[index].Axis);
            Assert.Equal(expected[index].RotationAngle, actual[index].RotationAngle);
        }
    }

    private static void AssertRotationInstancesAreDistinct(
        IReadOnlyList<AxisRotationModel> expected,
        IReadOnlyList<AxisRotationModel> actual)
    {
        Assert.NotSame(expected, actual);

        for (var index = 0; index < expected.Count; index++)
        {
            Assert.NotSame(expected[index], actual[index]);
        }
    }
}
