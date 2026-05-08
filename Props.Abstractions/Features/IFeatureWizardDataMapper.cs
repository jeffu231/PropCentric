using Props.Abstractions.Props;

namespace Props.Abstractions.Features;

/// <summary>
/// Transfers data between a feature wizard page and the prop it configures.
/// </summary>
/// <remarks>
/// Implementations own all casting to the specific feature interface and all data conversion.
/// Instances are created via <c>ActivatorUtilities</c> with the companion page as a constructor argument.
/// Call <see cref="PopulateFrom"/> before opening the wizard and <see cref="ApplyTo"/> after the user confirms.
/// </remarks>
public interface IFeatureWizardDataMapper
{
    /// <summary>Writes the wizard page's current state to the prop.</summary>
    /// <param name="prop">The prop to which the page data is applied.</param>
    void ApplyTo(IProp prop);

    /// <summary>Reads the prop's current state into the wizard page.</summary>
    /// <param name="prop">The prop from which the page data is populated.</param>
    void PopulateFrom(IProp prop);
}
