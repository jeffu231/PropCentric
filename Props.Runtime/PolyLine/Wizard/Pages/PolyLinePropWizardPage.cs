using Orc.Wizard;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.Wizards.Core.Pages;

namespace Props.Runtime.PolyLine.Wizard.Pages;

/// <summary>
/// Minimal wizard page for polyline prop configuration.
/// </summary>
public sealed class PolyLinePropWizardPage : LightPropWizardPage<PolyLinePropDraft>
{
    public PolyLinePropWizardPage(
        PolyLinePropDraft draft,
        IWizardPreviewCoordinator<PolyLinePropDraft> coordinator)
        : base(draft)
    {
        Coordinator = coordinator;

        Title = "Basic Attributes";
        Description = "Configure the PolyLine prop name, display settings, and live preview.";
    }

    public IWizardPreviewCoordinator<PolyLinePropDraft> Coordinator { get; }

    public int SegmentCount => Draft.Segments.Count;

    public int TotalPoints => Draft.Segments.Sum(segment => segment.PointCount);

    public string SegmentSummary =>
        SegmentCount == 0
            ? "No captured segments."
            : string.Join(Environment.NewLine,
                Draft.Segments.Select((segment, index) =>
                    $"Segment {index + 1}: ({segment.Start.X:0.###}, {segment.Start.Y:0.###}) -> " +
                    $"({segment.End.X:0.###}, {segment.End.Y:0.###}), points: {segment.PointCount}"));

    public override ISummaryItem GetSummary()
    {
        return new SummaryItem
        {
            Title = Title,
            Summary = $"Prop Type: PolyLine\n" +
                      $"Name: {Name}\n" +
                      $"Light Size: {LightSize}\n" +
                      $"Segments: {SegmentCount}\n" +
                      $"Total Points: {TotalPoints}\n"
        };
    }
}
