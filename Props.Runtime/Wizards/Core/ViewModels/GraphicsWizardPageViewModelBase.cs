using System.ComponentModel;
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
    private int _previewRequestVersion;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="wizardPage">Wizard page model</param>
    protected GraphicsWizardPageViewModelBase(TWizardPage wizardPage) : base(wizardPage)
    {
        DrawingEngine = new OpenGLPropDrawingEngine();
        DrawingEngine.SetModels([]);
        PropertyChanged += OnViewModelPropertyChanged;
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
    protected Func<CancellationToken, Task<IPropVisualModel>>? PreviewBuilder { get; set; }

    /// <summary>
    /// OpenGL prop drawing engine.
    /// </summary>
    public OpenGLPropDrawingEngine DrawingEngine { get; }

    /// <inheritdoc />
    public bool IsDrawingEngineInitialized => DrawingEngine.IsInitialized;

    protected override async Task InitializeAsync()
    {
        await TriggerPreviewRebuildAsync();
    }

    /// <summary>
    /// Schedules a preview rebuild using the debounced refresh path.
    /// </summary>
    protected void SchedulePreviewRebuild()
    {
        _ = SchedulePreviewRebuildAsync();
    }

    private async Task SchedulePreviewRebuildAsync()
    {
        try
        {
            await _previewDebouncer.InvokeAsync(TriggerPreviewRebuildAsync);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CurrentPreviewModel))
        {
            return;
        }

        SchedulePreviewRebuild();
    }

    private async Task TriggerPreviewRebuildAsync(CancellationToken cancellationToken = default)
    {
        if (PreviewBuilder is null)
        {
            return;
        }

        var requestVersion = Interlocked.Increment(ref _previewRequestVersion);
        var previewModel = await PreviewBuilder(cancellationToken);
        if (cancellationToken.IsCancellationRequested || requestVersion != _previewRequestVersion)
        {
            return;
        }

        CurrentPreviewModel = previewModel;
        DrawingEngine.SetModels(CurrentPreviewModel is null ? [] : [CurrentPreviewModel]);
    }
}
