using System.Collections.ObjectModel;
using Props.Abstractions.Features;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.PolyLine.Visuals;
using Props.Runtime.Wizards.Features.Segments.Pages;

namespace PropCentric.Tests.PolyLine;

/// <summary>
/// Verifies the draft-backed segments feature page behavior.
/// </summary>
public class SegmentsFeatureWizardPageTests
{
    [Fact]
    public void Initialize_BindsToSharedDraftSegments()
    {
        var draft = CreateDraft();
        var previewSession = new TestPreviewSession(draft);
        var page = new SegmentsFeatureWizardPage();

        page.Initialize(draft, previewSession);

        Assert.Same(previewSession, page.PreviewSession);
        Assert.Equal(draft.Segments.Count, page.Segments.Count);
        Assert.Equal("(20, 20)", page.Segments[0].StartDisplay);
        Assert.Equal("(30, 30)", page.Segments[0].EndDisplay);
        Assert.Equal(draft.Segments.Sum(segment => segment.PointCount), page.TotalPoints);
    }

    [Fact]
    public void PointCountEdit_UpdatesSharedDraftImmediately()
    {
        var draft = CreateDraft();
        var page = new SegmentsFeatureWizardPage();

        page.Initialize(draft, new TestPreviewSession(draft));
        page.Segments[0].PointCount = 12;
        page.Segments[1].PointCount = 34;

        Assert.Equal(12, draft.Segments[0].PointCount);
        Assert.Equal(34, draft.Segments[1].PointCount);
        Assert.Equal(46, page.TotalPoints);
    }

    private static PolyLinePropDraft CreateDraft()
    {
        return new PolyLinePropDraft
        {
            Segments = new ObservableCollection<SegmentDraftState>(
                PolyLineTestData.CreateSegments().Select(segment => new SegmentDraftState
                {
                    Start = segment.Start,
                    End = segment.End,
                    PointCount = segment.PointCount
                }))
        };
    }

    private sealed class TestPreviewSession(PolyLinePropDraft draft) : IWizardPreviewSession<PolyLinePropDraft>
    {
        public PolyLinePropDraft Draft => draft;

        Props.Abstractions.Setup.IPropDraft IWizardPreviewSession.Draft => Draft;

        public IPropVisualModel BuildPreview() => new PolyLinePropVisualModel { Elements = [] };
    }
}
