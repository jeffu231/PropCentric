using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Props.Abstractions.Features;
using Props.Runtime.Wizards.Core.ViewModels;
using Props.Runtime.Wizards.Features.Color.Pages;

namespace Props.Runtime.Wizards.Features.Color.ViewModels;

/// <summary>
/// View model for the <see cref="ColorFeatureWizardPage"/>.
/// </summary>
public sealed class ColorFeatureWizardPageViewModel : GraphicsWizardPageViewModelBase<ColorFeatureWizardPage>
{
    public ColorFeatureWizardPageViewModel(ColorFeatureWizardPage featureWizardPage) : base(featureWizardPage)
    {
        Page = featureWizardPage;
        PreviewBuilder = () => featureWizardPage.PreviewSession?.BuildPreview()
            ?? throw new InvalidOperationException("Color preview session has not been initialized.");

        HookWorkingColorHandlers(featureWizardPage.WorkingDiscreteColors);
        featureWizardPage.PropertyChanged += OnPagePropertyChanged;
    }

    public ColorFeatureWizardPage Page { get; }

    [ViewModelToModel]
    public LightType LightType
    {
        get => GetValue<LightType>(LightTypeProperty);
        set => SetValue(LightTypeProperty, value);
    }

    private static readonly IPropertyData LightTypeProperty = RegisterProperty<LightType>(nameof(LightType));

    [ViewModelToModel]
    public ObservableCollection<EditableDiscreteColorItem> WorkingDiscreteColors
    {
        get => GetValue<ObservableCollection<EditableDiscreteColorItem>>(WorkingDiscreteColorsProperty);
        set => SetValue(WorkingDiscreteColorsProperty, value);
    }

    private static readonly IPropertyData WorkingDiscreteColorsProperty =
        RegisterProperty<ObservableCollection<EditableDiscreteColorItem>>(nameof(WorkingDiscreteColors), []);

    [ViewModelToModel]
    public DiscreteColorSetDefinition? SelectedDiscreteColorSet
    {
        get => GetValue<DiscreteColorSetDefinition?>(SelectedDiscreteColorSetProperty);
        set => SetValue(SelectedDiscreteColorSetProperty, value);
    }

    private static readonly IPropertyData SelectedDiscreteColorSetProperty =
        RegisterProperty<DiscreteColorSetDefinition?>(nameof(SelectedDiscreteColorSet));

    [ViewModelToModel]
    public FullColorOrderDefinition? SelectedFullColorOrder
    {
        get => GetValue<FullColorOrderDefinition?>(SelectedFullColorOrderProperty);
        set => SetValue(SelectedFullColorOrderProperty, value);
    }

    private static readonly IPropertyData SelectedFullColorOrderProperty =
        RegisterProperty<FullColorOrderDefinition?>(nameof(SelectedFullColorOrder));

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);

        if (LightType == LightType.MultipleDiscreteColors)
        {
            if (WorkingDiscreteColors.Count == 0)
            {
                validationResults.Add(FieldValidationResult.CreateError(
                    nameof(WorkingDiscreteColors),
                    "At least one discrete color is required."));
            }
        }

        if (LightType == LightType.FullColor && SelectedFullColorOrder is null)
        {
            validationResults.Add(FieldValidationResult.CreateError(
                nameof(SelectedFullColorOrder),
                "A full color order must be selected."));
        }
    }

    private void HookWorkingColorHandlers(ObservableCollection<EditableDiscreteColorItem> items)
    {
        items.CollectionChanged -= OnWorkingDiscreteColorsCollectionChanged;
        items.CollectionChanged += OnWorkingDiscreteColorsCollectionChanged;

        foreach (var item in items)
        {
            item.PropertyChanged -= OnWorkingDiscreteColorPropertyChanged;
            item.PropertyChanged += OnWorkingDiscreteColorPropertyChanged;
        }
    }

    private void OnWorkingDiscreteColorsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (EditableDiscreteColorItem oldItem in e.OldItems)
            {
                oldItem.PropertyChanged -= OnWorkingDiscreteColorPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (EditableDiscreteColorItem newItem in e.NewItems)
            {
                newItem.PropertyChanged -= OnWorkingDiscreteColorPropertyChanged;
                newItem.PropertyChanged += OnWorkingDiscreteColorPropertyChanged;
            }
        }

        SchedulePreviewRebuild();
    }

    private void OnWorkingDiscreteColorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(EditableDiscreteColorItem.Color) or nameof(EditableDiscreteColorItem.Hex))
        {
            SchedulePreviewRebuild();
        }
    }

    private void OnPagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ColorFeatureWizardPage.LightType)
            or nameof(ColorFeatureWizardPage.SelectedDiscreteColorSet)
            or nameof(ColorFeatureWizardPage.SelectedFullColorOrder)
            or nameof(ColorFeatureWizardPage.SingleColor))
        {
            SchedulePreviewRebuild();
        }
    }
}
