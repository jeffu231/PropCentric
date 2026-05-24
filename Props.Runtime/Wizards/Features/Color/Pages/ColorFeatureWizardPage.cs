using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using DrawingColor = System.Drawing.Color;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.Setup;
using Props.Abstractions.Setup.Drafts;
using Props.Abstractions.Visuals;

namespace Props.Runtime.Wizards.Features.Color.Pages;

/// <summary>
/// Wizard page for editing reusable prop color settings.
/// </summary>
[FeatureWizardPage(typeof(IHasColor), priority: 130)]
public sealed class ColorFeatureWizardPage : WizardPageBase, IFeatureWizardDraftPage
{
    private readonly IColorConfigurationCatalog _catalog;
    private bool _isSynchronizing;
    private IHasColorSettingsDraft? _draft;
    private LightType _lightType;
    private DrawingColor _singleColor;
    private DiscreteColorSetDefinition? _selectedDiscreteColorSet;
    private FullColorOrderDefinition? _selectedFullColorOrder;
    private string _newDiscreteColorSetName = string.Empty;
    private EditableDiscreteColorItem? _selectedWorkingDiscreteColor;
    private ObservableCollection<DiscreteColorSetDefinition> _availableDiscreteColorSets = [];
    private ObservableCollection<FullColorOrderDefinition> _availableFullColorOrders = [];
    private ObservableCollection<EditableDiscreteColorItem> _workingDiscreteColors = [];

    public ColorFeatureWizardPage(IColorConfigurationCatalog catalog)
    {
        _catalog = catalog;

        Title = "Color";
        Description = "Choose the light color mode and configure the selected color behavior.";

        WorkingDiscreteColors.CollectionChanged += OnWorkingDiscreteColorsCollectionChanged;
    }

    public IWizardPreviewSession? PreviewSession { get; private set; }

    public LightType LightType
    {
        get => _lightType;
        set
        {
            if (_lightType == value)
            {
                return;
            }

            _lightType = value;
            RaisePropertyChanged(nameof(LightType));
            RaisePropertyChanged(nameof(IsSingleColorMode));
            RaisePropertyChanged(nameof(IsMultipleDiscreteColorsMode));
            RaisePropertyChanged(nameof(IsFullColorMode));
            EnsureSelectionsForCurrentMode();
            ApplyCurrentStateToDraft();
        }
    }

    public DrawingColor SingleColor
    {
        get => _singleColor;
        private set
        {
            if (_singleColor.ToArgb() == value.ToArgb())
            {
                return;
            }

            _singleColor = value;
            RaisePropertyChanged(nameof(SingleColor));
            RaisePropertyChanged(nameof(SingleColorHex));
            ApplyCurrentStateToDraft();
        }
    }

    public string SingleColorHex => $"#{SingleColor.R:X2}{SingleColor.G:X2}{SingleColor.B:X2}";

    public ObservableCollection<DiscreteColorSetDefinition> AvailableDiscreteColorSets
    {
        get => _availableDiscreteColorSets;
        private set
        {
            if (ReferenceEquals(_availableDiscreteColorSets, value))
            {
                return;
            }

            _availableDiscreteColorSets = value;
            RaisePropertyChanged(nameof(AvailableDiscreteColorSets));
        }
    }

    public ObservableCollection<FullColorOrderDefinition> AvailableFullColorOrders
    {
        get => _availableFullColorOrders;
        private set
        {
            if (ReferenceEquals(_availableFullColorOrders, value))
            {
                return;
            }

            _availableFullColorOrders = value;
            RaisePropertyChanged(nameof(AvailableFullColorOrders));
        }
    }

    public DiscreteColorSetDefinition? SelectedDiscreteColorSet
    {
        get => _selectedDiscreteColorSet;
        set
        {
            if (AreNamedSetsEqual(_selectedDiscreteColorSet, value))
            {
                return;
            }

            _selectedDiscreteColorSet = value?.DeepClone();
            RaisePropertyChanged(nameof(SelectedDiscreteColorSet));

            if (!_isSynchronizing && value is not null)
            {
                LoadWorkingDiscreteColors(value.Colors);
                ApplyCurrentStateToDraft();
            }
        }
    }

    public FullColorOrderDefinition? SelectedFullColorOrder
    {
        get => _selectedFullColorOrder;
        set
        {
            if (AreNamedSetsEqual(_selectedFullColorOrder, value))
            {
                return;
            }

            _selectedFullColorOrder = value?.DeepClone();
            RaisePropertyChanged(nameof(SelectedFullColorOrder));
            ApplyCurrentStateToDraft();
        }
    }

    public ObservableCollection<EditableDiscreteColorItem> WorkingDiscreteColors
    {
        get => _workingDiscreteColors;
        private set
        {
            if (ReferenceEquals(_workingDiscreteColors, value))
            {
                return;
            }

            if (_workingDiscreteColors is not null)
            {
                _workingDiscreteColors.CollectionChanged -= OnWorkingDiscreteColorsCollectionChanged;
                foreach (var item in _workingDiscreteColors)
                {
                    item.PropertyChanged -= OnWorkingDiscreteColorPropertyChanged;
                }
            }

            _workingDiscreteColors = value;
            _workingDiscreteColors.CollectionChanged += OnWorkingDiscreteColorsCollectionChanged;
            foreach (var item in _workingDiscreteColors)
            {
                item.PropertyChanged += OnWorkingDiscreteColorPropertyChanged;
            }

            RaisePropertyChanged(nameof(WorkingDiscreteColors));
        }
    }

    public EditableDiscreteColorItem? SelectedWorkingDiscreteColor
    {
        get => _selectedWorkingDiscreteColor;
        set
        {
            if (ReferenceEquals(_selectedWorkingDiscreteColor, value))
            {
                return;
            }

            _selectedWorkingDiscreteColor = value;
            RaisePropertyChanged(nameof(SelectedWorkingDiscreteColor));
            RaisePropertyChanged(nameof(CanRemoveWorkingDiscreteColor));
        }
    }

    public bool CanRemoveWorkingDiscreteColor => SelectedWorkingDiscreteColor is not null;

    public string NewDiscreteColorSetName
    {
        get => _newDiscreteColorSetName;
        set
        {
            value ??= string.Empty;
            if (string.Equals(_newDiscreteColorSetName, value, StringComparison.Ordinal))
            {
                return;
            }

            _newDiscreteColorSetName = value;
            RaisePropertyChanged(nameof(NewDiscreteColorSetName));
        }
    }

    public bool IsSingleColorMode => LightType == LightType.SingleColor;

    public bool IsMultipleDiscreteColorsMode => LightType == LightType.MultipleDiscreteColors;

    public bool IsFullColorMode => LightType == LightType.FullColor;

    public void Initialize(IPropDraft draft, IWizardPreviewSession previewSession)
    {
        if (draft is not IHasColorSettingsDraft colorDraft)
        {
            throw new InvalidOperationException($"Draft {draft.GetType()} does not implement {nameof(IHasColorSettingsDraft)}.");
        }

        PreviewSession = previewSession;
        _draft = colorDraft;

        AvailableDiscreteColorSets = new ObservableCollection<DiscreteColorSetDefinition>(
            _catalog.GetDiscreteColorSets().Select(definition => definition.DeepClone()));
        AvailableFullColorOrders = new ObservableCollection<FullColorOrderDefinition>(
            _catalog.GetFullColorOrders().Select(definition => definition.DeepClone()));

        var configuration = colorDraft.ColorConfiguration.DeepClone();
        EnsureOptionContainsCurrentSelections(configuration);

        _isSynchronizing = true;
        try
        {
            LightType = configuration.LightType;
            _singleColor = configuration.SingleColor;
            RaisePropertyChanged(nameof(SingleColor));
            RaisePropertyChanged(nameof(SingleColorHex));

            _selectedDiscreteColorSet = FindMatchingDiscreteColorSet(configuration.DiscreteColorSet);
            RaisePropertyChanged(nameof(SelectedDiscreteColorSet));

            _selectedFullColorOrder = FindMatchingFullColorOrder(configuration.FullColorOrder);
            RaisePropertyChanged(nameof(SelectedFullColorOrder));

            LoadWorkingDiscreteColors(configuration.DiscreteColorSet?.Colors
                                      ?? [DrawingColor.White]);
        }
        finally
        {
            _isSynchronizing = false;
        }

        EnsureSelectionsForCurrentMode();
        ApplyCurrentStateToDraft();
    }

    public void SetSingleColor(DrawingColor color)
    {
        SingleColor = color;
    }

    public void AddWorkingDiscreteColor()
    {
        var item = new EditableDiscreteColorItem(DrawingColor.White, WorkingDiscreteColors.Count + 1);
        item.PropertyChanged += OnWorkingDiscreteColorPropertyChanged;
        WorkingDiscreteColors.Add(item);
        SelectedWorkingDiscreteColor = item;
        ApplyCurrentStateToDraft();
    }

    public void RemoveSelectedWorkingDiscreteColor()
    {
        if (SelectedWorkingDiscreteColor is null)
        {
            return;
        }

        SelectedWorkingDiscreteColor.PropertyChanged -= OnWorkingDiscreteColorPropertyChanged;
        var removedIndex = WorkingDiscreteColors.IndexOf(SelectedWorkingDiscreteColor);
        WorkingDiscreteColors.Remove(SelectedWorkingDiscreteColor);
        RenumberWorkingDiscreteColors();
        SelectedWorkingDiscreteColor = removedIndex >= 0 && removedIndex < WorkingDiscreteColors.Count
            ? WorkingDiscreteColors[removedIndex]
            : WorkingDiscreteColors.LastOrDefault();
        ApplyCurrentStateToDraft();
    }

    public void SetWorkingDiscreteColor(EditableDiscreteColorItem item, DrawingColor color)
    {
        ArgumentNullException.ThrowIfNull(item);
        item.Color = color;
        ApplyCurrentStateToDraft();
    }

    public void SaveCustomDiscreteColorSet()
    {
        if (string.IsNullOrWhiteSpace(NewDiscreteColorSetName))
        {
            throw new InvalidOperationException("Custom color set names must be provided before saving.");
        }

        if (WorkingDiscreteColors.Count == 0)
        {
            throw new InvalidOperationException("Custom color sets must contain at least one color.");
        }

        var definition = new DiscreteColorSetDefinition(
            NewDiscreteColorSetName.Trim(),
            WorkingDiscreteColors.Select(item => item.Color).ToArray());

        _catalog.SaveDiscreteColorSet(definition);

        var clone = definition.DeepClone();
        AvailableDiscreteColorSets.Add(clone);
        SelectedDiscreteColorSet = clone;
        NewDiscreteColorSetName = string.Empty;
        ApplyCurrentStateToDraft();
    }

    public override ISummaryItem GetSummary()
    {
        return new SummaryItem
        {
            Title = Title,
            Summary = LightType switch
            {
                LightType.SingleColor => $"Mode: Single Color\nColor: {SingleColorHex}",
                LightType.MultipleDiscreteColors => $"Mode: Multiple Discrete Colors\nSet: {SelectedDiscreteColorSet?.Name ?? "None"}\nColors: {string.Join(", ", WorkingDiscreteColors.Select(item => item.Hex))}",
                LightType.FullColor => $"Mode: Full Color\nOrder: {SelectedFullColorOrder?.Name ?? "None"}",
                _ => "No color configuration selected."
            }
        };
    }

    private void EnsureOptionContainsCurrentSelections(LightColorConfiguration configuration)
    {
        if (configuration.DiscreteColorSet is { } discrete
            && AvailableDiscreteColorSets.All(existing => !string.Equals(existing.Name, discrete.Name, StringComparison.OrdinalIgnoreCase)))
        {
            AvailableDiscreteColorSets.Add(discrete.DeepClone());
        }

        if (configuration.FullColorOrder is { } fullColorOrder
            && AvailableFullColorOrders.All(existing => !string.Equals(existing.Name, fullColorOrder.Name, StringComparison.OrdinalIgnoreCase)))
        {
            AvailableFullColorOrders.Add(fullColorOrder.DeepClone());
        }
    }

    private void EnsureSelectionsForCurrentMode()
    {
        if (_isSynchronizing)
        {
            return;
        }

        _isSynchronizing = true;
        try
        {
            if (LightType == LightType.MultipleDiscreteColors)
            {
                if (SelectedDiscreteColorSet is null && AvailableDiscreteColorSets.Count > 0)
                {
                    _selectedDiscreteColorSet = AvailableDiscreteColorSets[0].DeepClone();
                    RaisePropertyChanged(nameof(SelectedDiscreteColorSet));
                    LoadWorkingDiscreteColors(_selectedDiscreteColorSet.Colors);
                }

                if (WorkingDiscreteColors.Count == 0)
                {
                    LoadWorkingDiscreteColors([DrawingColor.White]);
                }
            }

            if (LightType == LightType.FullColor && SelectedFullColorOrder is null && AvailableFullColorOrders.Count > 0)
            {
                _selectedFullColorOrder = AvailableFullColorOrders[0].DeepClone();
                RaisePropertyChanged(nameof(SelectedFullColorOrder));
            }
        }
        finally
        {
            _isSynchronizing = false;
        }
    }

    private void ApplyCurrentStateToDraft()
    {
        if (_draft is null || _isSynchronizing)
        {
            return;
        }

        _draft.ColorConfiguration = LightType switch
        {
            LightType.SingleColor => new LightColorConfiguration(
                LightType.SingleColor,
                SingleColor,
                SelectedDiscreteColorSet?.DeepClone(),
                SelectedFullColorOrder?.DeepClone()),
            LightType.MultipleDiscreteColors => new LightColorConfiguration(
                LightType.MultipleDiscreteColors,
                SingleColor,
                CreateWorkingDiscreteColorSetSnapshot(),
                SelectedFullColorOrder?.DeepClone()),
            LightType.FullColor => new LightColorConfiguration(
                LightType.FullColor,
                SingleColor,
                SelectedDiscreteColorSet?.DeepClone(),
                SelectedFullColorOrder?.DeepClone()),
            _ => _draft.ColorConfiguration
        };
    }

    private DiscreteColorSetDefinition CreateWorkingDiscreteColorSetSnapshot()
    {
        var name = SelectedDiscreteColorSet?.Name;
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Custom";
        }

        return new DiscreteColorSetDefinition(name, WorkingDiscreteColors.Select(item => item.Color).ToArray());
    }

    private void LoadWorkingDiscreteColors(IEnumerable<DrawingColor> colors)
    {
        var items = new ObservableCollection<EditableDiscreteColorItem>(
            colors.Select((color, index) => new EditableDiscreteColorItem(color, index + 1)));
        WorkingDiscreteColors = items;
    }

    private void RenumberWorkingDiscreteColors()
    {
        for (var index = 0; index < WorkingDiscreteColors.Count; index++)
        {
            WorkingDiscreteColors[index].DisplayIndex = index + 1;
        }
    }

    private DiscreteColorSetDefinition? FindMatchingDiscreteColorSet(DiscreteColorSetDefinition? definition)
    {
        if (definition is null)
        {
            return null;
        }

        return AvailableDiscreteColorSets.FirstOrDefault(existing =>
                   string.Equals(existing.Name, definition.Name, StringComparison.OrdinalIgnoreCase))
               ?.DeepClone()
               ?? definition.DeepClone();
    }

    private FullColorOrderDefinition? FindMatchingFullColorOrder(FullColorOrderDefinition? definition)
    {
        if (definition is null)
        {
            return null;
        }

        return AvailableFullColorOrders.FirstOrDefault(existing =>
                   string.Equals(existing.Name, definition.Name, StringComparison.OrdinalIgnoreCase))
               ?.DeepClone()
               ?? definition.DeepClone();
    }

    private void OnWorkingDiscreteColorsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (EditableDiscreteColorItem item in e.OldItems)
            {
                item.PropertyChanged -= OnWorkingDiscreteColorPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (EditableDiscreteColorItem item in e.NewItems)
            {
                item.PropertyChanged -= OnWorkingDiscreteColorPropertyChanged;
                item.PropertyChanged += OnWorkingDiscreteColorPropertyChanged;
            }
        }

        RaisePropertyChanged(nameof(CanRemoveWorkingDiscreteColor));
    }

    private void OnWorkingDiscreteColorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(EditableDiscreteColorItem.Color) or nameof(EditableDiscreteColorItem.Hex))
        {
            ApplyCurrentStateToDraft();
        }
    }

    private static bool AreNamedSetsEqual<TDefinition>(TDefinition? left, TDefinition? right)
        where TDefinition : class
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left switch
        {
            DiscreteColorSetDefinition leftDiscrete when right is DiscreteColorSetDefinition rightDiscrete =>
                string.Equals(leftDiscrete.Name, rightDiscrete.Name, StringComparison.OrdinalIgnoreCase),
            FullColorOrderDefinition leftOrder when right is FullColorOrderDefinition rightOrder =>
                string.Equals(leftOrder.Name, rightOrder.Name, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }
}
