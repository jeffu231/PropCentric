namespace Props.Abstractions.Setup;

/// <summary>
/// Marker interface for wizard-owned draft objects that hold user-entered field values during prop setup.
/// </summary>
/// <remarks>
/// A draft is a plain data object — it contains only the fields that appear in the wizard UI.
/// Feature state such as brightness and gamma lives in the feature wizard pages, not in the draft.
/// See the Three-Layer Visual Model Pattern in CLAUDE.md for the full pipeline description.
/// </remarks>
public interface IPropDraft { }
