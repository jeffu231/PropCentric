using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;

namespace Props.Runtime.PolyLine.Visuals;

/// <summary>
/// Coordinates incremental <see cref="PolyLinePropVisualModel"/> rebuilds during wizard editing,
/// skipping geometry generation when the inputs have not changed.
/// </summary>
public sealed class PolyLineWizardPreviewCoordinator : IWizardPreviewCoordinator<PolyLinePropDraft>
{
    private readonly IVisualInputMapper<PolyLinePropDraft, PolyLineVisualInput> _mapper;
    private readonly IPropVisualModelBuilder<PolyLineVisualInput, PolyLinePropVisualModel> _builder;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private PolyLineVisualInput? _lastInput;
    private IPropVisualModel? _lastModel;

    /// <summary>Initializes a new instance of the <see cref="PolyLineWizardPreviewCoordinator"/> class.</summary>
    /// <param name="mapper">The mapper that projects a draft onto a <see cref="PolyLineVisualInput"/>.</param>
    /// <param name="builder">The factory that produces a visual model from a <see cref="PolyLineVisualInput"/>.</param>
    public PolyLineWizardPreviewCoordinator(
        IVisualInputMapper<PolyLinePropDraft, PolyLineVisualInput> mapper,
        IPropVisualModelBuilder<PolyLineVisualInput, PolyLinePropVisualModel> builder)
    {
        _mapper = mapper;
        _builder = builder;
    }

    /// <inheritdoc />
    public async Task<IPropVisualModel> BuildPreviewAsync(PolyLinePropDraft draft, CancellationToken cancellationToken = default)
    {
        var input = _mapper.Map(draft);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (input == _lastInput && _lastModel is not null)
            {
                return _lastModel;
            }

            var model = await Task.Run(() => _builder.Create(input), cancellationToken).ConfigureAwait(false);
            _lastInput = input;
            _lastModel = model;
            return model;
        }
        finally
        {
            _gate.Release();
        }
    }
}
