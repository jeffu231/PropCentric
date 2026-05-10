using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

/// <summary>
/// This is a wrapper around the creation and editing of props to keep the actual PRop implementation out of the Wizard
/// </summary>
public interface IPropSetup
{
    /// <summary>Creates a prop group through the setup flow.</summary>
    /// <param name="context">Optional external input required by the setup flow.</param>
    /// <returns>The created prop group, or <see langword="null"/> when the flow is cancelled.</returns>
    Task<IPropGroup?> CreateAsync(IPropSetupContext? context = null);

    /// <summary>Edits an existing prop through the setup flow.</summary>
    /// <param name="existing">The existing prop instance to edit.</param>
    /// <param name="context">Optional external input required by the setup flow.</param>
    /// <returns>The edited prop instance.</returns>
    Task<IProp> EditAsync(IProp existing, IPropSetupContext? context = null);
}
