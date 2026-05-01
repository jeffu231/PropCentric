using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Props;

public abstract class Prop : IProp
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "Tree Prop";

    private IPropVisualModel? _visualModel;
    public IPropVisualModel VisualModel => _visualModel ??= BuildVisualModel();

    public event EventHandler? VisualModelChanged;

    protected abstract IPropVisualModel BuildVisualModel();

    protected void InvalidateVisualModel()
    {
        _visualModel = null;
        VisualModelChanged?.Invoke(this, EventArgs.Empty);
    }
}