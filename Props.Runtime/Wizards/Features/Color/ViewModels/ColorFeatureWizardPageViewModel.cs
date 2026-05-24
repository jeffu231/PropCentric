using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Data;
using DrawingColor = System.Drawing.Color;
using Props.Abstractions.Features;
using Props.Runtime.Wizards.Core.ViewModels;
using Props.Runtime.Wizards.Features.Color.Pages;

namespace Props.Runtime.Wizards.Features.Color.ViewModels;

/// <summary>
/// View model for the <see cref="ColorFeatureWizardPage"/>.
/// </summary>
public sealed class ColorFeatureWizardPageViewModel : GraphicsWizardPageViewModelBase<ColorFeatureWizardPage>
{
    private ObservableCollection<EditableDiscreteColorItem> _trackedWorkingDiscreteColors;

    public ColorFeatureWizardPageViewModel(ColorFeatureWizardPage featureWizardPage) : base(featureWizardPage)
    {
        PreviewBuilder = () => featureWizardPage.PreviewSession?.BuildPreview()
            ?? throw new InvalidOperationException("Color preview session has not been initialized.");

        _trackedWorkingDiscreteColors = featureWizardPage.WorkingDiscreteColors;
        HookWorkingColorHandlers(_trackedWorkingDiscreteColors);
        featureWizardPage.PropertyChanged += OnPagePropertyChanged;
    }

    public ColorFeatureWizardPage Page => WizardPage;

    public LightType LightType
    {
        get => Page.LightType;
        set
        {
            if (Page.LightType == value)
            {
                return;
            }

            Page.LightType = value;
            RaisePropertyChanged(nameof(LightType));
            RaisePropertyChanged(nameof(IsSingleColorMode));
            RaisePropertyChanged(nameof(IsMultipleDiscreteColorsMode));
            RaisePropertyChanged(nameof(IsFullColorMode));
        }
    }

    public bool IsSingleColorMode => Page.IsSingleColorMode;

    public bool IsMultipleDiscreteColorsMode => Page.IsMultipleDiscreteColorsMode;

    public bool IsFullColorMode => Page.IsFullColorMode;

    public DrawingColor SingleColor => Page.SingleColor;

    public string SingleColorHex => Page.SingleColorHex;

    public ObservableCollection<DiscreteColorSetDefinition> AvailableDiscreteColorSets => Page.AvailableDiscreteColorSets;

    public DiscreteColorSetDefinition? SelectedDiscreteColorSet
    {
        get => Page.SelectedDiscreteColorSet;
        set
        {
            if (ReferenceEquals(Page.SelectedDiscreteColorSet, value))
            {
                return;
            }

            Page.SelectedDiscreteColorSet = value;
            RaisePropertyChanged(nameof(SelectedDiscreteColorSet));
        }
    }

    public ObservableCollection<EditableDiscreteColorItem> WorkingDiscreteColors => Page.WorkingDiscreteColors;

    public EditableDiscreteColorItem? SelectedWorkingDiscreteColor
    {
        get => Page.SelectedWorkingDiscreteColor;
        set
        {
            if (ReferenceEquals(Page.SelectedWorkingDiscreteColor, value))
            {
                return;
            }

            Page.SelectedWorkingDiscreteColor = value;
            RaisePropertyChanged(nameof(SelectedWorkingDiscreteColor));
            RaisePropertyChanged(nameof(CanRemoveWorkingDiscreteColor));
        }
    }

    public bool CanRemoveWorkingDiscreteColor => Page.CanRemoveWorkingDiscreteColor;

    public string NewDiscreteColorSetName
    {
        get => Page.NewDiscreteColorSetName;
        set
        {
            if (string.Equals(Page.NewDiscreteColorSetName, value, StringComparison.Ordinal))
            {
                return;
            }

            Page.NewDiscreteColorSetName = value;
            RaisePropertyChanged(nameof(NewDiscreteColorSetName));
        }
    }

    public ObservableCollection<FullColorOrderDefinition> AvailableFullColorOrders => Page.AvailableFullColorOrders;

    public FullColorOrderDefinition? SelectedFullColorOrder
    {
        get => Page.SelectedFullColorOrder;
        set
        {
            if (ReferenceEquals(Page.SelectedFullColorOrder, value))
            {
                return;
            }

            Page.SelectedFullColorOrder = value;
            RaisePropertyChanged(nameof(SelectedFullColorOrder));
        }
    }

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);

        if (LightType == LightType.MultipleDiscreteColors && WorkingDiscreteColors.Count == 0)
        {
            validationResults.Add(FieldValidationResult.CreateError(
                nameof(WorkingDiscreteColors),
                "At least one discrete color is required."));
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

    private void UnhookWorkingColorHandlers(ObservableCollection<EditableDiscreteColorItem> items)
    {
        items.CollectionChanged -= OnWorkingDiscreteColorsCollectionChanged;

        foreach (var item in items)
        {
            item.PropertyChanged -= OnWorkingDiscreteColorPropertyChanged;
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

        RaisePropertyChanged(nameof(WorkingDiscreteColors));
        RaisePropertyChanged(nameof(CanRemoveWorkingDiscreteColor));
        SchedulePreviewRebuild();
    }

    private void OnWorkingDiscreteColorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(EditableDiscreteColorItem.Color) or nameof(EditableDiscreteColorItem.Hex))
        {
            RaisePropertyChanged(nameof(WorkingDiscreteColors));
            SchedulePreviewRebuild();
        }
    }

    private void OnPagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ColorFeatureWizardPage.LightType):
                RaisePropertyChanged(nameof(LightType));
                RaisePropertyChanged(nameof(IsSingleColorMode));
                RaisePropertyChanged(nameof(IsMultipleDiscreteColorsMode));
                RaisePropertyChanged(nameof(IsFullColorMode));
                SchedulePreviewRebuild();
                break;
            case nameof(ColorFeatureWizardPage.SingleColor):
                RaisePropertyChanged(nameof(SingleColor));
                RaisePropertyChanged(nameof(SingleColorHex));
                SchedulePreviewRebuild();
                break;
            case nameof(ColorFeatureWizardPage.SelectedDiscreteColorSet):
                RaisePropertyChanged(nameof(SelectedDiscreteColorSet));
                SchedulePreviewRebuild();
                break;
            case nameof(ColorFeatureWizardPage.SelectedFullColorOrder):
                RaisePropertyChanged(nameof(SelectedFullColorOrder));
                SchedulePreviewRebuild();
                break;
            case nameof(ColorFeatureWizardPage.SelectedWorkingDiscreteColor):
                RaisePropertyChanged(nameof(SelectedWorkingDiscreteColor));
                RaisePropertyChanged(nameof(CanRemoveWorkingDiscreteColor));
                break;
            case nameof(ColorFeatureWizardPage.NewDiscreteColorSetName):
                RaisePropertyChanged(nameof(NewDiscreteColorSetName));
                break;
            case nameof(ColorFeatureWizardPage.AvailableDiscreteColorSets):
                RaisePropertyChanged(nameof(AvailableDiscreteColorSets));
                break;
            case nameof(ColorFeatureWizardPage.AvailableFullColorOrders):
                RaisePropertyChanged(nameof(AvailableFullColorOrders));
                break;
            case nameof(ColorFeatureWizardPage.WorkingDiscreteColors):
                UnhookWorkingColorHandlers(_trackedWorkingDiscreteColors);
                _trackedWorkingDiscreteColors = Page.WorkingDiscreteColors;
                HookWorkingColorHandlers(_trackedWorkingDiscreteColors);
                RaisePropertyChanged(nameof(WorkingDiscreteColors));
                RaisePropertyChanged(nameof(CanRemoveWorkingDiscreteColor));
                break;
        }
    }
}
