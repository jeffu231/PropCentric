namespace Props.Abstractions.Setup.Drafts;

/// <summary>
/// Exposes a mutable draft display name for wizard pages that edit prop identity fields.
/// </summary>
public interface IHasNameDraft
{
    /// <summary>Gets or sets the display name for the current wizard flow.</summary>
    string Name { get; set; }
}
