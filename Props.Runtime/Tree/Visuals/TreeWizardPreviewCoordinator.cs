using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree.Setup;

namespace Props.Runtime.Tree.Visuals;

public sealed class TreeWizardPreviewCoordinator : IWizardPreviewCoordinator<TreePropDraft>
{
    private readonly IVisualInputMapper<TreePropDraft, TreeVisualInput> _mapper;
    private readonly IPropVisualModelFactory<TreeVisualInput> _factory;
    private TreeVisualInput? _lastInput;
    private IPropVisualModel? _lastModel;

    public TreeWizardPreviewCoordinator(
        IVisualInputMapper<TreePropDraft, TreeVisualInput> mapper,
        IPropVisualModelFactory<TreeVisualInput> factory)
    {
        _mapper = mapper;
        _factory = factory;
    }

    public IPropVisualModel BuildPreview(TreePropDraft draft)
    {
        var input = _mapper.Map(draft);
        if (input == _lastInput && _lastModel is not null)
            return _lastModel;
        _lastInput = input;
        _lastModel = _factory.Create(input);
        return _lastModel;
    }
}
