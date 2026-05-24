using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Services;
using Catel.Data;
using Catel.MVVM;
using DrawingColor = System.Drawing.Color;
using Props.Abstractions.Features;
using Props.Runtime.Wizards.Core.ViewModels;
using Props.Runtime.Wizards.Features.Color.Pages;
using Props.Runtime.Wizards.Features.Color.Services;

namespace Props.Runtime.Wizards.Features.Color.ViewModels;

/// <summary>
/// View model for the <see cref="ColorFeatureWizardPage"/>.
/// </summary>
public sealed class ColorFeatureWizardPageViewModel : GraphicsWizardPageViewModelBase<ColorFeatureWizardPage>
{
    private readonly IColorFeatureWizardInteractionService _interactionService;
    private readonly IMessageService _messageService;
    private ObservableCollection<EditableDiscreteColorItem> _trackedWorkingDiscreteColors;

    public ColorFeatureWizardPageViewModel(ColorFeatureWizardPage featureWizardPage)
        : this(featureWizardPage, new ColorFeatureWizardInteractionService(), new NoOpMessageService())
    {
    }

    public ColorFeatureWizardPageViewModel(
        ColorFeatureWizardPage featureWizardPage,
        IColorFeatureWizardInteractionService interactionService,
        IMessageService messageService)
        : base(featureWizardPage)
    {
        _interactionService = interactionService;
        _messageService = messageService;
        PreviewBuilder = () => featureWizardPage.PreviewSession?.BuildPreview()
            ?? throw new InvalidOperationException("Color preview session has not been initialized.");

        _trackedWorkingDiscreteColors = featureWizardPage.WorkingDiscreteColors;
        HookWorkingColorHandlers(_trackedWorkingDiscreteColors);
        featureWizardPage.PropertyChanged += OnPagePropertyChanged;

        PickSingleColorCommand = new Command(PickSingleColor);
        EditWorkingDiscreteColorCommand = new Command<EditableDiscreteColorItem?>(EditWorkingDiscreteColor);
        AddWorkingDiscreteColorCommand = new Command(AddWorkingDiscreteColor);
        RemoveWorkingDiscreteColorCommand = new Command(RemoveWorkingDiscreteColor);
        SaveCustomSetCommand = new TaskCommand(SaveCustomSetAsync);
    }

    public Command PickSingleColorCommand { get; }

    public Command<EditableDiscreteColorItem?> EditWorkingDiscreteColorCommand { get; }

    public Command AddWorkingDiscreteColorCommand { get; }

    public Command RemoveWorkingDiscreteColorCommand { get; }

    public TaskCommand SaveCustomSetCommand { get; }

    public LightType LightType
    {
        get => WizardPage.LightType;
        set
        {
            if (WizardPage.LightType == value)
            {
                return;
            }

            WizardPage.LightType = value;
            RaisePropertyChanged(nameof(LightType));
            RaisePropertyChanged(nameof(IsSingleColorMode));
            RaisePropertyChanged(nameof(IsMultipleDiscreteColorsMode));
            RaisePropertyChanged(nameof(IsFullColorMode));
        }
    }

    public bool IsSingleColorMode => WizardPage.IsSingleColorMode;

    public bool IsMultipleDiscreteColorsMode => WizardPage.IsMultipleDiscreteColorsMode;

    public bool IsFullColorMode => WizardPage.IsFullColorMode;

    public DrawingColor SingleColor => WizardPage.SingleColor;

    public string SingleColorHex => WizardPage.SingleColorHex;

    public ObservableCollection<DiscreteColorSetDefinition> AvailableDiscreteColorSets => WizardPage.AvailableDiscreteColorSets;

    public DiscreteColorSetDefinition? SelectedDiscreteColorSet
    {
        get => WizardPage.SelectedDiscreteColorSet;
        set
        {
            if (ReferenceEquals(WizardPage.SelectedDiscreteColorSet, value))
            {
                return;
            }

            WizardPage.SelectedDiscreteColorSet = value;
            RaisePropertyChanged(nameof(SelectedDiscreteColorSet));
        }
    }

    public ObservableCollection<EditableDiscreteColorItem> WorkingDiscreteColors => WizardPage.WorkingDiscreteColors;

    public EditableDiscreteColorItem? SelectedWorkingDiscreteColor
    {
        get => WizardPage.SelectedWorkingDiscreteColor;
        set
        {
            if (ReferenceEquals(WizardPage.SelectedWorkingDiscreteColor, value))
            {
                return;
            }

            WizardPage.SelectedWorkingDiscreteColor = value;
            RaisePropertyChanged(nameof(SelectedWorkingDiscreteColor));
            RaisePropertyChanged(nameof(CanRemoveWorkingDiscreteColor));
        }
    }

    public bool CanRemoveWorkingDiscreteColor => WizardPage.CanRemoveWorkingDiscreteColor;

    public string NewDiscreteColorSetName
    {
        get => WizardPage.NewDiscreteColorSetName;
        set
        {
            if (string.Equals(WizardPage.NewDiscreteColorSetName, value, StringComparison.Ordinal))
            {
                return;
            }

            WizardPage.NewDiscreteColorSetName = value;
            RaisePropertyChanged(nameof(NewDiscreteColorSetName));
        }
    }

    public ObservableCollection<FullColorOrderDefinition> AvailableFullColorOrders => WizardPage.AvailableFullColorOrders;

    public FullColorOrderDefinition? SelectedFullColorOrder
    {
        get => WizardPage.SelectedFullColorOrder;
        set
        {
            if (ReferenceEquals(WizardPage.SelectedFullColorOrder, value))
            {
                return;
            }

            WizardPage.SelectedFullColorOrder = value;
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

    private void PickSingleColor()
    {
        var selectedColor = _interactionService.PickColor(WizardPage.SingleColor);
        if (selectedColor is { } color)
        {
            WizardPage.SetSingleColor(color);
        }
    }

    private void EditWorkingDiscreteColor(EditableDiscreteColorItem? item)
    {
        if (item is null)
        {
            return;
        }

        var selectedColor = _interactionService.PickColor(item.Color);
        if (selectedColor is { } color)
        {
            WizardPage.SetWorkingDiscreteColor(item, color);
        }
    }

    private void AddWorkingDiscreteColor()
    {
        WizardPage.AddWorkingDiscreteColor();
    }

    private void RemoveWorkingDiscreteColor()
    {
        WizardPage.RemoveSelectedWorkingDiscreteColor();
    }

    private async Task SaveCustomSetAsync()
    {
        try
        {
            WizardPage.SaveCustomDiscreteColorSet();
        }
        catch (InvalidOperationException ex)
        {
            await _messageService.ShowWarningAsync(ex.Message, "Color Set");
        }
    }

    private sealed class NoOpMessageService : IMessageService
    {
        public Task<MessageResult> ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowErrorAsync(string message, string caption)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowErrorAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowWarningAsync(string message, string caption)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowWarningAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowInformationAsync(string message, string caption)
            => Task.FromResult(MessageResult.OK);

        public Task<MessageResult> ShowInformationAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageResult defaultResult = MessageResult.None)
            => Task.FromResult(MessageResult.OK);
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
                _trackedWorkingDiscreteColors = WizardPage.WorkingDiscreteColors;
                HookWorkingColorHandlers(_trackedWorkingDiscreteColors);
                RaisePropertyChanged(nameof(WorkingDiscreteColors));
                RaisePropertyChanged(nameof(CanRemoveWorkingDiscreteColor));
                break;
        }
    }
}
