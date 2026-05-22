using Catel.Data;
using Orc.Wizard;
using Props.Abstractions.PropVisualModels;
using Props.OpenGlCommon;
using Props.Runtime.Utilities;

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
