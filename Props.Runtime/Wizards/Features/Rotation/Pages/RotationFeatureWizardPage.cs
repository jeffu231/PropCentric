using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Props.Abstractions.Features;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup;
using Props.Abstractions.Setup.Drafts;
using Props.Abstractions.Visuals;

namespace Props.Runtime.Wizards.Features.Rotation.Pages;

/// <summary>
/// Wizard page for editing prop axis rotations.
/// </summary>
[FeatureWizardPage(typeof(ICanRotate), priority: 140)]
public sealed class RotationFeatureWizardPage : WizardPageBase, IFeatureWizardDraftPage
{
    public RotationFeatureWizardPage()
    {
        Title = "Rotation";
        Description = "Adjust prop axis rotations and preview the transformed result.";
        Rotations = [];
    }

    public ObservableCollection<RotationFeatureWizardItem> Rotations
    {
        get => GetValue<ObservableCollection<RotationFeatureWizardItem>>(RotationsProperty);
        set
        {
            var current = Rotations;
            if (ReferenceEquals(current, value))
            {
                return;
            }

            if (current is not null)
            {
                current.CollectionChanged -= OnRotationsCollectionChanged;
                foreach (var rotation in current)
                {
                    rotation.PropertyChanged -= OnRotationItemPropertyChanged;
                }
            }

            SetValue(RotationsProperty, value);

            value.CollectionChanged += OnRotationsCollectionChanged;
            foreach (var rotation in value)
            {
                rotation.PropertyChanged += OnRotationItemPropertyChanged;
            }

            RefreshSummary();
        }
    }

    private static readonly IPropertyData RotationsProperty =
        RegisterProperty<ObservableCollection<RotationFeatureWizardItem>>(nameof(Rotations), []);

    public string RotationSummary
    {
        get => GetValue<string>(RotationSummaryProperty);
        private set => SetValue(RotationSummaryProperty, value);
    }

    private static readonly IPropertyData RotationSummaryProperty =
        RegisterProperty<string>(nameof(RotationSummary), string.Empty);

    public IWizardPreviewSession? PreviewSession { get; private set; }

    public void Initialize(IPropDraft draft, IWizardPreviewSession previewSession)
    {
        if (draft is not IHasRotationsDraft rotationsDraft)
        {
            throw new InvalidOperationException($"Draft {draft.GetType()} does not implement {nameof(IHasRotationsDraft)}.");
        }

        PreviewSession = previewSession;
        Rotations = new ObservableCollection<RotationFeatureWizardItem>(
            rotationsDraft.AxisRotations.Select(rotation => new RotationFeatureWizardItem(rotation)));
        RefreshSummary();
    }

    public override ISummaryItem GetSummary()
    {
        return new SummaryItem
        {
            Title = Title,
            Summary = RotationSummary
        };
    }

    internal void RefreshSummary()
    {
        RotationSummary = Rotations.Count == 0
            ? "No rotations configured."
            : string.Join(Environment.NewLine, Rotations.Select(rotation => $"{rotation.Axis}: {rotation.RotationAngle}°"));
    }

    private void OnRotationsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (RotationFeatureWizardItem rotation in e.OldItems)
            {
                rotation.PropertyChanged -= OnRotationItemPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (RotationFeatureWizardItem rotation in e.NewItems)
            {
                rotation.PropertyChanged += OnRotationItemPropertyChanged;
            }
        }

        RefreshSummary();
    }

    private void OnRotationItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(RotationFeatureWizardItem.Axis) or nameof(RotationFeatureWizardItem.RotationAngle))
        {
            RefreshSummary();
        }
    }
}

/// <summary>
/// Page-facing wrapper around shared draft rotation state.
/// </summary>
public sealed class RotationFeatureWizardItem : ModelBase
{
    private readonly AxisRotationModel _rotation;

    public RotationFeatureWizardItem(AxisRotationModel rotation)
    {
        _rotation = rotation;
        RotationAngleDefault = rotation.RotationAngle;
        Axis = AxisRotationModel.ConvertAxis(rotation.Axis);
        RotationAngle = rotation.RotationAngle;
    }

    public List<string> Axes { get; } = ["X", "Y", "Z"];

    public string Axis
    {
        get => GetValue<string>(AxisProperty);
        set
        {
            SetValue(AxisProperty, value);
            _rotation.ConvertAxis(value);
        }
    }

    private static readonly IPropertyData AxisProperty = RegisterProperty<string>(nameof(Axis), "X");

    public int RotationAngle
    {
        get => GetValue<int>(RotationAngleProperty);
        set
        {
            SetValue(RotationAngleProperty, value);
            _rotation.RotationAngle = value;
        }
    }

    private static readonly IPropertyData RotationAngleProperty = RegisterProperty<int>(nameof(RotationAngle));

    public int RotationAngleDefault
    {
        get => GetValue<int>(RotationAngleDefaultProperty);
        private set => SetValue(RotationAngleDefaultProperty, value);
    }

    private static readonly IPropertyData RotationAngleDefaultProperty =
        RegisterProperty<int>(nameof(RotationAngleDefault));
}
