using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Visuals;

public interface IPropVisualModelFactory<in TVisualInput>
{
    IPropVisualModel Create(TVisualInput input);
}
