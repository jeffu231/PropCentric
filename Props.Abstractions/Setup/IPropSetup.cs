using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

/// <summary>
/// This is a wrapper around the creation and editing of props to keep the actual PRop implementation out of the Wizard
/// </summary>
public interface IPropSetup
{
    Task<IPropGroup?> CreateAsync();
    Task<IProp> EditAsync(IProp existing);
}