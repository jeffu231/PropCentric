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
        PreviewBuilder = cancellationToken => featureWizardPage.PreviewSession?.BuildPreviewAsync(cancellationToken)
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

    [ViewModelToModel]
    public LightType LightType
    {
        get => GetValue<LightType>(LightTypeProperty);
        set => SetValue(LightTypeProperty, value);
    }

    private static readonly IPropertyData LightTypeProperty = RegisterProperty<LightType>(nameof(LightType));

    public bool IsSingleColorMode => WizardPage.IsSingleColorMode;

    public bool IsMultipleDiscreteColorsMode => WizardPage.IsMultipleDiscreteColorsMode;

    public bool IsFullColorMode => WizardPage.IsFullColorMode;

    public DrawingColor SingleColor => WizardPage.SingleColor;

    public string SingleColorHex => WizardPage.SingleColorHex;

    public ObservableCollection<DiscreteColorSetDefinition> AvailableDiscreteColorSets => WizardPage.AvailableDiscreteColorSets;

    [ViewModelToModel]
    public DiscreteColorSetDefinition? SelectedDiscreteColorSet
    {
        get => GetValue<DiscreteColorSetDefinition?>(SelectedDiscreteColorSetProperty);
        set => SetValue(SelectedDiscreteColorSetProperty, value);
    }

    private static readonly IPropertyData SelectedDiscreteColorSetProperty =
        RegisterProperty<DiscreteColorSetDefinition?>(nameof(SelectedDiscreteColorSet));

    public ObservableCollection<EditableDiscreteColorItem> WorkingDiscreteColors => WizardPage.WorkingDiscreteColors;

    [ViewModelToModel]
    public EditableDiscreteColorItem? SelectedWorkingDiscreteColor
    {
        get => GetValue<EditableDiscreteColorItem?>(SelectedWorkingDiscreteColorProperty);
        set => SetValue(SelectedWorkingDiscreteColorProperty, value);
    }

    private static readonly IPropertyData SelectedWorkingDiscreteColorProperty =
        RegisterProperty<EditableDiscreteColorItem?>(nameof(SelectedWorkingDiscreteColor));

    public bool CanRemoveWorkingDiscreteColor => WizardPage.CanRemoveWorkingDiscreteColor;

    [ViewModelToModel]
    public string NewDiscreteColorSetName
    {
        get => GetValue<string>(NewDiscreteColorSetNameProperty);
        set => SetValue(NewDiscreteColorSetNameProperty, value);
    }

    private static readonly IPropertyData NewDiscreteColorSetNameProperty =
        RegisterProperty<string>(nameof(NewDiscreteColorSetName), string.Empty);

    public ObservableCollection<FullColorOrderDefinition> AvailableFullColorOrders => WizardPage.AvailableFullColorOrders;

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
