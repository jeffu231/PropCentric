using System.Collections.ObjectModel;
using System.Numerics;
using PropCentric.Tests.Common;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.PolyLine.Visuals;

namespace PropCentric.Tests.PolyLine;

/// <summary>
/// Verifies cached preview rebuild behavior for polyline drafts.
/// </summary>
public class PolyLineWizardPreviewCoordinatorTests
{
    [Fact]
    public void BuildPreview_UnchangedInput_ReusesCachedModel()
    {
        var builder = new CountingBuilder();
        var coordinator = new PolyLineWizardPreviewCoordinator(new PolyLineDraftToVisualInputMapper(), builder);
        var draft = CreateDraft();

        var first = coordinator.BuildPreview(draft);
        var second = coordinator.BuildPreview(draft);

        Assert.Same(first, second);
        Assert.Equal(1, builder.CallCount);
    }

    [Fact]
    public void BuildPreview_ChangedSegmentPointCount_RebuildsModel()
    {
        var builder = new CountingBuilder();
        var coordinator = new PolyLineWizardPreviewCoordinator(new PolyLineDraftToVisualInputMapper(), builder);
        var draft = CreateDraft();

        var first = coordinator.BuildPreview(draft);
        draft.Segments[0].PointCount += 1;
        var second = coordinator.BuildPreview(draft);

        Assert.NotSame(first, second);
        Assert.Equal(2, builder.CallCount);
    }

    private static PolyLinePropDraft CreateDraft()
    {
        return new PolyLinePropDraft
        {
            Name = "Preview PolyLine",
            LightSize = 2,
            Segments = new ObservableCollection<SegmentDraftItem>(
                PolyLineTestData.CreateSegments().Select(segment => new SegmentDraftItem
                {
                    Start = segment.Start,
                    End = segment.End,
                    PointCount = segment.PointCount
                })),
            AxisRotations = TestDataHelper.CreateRotations((Axis.XAxis, 0), (Axis.YAxis, 0), (Axis.ZAxis, 0))
        };
    }

    private sealed class CountingBuilder : IPropVisualModelBuilder<PolyLineVisualInput, PolyLinePropVisualModel>
    {
        public int CallCount { get; private set; }

        public PolyLinePropVisualModel Create(PolyLineVisualInput input)
        {
            CallCount++;
            return new PolyLinePropVisualModel
            {
                StartingLightPoint = input.Segments.Count > 0
                    ? new LightPoint
                    {
                        Position = new Vector3(input.Segments[0].Start, 0f),
                        PointSize = input.LightSize,
                        ElementId = Guid.NewGuid()
                    }
                    : null,
                Elements = []
            };
        }
    }
}
