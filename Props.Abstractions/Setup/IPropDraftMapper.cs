using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

/// <summary>
/// Copies field values between a prop draft and a prop instance.
/// </summary>
/// <typeparam name="TDraft">The draft type that holds wizard UI state.</typeparam>
/// <typeparam name="TProp">The concrete prop type whose fields are mirrored in the draft.</typeparam>
/// <remarks>
/// Implementations are pure field copiers with no side effects.
/// Call <see cref="PopulateDraft"/> before opening the wizard and <see cref="ApplyDraft"/> after
/// the user confirms. After <see cref="ApplyDraft"/>, call <c>await prop.CommitAsync()</c> to
/// finalize element-node generation.
/// </remarks>
public interface IPropDraftMapper<in TDraft, in TProp>
    where TDraft : class, IPropDraft
    where TProp : class, IProp
{
    /// <summary>Copies the prop's current values into the draft before the wizard opens.</summary>
    /// <param name="draft">The draft to populate.</param>
    /// <param name="prop">The prop whose values are the source.</param>
    void PopulateDraft(TDraft draft, TProp prop);

    /// <summary>Writes the draft's values back to the prop after the user confirms.</summary>
    /// <param name="draft">The draft containing the user-entered values.</param>
    /// <param name="prop">The prop to update.</param>
    void ApplyDraft(TDraft draft, TProp prop);
}
