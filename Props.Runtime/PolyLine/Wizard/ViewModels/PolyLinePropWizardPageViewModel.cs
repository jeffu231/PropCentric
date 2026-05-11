using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using Props.Runtime.PolyLine.Visuals;
using Props.Runtime.PolyLine.Wizard.Pages;
using Props.Runtime.ViewModels;
using Props.Runtime.Wizards.Core.ViewModels;

namespace Props.Runtime.PolyLine.Wizard.ViewModels;

/// <summary>
/// View model for the polyline prop wizard page.
/// </summary>
public sealed class PolyLinePropWizardPageViewModel
    : LightWizardPageViewModel<PolyLinePropWizardPage, PolyLinePropVisualModel>, IPropWizardPageViewModel
{
    public PolyLinePropWizardPageViewModel(PolyLinePropWizardPage wizardPage) : base(wizardPage)
    {
        Rotations = AxisRotationViewModel.ConvertToViewModel(
            new ObservableCollection<Props.Abstractions.PropVisualModels.AxisRotationModel>(wizardPage.Draft.AxisRotations));
        PreviewBuilder = () =>
        {
            wizardPage.Draft.AxisRotations = AxisRotationViewModel.ConvertToModel(Rotations);
            return wizardPage.Coordinator.BuildPreview(wizardPage.Draft);
        };
    }
    
    [ViewModelToModel]
    public int SegmentCount
    {
        get => GetValue<int>(SegmentCountProperty);
        set => SetValue(SegmentCountProperty, value);
    }

    public static readonly IPropertyData SegmentCountProperty = RegisterProperty<int>(nameof(SegmentCount));

    [ViewModelToModel]
    public int TotalPoints
    {
        get => GetValue<int>(TotalPointsProperty);
        set => SetValue(TotalPointsProperty, value);
    }

    public static readonly IPropertyData TotalPointsProperty = RegisterProperty<int>(nameof(TotalPoints));

    [ViewModelToModel]
    public string SegmentSummary
    {
        get => GetValue<string>(SegmentSummaryProperty);
        set => SetValue(SegmentSummaryProperty, value);
    }

    public static readonly IPropertyData SegmentSummaryProperty = RegisterProperty<string>(nameof(SegmentSummary), string.Empty);

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);

        if (SegmentCount <= 0)
        {
            validationResults.Add(FieldValidationResult.CreateError(nameof(SegmentCount),
                "At least one segment is required."));
        }

        if (TotalPoints <= 0)
        {
            validationResults.Add(FieldValidationResult.CreateError(nameof(TotalPoints),
                "At least one light point is required."));
        }
    }
}
