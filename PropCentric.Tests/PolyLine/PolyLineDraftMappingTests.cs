using System.Collections.ObjectModel;
using PropCentric.Tests.Common;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Props.Runtime.PolyLine.Setup;

namespace PropCentric.Tests.PolyLine;

public class PolyLineDraftMappingTests
{
    [Fact]
    public void PolyLinePropDraftMapper_PopulateDraft_CopiesConfiguredFields()
    {
        var prop = PolyLineTestData.CreateTreeProp();
        var mapper = new PolyLinePropDraftMapper();
        var draft = new PolyLinePropDraft();

        mapper.PopulateDraft(draft, prop);

        Assert.Equal(prop.Name, draft.Name);
        Assert.Equal(prop.LightSize, draft.LightSize);
       
        AssertSegmentsEqual(draft.Segments, prop.Segments);
        AssertRotationsEqual(prop.AxisRotations, draft.AxisRotations);
    }
    
    [Fact]
    public void PolyLinePropDraftMapper_ApplyDraft_CopiesConfiguredFieldsBackToProp()
    {
        var prop = PolyLineTestData.CreateTreeProp();
        var mapper = new PolyLinePropDraftMapper();
        var segments = PolyLineTestData.CreateSegments().Select(x =>
            new SegmentDraftItem { Start = x.Start, End = x.End, PointCount = x.PointCount });
        var draft = new PolyLinePropDraft
        {
            Name = "PolyLine Draft",
            LightSize = 3,
            Segments = new ObservableCollection<SegmentDraftItem>(segments),
            AxisRotations = TestDataHelper.CreateRotations((Axis.XAxis, 10), (Axis.YAxis, 20), (Axis.ZAxis, 30))
        };

        mapper.ApplyDraft(draft, prop);

        Assert.Equal(draft.Name, prop.Name);
        Assert.Equal(draft.LightSize, prop.LightSize);
        AssertSegmentsEqual(draft.Segments, prop.Segments);
        AssertRotationsEqual(draft.AxisRotations, prop.AxisRotations);
    }
    
    private static void AssertRotationsEqual(
        IReadOnlyList<AxisRotationModel> expected,
        IReadOnlyList<AxisRotationModel> actual)
    {
        Assert.Equal(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Axis, actual[i].Axis);
            Assert.Equal(expected[i].RotationAngle, actual[i].RotationAngle);
        }
    }

    private static void AssertSegmentsEqual(IReadOnlyList<SegmentDraftItem>  expected, IReadOnlyList<Segment> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Start, actual[i].Start);
            Assert.Equal(expected[i].End, actual[i].End);
            Assert.Equal(expected[i].PointCount, actual[i].PointCount);
        }
    }
}