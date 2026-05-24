using System.Collections.ObjectModel;
using System.Drawing;
using PropCentric.Tests.Common;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup.Drafts;
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
        AssertColorConfigurationsEqual(prop.ColorConfiguration, draft.ColorConfiguration);

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
            ColorConfiguration = new LightColorConfiguration(
                LightType.SingleColor,
                Color.Cyan,
                new DiscreteColorSetDefinition("RGB", [Color.Red, Color.Green, Color.Blue]),
                new FullColorOrderDefinition(
                    "GRB",
                    [LightColorChannel.Green, LightColorChannel.Red, LightColorChannel.Blue])),
            Segments = new ObservableCollection<SegmentDraftState>(segments)
        };

        mapper.ApplyDraft(draft, prop);

        Assert.Equal(draft.Name, prop.Name);
        Assert.Equal(draft.LightSize, prop.LightSize);
        AssertColorConfigurationsEqual(draft.ColorConfiguration, prop.ColorConfiguration);
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

    private static void AssertColorConfigurationsEqual(
        LightColorConfiguration expected,
        LightColorConfiguration actual)
    {
        Assert.Equal(expected.LightType, actual.LightType);
        Assert.Equal(expected.SingleColor, actual.SingleColor);
        Assert.Equal(expected.DiscreteColorSet?.Name, actual.DiscreteColorSet?.Name);
        Assert.Equal(expected.FullColorOrder?.Name, actual.FullColorOrder?.Name);
    }
}
