using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup;

namespace Props.Abstractions.Visuals;

public interface IWizardPreviewCoordinator<in TDraft>
    where TDraft : class, IPropDraft
{
    IPropVisualModel BuildPreview(TDraft draft);
}
