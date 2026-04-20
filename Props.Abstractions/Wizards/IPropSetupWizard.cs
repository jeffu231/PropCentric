using Props.Abstractions.Props;

namespace Props.Abstractions.Wizards;

public interface IPropSetupWizard
{
    Task<IProp> CreateAsync();
    Task<IProp> EditAsync(IProp existing);
}

public interface IPropSetupWizard<TProp> : IPropSetupWizard where TProp : IProp
{
    new Task<TProp> CreateAsync();
    Task<TProp> EditAsync(TProp existing);
}