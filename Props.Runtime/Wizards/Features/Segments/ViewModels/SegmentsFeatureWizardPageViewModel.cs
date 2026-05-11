using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Props.Runtime.Wizards.Features.Segments.Pages;

namespace Props.Runtime.Wizards.Features.Segments.ViewModels;

/// <summary>
/// View model for the <see cref="SegmentsFeatureWizardPage"/>.
/// </summary>
public sealed class SegmentsFeatureWizardPageViewModel : WizardPageViewModelBase<SegmentsFeatureWizardPage>
{
    public SegmentsFeatureWizardPageViewModel(SegmentsFeatureWizardPage featureWizardPage) : base(featureWizardPage)
    {
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
}
