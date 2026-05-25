namespace Props.Abstractions.Setup.Drafts;

/// <summary>
/// Exposes mutable draft dimming settings for wizard flows that edit brightness and gamma.
/// </summary>
public interface IHasDimmingSettingsDraft
{
    /// <summary>
    /// Gets or sets the maximum brightness level for the current wizard flow.
    /// </summary>
    /// <value>A percentage value between 0 and 100 inclusive.</value>
    double Brightness { get; set; }

    /// <summary>
    /// Gets or sets the gamma correction factor for the current wizard flow.
    /// </summary>
    /// <value>A positive non-zero value where <c>1.0</c> represents no correction.</value>
    double Gamma { get; set; }
}
