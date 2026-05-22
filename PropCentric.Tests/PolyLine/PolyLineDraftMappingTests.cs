using System.Collections.ObjectModel;
using PropCentric.Tests.Common;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
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
    }

    [Fact]
    public void PolyLinePropDraftMapper_ApplyDraft_CopiesConfiguredFieldsBackToProp()
    {
        var prop = PolyLineTestData.CreateTreeProp();
        var mapper = new PolyLinePropDraftMapper();
        var segments = PolyLineTestData.CreateSegments().Select(x =>
            new SegmentDraftState { Start = x.Start, End = x.End, PointCount = x.PointCount });
        var draft = new PolyLinePropDraft
        {
            Name = "PolyLine Draft",
            LightSize = 3,
            Segments = new ObservableCollection<SegmentDraftState>(segments)
        };

        mapper.ApplyDraft(draft, prop);

        Assert.Equal(draft.Name, prop.Name);
        Assert.Equal(draft.LightSize, prop.LightSize);
        AssertSegmentsEqual(draft.Segments, prop.Segments);
    }

    private static void AssertSegmentsEqual(IReadOnlyList<SegmentDraftState> expected, IReadOnlyList<Segment> actual)
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
