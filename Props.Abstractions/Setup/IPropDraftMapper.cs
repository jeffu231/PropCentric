using Props.Abstractions.Props;

namespace Props.Abstractions.Setup;

public interface IPropDraftMapper<in TDraft, in TProp>
    where TDraft : class, IPropDraft
    where TProp : class, IProp
{
    void PopulateDraft(TDraft draft, TProp prop);
    void ApplyDraft(TDraft draft, TProp prop);
}
