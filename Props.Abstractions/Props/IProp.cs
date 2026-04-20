using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Props;

public interface IProp
{
    Guid Id { get; init; }
    IPropVisualModel VisualModel { get; }
    event EventHandler? VisualModelChanged;
}