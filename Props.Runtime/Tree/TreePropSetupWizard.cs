using Props.Abstractions.Props;
using Props.Abstractions.Wizards;

namespace Props.Runtime.Tree;

public class TreePropSetupWizard: IPropSetupWizard<TreeProp>
{
    public Task<TreeProp> EditAsync(TreeProp existing)
    {
        return Task.FromResult(existing);
    }

    Task<TreeProp> IPropSetupWizard<TreeProp>.CreateAsync()
    {
        return Task.FromResult(new TreeProp());
    }

    public Task<IProp> EditAsync(IProp existing)
    {
        //Do the actual editing here
        return Task.FromResult<IProp>(existing);
    }

    public Task<IProp> CreateAsync()
    {
        return Task.FromResult<IProp>(new TreeProp());
    }
}