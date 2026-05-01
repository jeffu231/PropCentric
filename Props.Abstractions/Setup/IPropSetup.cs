using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

public interface IPropSetup
{
    Task<IPropGroup<IProp>?> CreateAsync();
    Task<IProp> EditAsync(IProp existing);
}

public interface IPropSetup<TProp> : IPropSetup where TProp : IProp
{
    new Task<IPropGroup<TProp>?> CreateAsync();
    Task<TProp> EditAsync(TProp existing);
}