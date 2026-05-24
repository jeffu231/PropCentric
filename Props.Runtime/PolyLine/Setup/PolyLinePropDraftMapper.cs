using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Abstractions.Setup.Drafts;

namespace Props.Runtime.PolyLine.Setup;

public class PolyLinePropDraftMapper : IPropDraftMapper<PolyLinePropDraft, PolyLineProp>
{
    public void PopulateDraft(PolyLinePropDraft draft, PolyLineProp prop)
    {
        draft.Name = prop.Name;
        draft.LightSize = prop.LightSize;
        draft.ColorConfiguration = prop.ColorConfiguration.DeepClone();
        draft.Segments = new ObservableCollection<SegmentDraftState>(
            prop.Segments.Select(x =>
                new SegmentDraftState { Start = x.Start, End = x.End, PointCount = x.PointCount }));
    }

    public void ApplyDraft(PolyLinePropDraft draft, PolyLineProp prop)
    {
        prop.Name = draft.Name;
        prop.LightSize = draft.LightSize;
        prop.ColorConfiguration = draft.ColorConfiguration.DeepClone();
        prop.ReplaceSegments(draft.Segments.Select(x =>
            new Segment(x.Start, x.End, x.PointCount)).ToImmutableList());
    }
}
