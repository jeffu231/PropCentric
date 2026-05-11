using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Props.Runtime.Wizards.Core.ViewModels;
using Props.Runtime.Wizards.Features.Segments.Pages;

namespace Props.Runtime.Wizards.Features.Segments.ViewModels;

/// <summary>
/// View model for the <see cref="SegmentsFeatureWizardPage"/>.
/// </summary>
public sealed class SegmentsFeatureWizardPageViewModel : GraphicsWizardPageViewModelBase<SegmentsFeatureWizardPage>
{
    public SegmentsFeatureWizardPageViewModel(SegmentsFeatureWizardPage featureWizardPage) : base(featureWizardPage)
    {
        PreviewBuilder = () => featureWizardPage.PreviewSession?.BuildPreview()
            ?? throw new InvalidOperationException("Segments preview session has not been initialized.");

        HookSegmentHandlers(featureWizardPage.Segments);
    }

    [ViewModelToModel]
    public ObservableCollection<SegmentFeatureWizardItem> Segments
    {
        get => GetValue<ObservableCollection<SegmentFeatureWizardItem>>(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    private static readonly IPropertyData SegmentsProperty =
        RegisterProperty<ObservableCollection<SegmentFeatureWizardItem>>(nameof(Segments), []);

    [ViewModelToModel]
    public int TotalPoints
    {
        get => GetValue<int>(TotalPointsProperty);
        set => SetValue(TotalPointsProperty, value);
    }

    private static readonly IPropertyData TotalPointsProperty = RegisterProperty<int>(nameof(TotalPoints));

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);

        if (Segments.Count == 0)
        {
            validationResults.Add(FieldValidationResult.CreateError(nameof(Segments), "At least one segment is required."));
        }

        for (var index = 0; index < Segments.Count; index++)
        {
            if (Segments[index].PointCount <= 0)
            {
                validationResults.Add(FieldValidationResult.CreateError(
                    nameof(Segments),
                    $"Segment {index + 1} point count must be greater than zero."));
            }
        }
    }

    private void HookSegmentHandlers(ObservableCollection<SegmentFeatureWizardItem> segments)
    {
        segments.CollectionChanged -= OnSegmentsCollectionChanged;
        segments.CollectionChanged += OnSegmentsCollectionChanged;

        foreach (var segment in segments)
        {
            segment.PropertyChanged -= OnSegmentPropertyChanged;
            segment.PropertyChanged += OnSegmentPropertyChanged;
        }
    }

    private void OnSegmentsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (SegmentFeatureWizardItem oldItem in e.OldItems)
            {
                oldItem.PropertyChanged -= OnSegmentPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (SegmentFeatureWizardItem newItem in e.NewItems)
            {
                newItem.PropertyChanged -= OnSegmentPropertyChanged;
                newItem.PropertyChanged += OnSegmentPropertyChanged;
            }
        }

        SchedulePreviewRebuild();
    }

    private void OnSegmentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SegmentFeatureWizardItem.PointCount))
        {
            SchedulePreviewRebuild();
        }
    }
}
