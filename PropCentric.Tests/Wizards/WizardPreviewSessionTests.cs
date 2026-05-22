using System.Collections.ObjectModel;
using System.Numerics;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup.Drafts;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.PolyLine.Visuals;
using Props.Runtime.Wizards.Core.Preview;

namespace PropCentric.Tests.Wizards;

/// <summary>
/// Verifies shared preview-session behavior for wizard flows.
/// </summary>
public class WizardPreviewSessionTests
{
    [Fact]
    public void BuildPreview_DelegatesToCoordinatorUsingSharedDraft()
    {
        var draft = new PolyLinePropDraft
        {
            Name = "Preview Session",
            LightSize = 3,
            Segments =
            [
                new SegmentDraftState
                {
                    Start = new Vector2(0f, 0f),
                    End = new Vector2(1f, 0f),
                    PointCount = 5
                }
            ]
        };

        var coordinator = new TrackingCoordinator();
        var session = new WizardPreviewSession<PolyLinePropDraft>(draft, coordinator);

        var model = session.BuildPreview();

        Assert.Same(draft, session.Draft);
        Assert.Same(draft, coordinator.LastDraft);
        Assert.Same(model, coordinator.ModelToReturn);
    }

    private sealed class TrackingCoordinator : IWizardPreviewCoordinator<PolyLinePropDraft>
    {
        public TrackingCoordinator()
        {
            ModelToReturn = new PolyLinePropVisualModel { Elements = [] };
        }

        public PolyLinePropDraft? LastDraft { get; private set; }

        public IPropVisualModel ModelToReturn { get; }

        public IPropVisualModel BuildPreview(PolyLinePropDraft draft)
        {
            LastDraft = draft;
            return ModelToReturn;
        }
    }
}
