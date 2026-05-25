using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Props.Runtime.Wizards.Core.ViewModels;
using Props.Runtime.Wizards.Features.Rotation.Pages;

namespace Props.Runtime.Wizards.Features.Rotation.ViewModels;

/// <summary>
/// View model for the <see cref="RotationFeatureWizardPage"/>.
/// </summary>
public sealed class RotationFeatureWizardPageViewModel : GraphicsWizardPageViewModelBase<RotationFeatureWizardPage>
{
    public RotationFeatureWizardPageViewModel(RotationFeatureWizardPage featureWizardPage) : base(featureWizardPage)
    {
        PreviewBuilder = cancellationToken => featureWizardPage.PreviewSession?.BuildPreviewAsync(cancellationToken)
            ?? throw new InvalidOperationException("Rotation preview session has not been initialized.");

        HookRotationHandlers(featureWizardPage.Rotations);
    }

    [ViewModelToModel]
    public ObservableCollection<RotationFeatureWizardItem> Rotations
    {
        get => GetValue<ObservableCollection<RotationFeatureWizardItem>>(RotationsProperty);
        set => SetValue(RotationsProperty, value);
    }

    private static readonly IPropertyData RotationsProperty =
        RegisterProperty<ObservableCollection<RotationFeatureWizardItem>>(nameof(Rotations), []);

    [ViewModelToModel]
    public string RotationSummary
    {
        get => GetValue<string>(RotationSummaryProperty);
        set => SetValue(RotationSummaryProperty, value);
    }

    private static readonly IPropertyData RotationSummaryProperty =
        RegisterProperty<string>(nameof(RotationSummary), string.Empty);

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);

        if (Rotations.Count == 0)
        {
            validationResults.Add(FieldValidationResult.CreateError(nameof(Rotations), "At least one rotation is required."));
        }
    }

    private void HookRotationHandlers(ObservableCollection<RotationFeatureWizardItem> rotations)
    {
        rotations.CollectionChanged -= OnRotationsCollectionChanged;
        rotations.CollectionChanged += OnRotationsCollectionChanged;

        foreach (var rotation in rotations)
        {
            rotation.PropertyChanged -= OnRotationPropertyChanged;
            rotation.PropertyChanged += OnRotationPropertyChanged;
        }
    }

    private void OnRotationsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (RotationFeatureWizardItem oldItem in e.OldItems)
            {
                oldItem.PropertyChanged -= OnRotationPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (RotationFeatureWizardItem newItem in e.NewItems)
            {
                newItem.PropertyChanged -= OnRotationPropertyChanged;
                newItem.PropertyChanged += OnRotationPropertyChanged;
            }
        }

        SchedulePreviewRebuild();
    }

    private void OnRotationPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(RotationFeatureWizardItem.Axis) or nameof(RotationFeatureWizardItem.RotationAngle))
        {
            SchedulePreviewRebuild();
        }
    }
}
