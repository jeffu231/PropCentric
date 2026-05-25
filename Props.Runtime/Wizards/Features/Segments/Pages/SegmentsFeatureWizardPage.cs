using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.Setup;
using Props.Abstractions.Setup.Drafts;
using Props.Abstractions.Visuals;

namespace Props.Runtime.Wizards.Features.Segments.Pages;

/// <summary>
/// Wizard page for reviewing captured segments and editing per-segment point counts.
/// </summary>
[FeatureWizardPage(typeof(IHasSegments), priority: 150)]
public sealed class SegmentsFeatureWizardPage : WizardPageBase, IFeatureWizardDraftPage
{
    public SegmentsFeatureWizardPage()
    {
        Title = "Segments";
        Description = "Review captured segments, adjust the point count for each segment, and preview the result.";
        Segments = [];
    }

    public ObservableCollection<SegmentFeatureWizardItem> Segments
    {
        get => GetValue<ObservableCollection<SegmentFeatureWizardItem>>(SegmentsProperty);
        set
        {
            var current = Segments;
            if (ReferenceEquals(current, value))
            {
                return;
            }

            if (current is not null)
            {
                current.CollectionChanged -= OnSegmentsCollectionChanged;
                foreach (var segment in current)
                {
                    segment.PropertyChanged -= OnSegmentItemPropertyChanged;
                }
            }

            SetValue(SegmentsProperty, value);

            value.CollectionChanged += OnSegmentsCollectionChanged;
            foreach (var segment in value)
            {
                segment.PropertyChanged += OnSegmentItemPropertyChanged;
            }

            RefreshTotals();
        }
    }

    private static readonly IPropertyData SegmentsProperty =
        RegisterProperty<ObservableCollection<SegmentFeatureWizardItem>>(nameof(Segments), []);

    public int TotalPoints
    {
        get => GetValue<int>(TotalPointsProperty);
        private set => SetValue(TotalPointsProperty, value);
    }

    private static readonly IPropertyData TotalPointsProperty = RegisterProperty<int>(nameof(TotalPoints));

    public IWizardPreviewSession? PreviewSession { get; private set; }

    public void Initialize(FeatureWizardContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Draft is not IHasSegmentsDraft segmentsDraft)
        {
            throw new InvalidOperationException($"Draft {context.Draft.GetType()} does not implement {nameof(IHasSegmentsDraft)}.");
        }

        PreviewSession = context.PreviewSession;
        Segments = new ObservableCollection<SegmentFeatureWizardItem>(
            segmentsDraft.Segments.Select(segment => new SegmentFeatureWizardItem(segment)));
        RefreshTotals();
    }

    public override ISummaryItem GetSummary()
    {
        return new SummaryItem
        {
            Title = Title,
            Summary = $"Segments: {Segments.Count}\nTotal Points: {TotalPoints}"
        };
    }

    internal void RefreshTotals()
    {
        TotalPoints = Segments.Sum(segment => segment.PointCount);
    }

    private void OnSegmentsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (SegmentFeatureWizardItem segment in e.OldItems)
            {
                segment.PropertyChanged -= OnSegmentItemPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (SegmentFeatureWizardItem segment in e.NewItems)
            {
                segment.PropertyChanged += OnSegmentItemPropertyChanged;
            }
        }

        RefreshTotals();
    }

    private void OnSegmentItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SegmentFeatureWizardItem.PointCount))
        {
            RefreshTotals();
        }
    }
}

/// <summary>
/// Page-facing wrapper around shared draft segment state.
/// </summary>
public sealed class SegmentFeatureWizardItem : ModelBase
{
    private readonly SegmentDraftState _segment;

    public SegmentFeatureWizardItem(SegmentDraftState segment)
    {
        _segment = segment;
        StartDisplay = Format(segment.Start);
        EndDisplay = Format(segment.End);
        PointCount = segment.PointCount;
    }

    public string StartDisplay
    {
        get => GetValue<string>(StartDisplayProperty);
        private set => SetValue(StartDisplayProperty, value);
    }

    private static readonly IPropertyData StartDisplayProperty = RegisterProperty<string>(nameof(StartDisplay), string.Empty);

    public string EndDisplay
    {
        get => GetValue<string>(EndDisplayProperty);
        private set => SetValue(EndDisplayProperty, value);
    }

    private static readonly IPropertyData EndDisplayProperty = RegisterProperty<string>(nameof(EndDisplay), string.Empty);

    public int PointCount
    {
        get => GetValue<int>(PointCountProperty);
        set
        {
            SetValue(PointCountProperty, value);
            _segment.PointCount = value;
        }
    }

    private static readonly IPropertyData PointCountProperty = RegisterProperty<int>(nameof(PointCount));

    private static string Format(System.Numerics.Vector2 point) => $"({point.X:0.###}, {point.Y:0.###})";
}
