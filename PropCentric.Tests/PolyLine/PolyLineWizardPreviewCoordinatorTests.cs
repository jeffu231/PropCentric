using System.Collections.ObjectModel;
using System.Numerics;
using PropCentric.Tests.Common;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup.Drafts;
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
    public async Task BuildPreviewAsync_UnchangedInput_ReusesCachedModel()
    {
        var builder = new CountingBuilder();
        var coordinator = new PolyLineWizardPreviewCoordinator(new PolyLineDraftToVisualInputMapper(), builder);
        var draft = CreateDraft();

        var first = await coordinator.BuildPreviewAsync(draft);
        var second = await coordinator.BuildPreviewAsync(draft);

        Assert.Same(first, second);
        Assert.Equal(1, builder.CallCount);
    }

    [Fact]
    public async Task BuildPreviewAsync_ChangedSegmentPointCount_RebuildsModel()
    {
        var builder = new CountingBuilder();
        var coordinator = new PolyLineWizardPreviewCoordinator(new PolyLineDraftToVisualInputMapper(), builder);
        var draft = CreateDraft();

        var first = await coordinator.BuildPreviewAsync(draft);
        draft.Segments[0].PointCount += 1;
        var second = await coordinator.BuildPreviewAsync(draft);

        Assert.NotSame(first, second);
        Assert.Equal(2, builder.CallCount);
    }

    private static PolyLinePropDraft CreateDraft()
    {
        return new PolyLinePropDraft
        {
            Name = "Preview PolyLine",
            LightSize = 2,
            Segments = new ObservableCollection<SegmentDraftState>(
                PolyLineTestData.CreateSegments().Select(segment => new SegmentDraftState
                {
                    Start = segment.Start,
                    End = segment.End,
                    PointCount = segment.PointCount
                }))
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
