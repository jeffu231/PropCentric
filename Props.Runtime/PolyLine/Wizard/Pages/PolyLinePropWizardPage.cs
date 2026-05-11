using System.ComponentModel;
using Orc.Wizard;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.Wizards.Core.Pages;

namespace Props.Runtime.PolyLine.Wizard.Pages;

/// <summary>
/// Minimal wizard page for polyline prop configuration.
/// </summary>
public sealed class PolyLinePropWizardPage : LightPropWizardPage
{
    private readonly PolyLinePropDraft _draft;

    public PolyLinePropWizardPage(
        PolyLinePropDraft draft,
        IWizardPreviewCoordinator<PolyLinePropDraft> coordinator)
    {
        _draft = draft;
        Coordinator = coordinator;

        Title = "Basic Attributes";
        Description = "Configure the PolyLine prop name, display settings, and live preview.";

        PropertyChanged += OnParentPropertyChanged;
    }

    public IWizardPreviewCoordinator<PolyLinePropDraft> Coordinator { get; }

    public PolyLinePropDraft Draft => _draft;

    public int SegmentCount => _draft.Segments.Count;

    public int TotalPoints => _draft.Segments.Sum(segment => segment.PointCount);

    public string SegmentSummary =>
        SegmentCount == 0
            ? "No captured segments."
            : string.Join(Environment.NewLine,
                _draft.Segments.Select((segment, index) =>
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

    private void OnParentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Name):
                _draft.Name = Name;
                break;
            case nameof(LightSize):
                _draft.LightSize = LightSize;
                break;
        }

        if (e.PropertyName is nameof(Name) or nameof(LightSize))
        {
            RaisePropertyChanged(nameof(SegmentCount));
            RaisePropertyChanged(nameof(TotalPoints));
            RaisePropertyChanged(nameof(SegmentSummary));
        }
    }
}
