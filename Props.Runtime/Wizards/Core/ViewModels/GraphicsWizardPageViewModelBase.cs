using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Props.Abstractions.PropVisualModels;
using Props.OpenGlCommon;
using Props.Runtime.Utilities;
using Props.Runtime.ViewModels;

namespace Props.Runtime.Wizards.Core.ViewModels;

public class GraphicsWizardPageViewModelBase<TWizardPage> : WizardPageViewModelBase<TWizardPage>, IPropWizardPageViewModel
    where TWizardPage : class, IWizardPage
{
    private readonly Debouncer _previewDebouncer = new(TimeSpan.FromMilliseconds(150));

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="wizardPage">Wizard page model</param>
    protected GraphicsWizardPageViewModelBase(TWizardPage wizardPage) : base(wizardPage)
    {
        DrawingEngine = new OpenGLPropDrawingEngine();
        DrawingEngine.SetModels([]);

        AttachRotationHandlers(Rotations);

        PropertyChanged += (_, _) => _previewDebouncer.Invoke(TriggerPreviewRebuild);
    }

    /// <summary>
    /// Holds the most recently built preview model for the current wizard page.
    /// </summary>
    protected IPropVisualModel? CurrentPreviewModel
    {
        get => GetValue<IPropVisualModel?>(CurrentPreviewModelProperty);
        set => SetValue(CurrentPreviewModelProperty, value);
    }

    private static readonly IPropertyData CurrentPreviewModelProperty =
        RegisterProperty<IPropVisualModel?>(nameof(CurrentPreviewModel));

    /// <summary>
    /// Set by a concrete ViewModel to supply a fresh <see cref="IPropVisualModel"/> on each preview rebuild.
    /// The closure should read the current draft state at call time.
    /// </summary>
    protected Func<IPropVisualModel>? PreviewBuilder { get; set; }

    /// <summary>
    /// Collection of rotations to support rotating the props around the x,y, and z axis.
    /// </summary>
    [ViewModelToModel]
    public ObservableCollection<AxisRotationViewModel> Rotations
    {
        //TODO this does not belong in here. It should be part of the actual Prop View Models.
        get => GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty);
        set
        {
            var currentRotations = GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty);
            if (!ReferenceEquals(currentRotations, value))
            {
                DetachRotationHandlers(currentRotations);
                SetValue(RotationsProperty, value);
                AttachRotationHandlers(value);
            }

            var rotations = GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty);
            if (rotations is null)
            {
                return;
            }

            for (int index = 0; index < rotations.Count; index++)
            {
                rotations[index].RotationAngleDefault = rotations[index].RotationAngle;
            }
        }
    }

    private static readonly IPropertyData RotationsProperty =
        RegisterProperty<ObservableCollection<AxisRotationViewModel>>(nameof(Rotations));

    /// <summary>
    /// OpenGL prop drawing engine.
    /// </summary>
    public OpenGLPropDrawingEngine DrawingEngine { get; }

    /// <inheritdoc />
    public bool IsDrawingEngineInitialized => DrawingEngine.IsInitialized;

    protected override async Task InitializeAsync()
    {
        TriggerPreviewRebuild();
    }

    /// <summary>
    /// Schedules a preview rebuild using the debounced refresh path.
    /// </summary>
    protected void SchedulePreviewRebuild()
    {
        _previewDebouncer.Invoke(TriggerPreviewRebuild);
    }

    private void AttachRotationHandlers(ObservableCollection<AxisRotationViewModel>? rotationCollection)
    {
        if (rotationCollection is null)
        {
            return;
        }

        rotationCollection.CollectionChanged -= OnRotationsCollectionChanged;
        rotationCollection.CollectionChanged += OnRotationsCollectionChanged;

        foreach (var rotation in rotationCollection)
        {
            rotation.RotationChanged -= OnRotationChanged;
            rotation.RotationChanged += OnRotationChanged;
        }
    }

    private void DetachRotationHandlers(ObservableCollection<AxisRotationViewModel>? rotationCollection)
    {
        if (rotationCollection is null)
        {
            return;
        }

        rotationCollection.CollectionChanged -= OnRotationsCollectionChanged;

        foreach (var rotation in rotationCollection)
        {
            rotation.RotationChanged -= OnRotationChanged;
        }
    }

    private void OnRotationsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (AxisRotationViewModel oldItem in e.OldItems)
            {
                oldItem.RotationChanged -= OnRotationChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (AxisRotationViewModel newItem in e.NewItems)
            {
                newItem.RotationChanged -= OnRotationChanged;
                newItem.RotationChanged += OnRotationChanged;
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Reset && sender is ObservableCollection<AxisRotationViewModel> rotationCollection)
        {
            DetachRotationHandlers(rotationCollection);
            AttachRotationHandlers(rotationCollection);
        }
    }

    private void OnRotationChanged(object? sender, EventArgs e)
    {
        if (sender is not AxisRotationViewModel newRotation)
        {
            return;
        }

        var duplicateRotation = Rotations.FirstOrDefault(x => x != newRotation && x.Axis == newRotation.Axis);
        if (duplicateRotation is null)
        {
            return;
        }

        var otherRotation = Rotations.FirstOrDefault(x => x != newRotation && x != duplicateRotation);
        if (otherRotation is null)
        {
            return;
        }

        var missingAxis = newRotation.Axes.FirstOrDefault(x => x != duplicateRotation.Axis && x != otherRotation.Axis);
        duplicateRotation.Axis = missingAxis;
        (duplicateRotation.RotationAngle, newRotation.RotationAngle) = (newRotation.RotationAngle, duplicateRotation.RotationAngle);
    }

    private void TriggerPreviewRebuild()
    {
        if (PreviewBuilder is null)
        {
            return;
        }
        CurrentPreviewModel = PreviewBuilder();
        DrawingEngine.SetModels(CurrentPreviewModel is null ? [] : [CurrentPreviewModel]);
    }
}
