using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Props;

public interface IProp
{
    Guid Id { get; init; }
    string Name { get; set; }
    IPropVisualModel VisualModel { get; }
    event EventHandler? VisualModelChanged;
}