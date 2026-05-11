using Props.Runtime.PolyLine.Setup;
using Props.Runtime.PolyLine.Visuals;
using PropCentric.Tests.Common;
using Props.Abstractions.PropVisualModels;

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
        Assert.Same(prop.Segments, input.Segments);
        Assert.Same(prop.AxisRotations, input.AxisRotations);
    }

    [Fact]
    public void PolyLineDraftToVisualInputMapper_Map_ProjectsSegmentsInOrder()
    {
        var draft = new PolyLinePropDraft
        {
            LightSize = 4,
            Segments = new(
                PolyLineTestData.CreateSegments().Select(segment => new SegmentDraftItem
                {
                    Start = segment.Start,
                    End = segment.End,
                    PointCount = segment.PointCount
                })),
            AxisRotations = TestDataHelper.CreateRotations((Axis.XAxis, 15), (Axis.ZAxis, 30))
        };

        var mapper = new PolyLinePropDraftToVisualInputMapper();

        var input = mapper.Map(draft);

        Assert.Equal(draft.LightSize, input.LightSize);
        Assert.Same(draft.AxisRotations, input.AxisRotations);
        Assert.Equal(draft.Segments.Count, input.Segments.Count);

        for (var index = 0; index < draft.Segments.Count; index++)
        {
            Assert.Equal(draft.Segments[index].Start, input.Segments[index].Start);
            Assert.Equal(draft.Segments[index].End, input.Segments[index].End);
            Assert.Equal(draft.Segments[index].PointCount, input.Segments[index].PointCount);
        }
    }
}
