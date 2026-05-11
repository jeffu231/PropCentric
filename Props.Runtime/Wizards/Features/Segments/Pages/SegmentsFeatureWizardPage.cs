using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Data;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Runtime.Wizards.Features.Segments.Mappers;

namespace Props.Runtime.Wizards.Features.Segments.Pages;

/// <summary>
/// Wizard page for reviewing captured segments and editing per-segment point counts.
/// </summary>
[FeatureWizardPage(typeof(IHasSegments), mapperType: typeof(SegmentsFeatureWizardDataMapper), priority: 150)]
public sealed class SegmentsFeatureWizardPage : WizardPageBase
{
    public SegmentsFeatureWizardPage()
    {
        Title = "Segments";
        Description = "Review captured segments and adjust the point count for each segment.";
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
/// Page-owned segment state used by the segments feature wizard.
/// </summary>
public sealed class SegmentFeatureWizardItem : ModelBase
{
    public string StartDisplay
    {
        get => GetValue<string>(StartDisplayProperty);
        set => SetValue(StartDisplayProperty, value);
    }

    private static readonly IPropertyData StartDisplayProperty = RegisterProperty<string>(nameof(StartDisplay), string.Empty);

    public string EndDisplay
    {
        get => GetValue<string>(EndDisplayProperty);
        set => SetValue(EndDisplayProperty, value);
    }

    private static readonly IPropertyData EndDisplayProperty = RegisterProperty<string>(nameof(EndDisplay), string.Empty);

    public int PointCount
    {
        get => GetValue<int>(PointCountProperty);
        set => SetValue(PointCountProperty, value);
    }

    private static readonly IPropertyData PointCountProperty = RegisterProperty<int>(nameof(PointCount));
}
