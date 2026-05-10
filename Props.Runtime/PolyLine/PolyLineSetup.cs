using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.Tree.Setup;

namespace Props.Runtime.PolyLine;

public class PolyLineSetup( IFeatureWizardPageResolver featurePageResolver,
    IPropFactory propFactory,
    IPropDraftMapper<PolyLinePropDraft, PolyLineProp> draftMapper,
    IWizardPreviewCoordinator<TreePropDraft> previewCoordinator) : IPropSetup
{
    public Task<IPropGroup?> CreateAsync(IPropSetupContext? context = null)
    {
        throw new NotImplementedException();
    }

    public Task<IProp> EditAsync(IProp existing, IPropSetupContext? context = null)
    {
        throw new NotImplementedException();
    }
}