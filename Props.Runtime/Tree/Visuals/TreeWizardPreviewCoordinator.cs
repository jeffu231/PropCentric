using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree.Setup;

namespace Props.Runtime.Tree.Visuals;

/// <summary>
/// Coordinates incremental <see cref="TreePropVisualModel"/> rebuilds during wizard editing,
/// skipping geometry generation when the inputs have not changed.
/// </summary>
public sealed class TreeWizardPreviewCoordinator : IWizardPreviewCoordinator<TreePropDraft>
{
    private readonly IVisualInputMapper<TreePropDraft, TreeVisualInput> _mapper;
    private readonly IPropVisualModelBuilder<TreeVisualInput, TreePropVisualModel> _builder;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private TreeVisualInput? _lastInput;
    private IPropVisualModel? _lastModel;

    /// <summary>Initializes a new instance of the <see cref="TreeWizardPreviewCoordinator"/> class.</summary>
    /// <param name="mapper">The mapper that projects a draft onto a <see cref="TreeVisualInput"/>.</param>
    /// <param name="builder">The factory that produces a visual model from a <see cref="TreeVisualInput"/>.</param>
    public TreeWizardPreviewCoordinator(
        IVisualInputMapper<TreePropDraft, TreeVisualInput> mapper,
        IPropVisualModelBuilder<TreeVisualInput, TreePropVisualModel> builder)
    {
        _mapper = mapper;
        _builder = builder;
    }

    /// <summary>
    /// Creates the Visual Model for a preview to use based on the draft input data.
    /// </summary>
    /// <param name="draft"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IPropVisualModel> BuildPreviewAsync(TreePropDraft draft, CancellationToken cancellationToken = default)
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
