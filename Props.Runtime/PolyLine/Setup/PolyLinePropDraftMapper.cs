using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup;

namespace Props.Runtime.PolyLine.Setup;

public class PolyLinePropDraftMapper : IPropDraftMapper<PolyLinePropDraft, PolyLineProp>
{
    public void PopulateDraft(PolyLinePropDraft draft, PolyLineProp prop)
    {
        draft.Name = prop.Name;
        draft.LightSize = prop.LightSize;
        draft.AxisRotations = new ObservableCollection<AxisRotationModel>(prop.AxisRotations);
        draft.Segments = new ObservableCollection<SegmentDraftState>(
            prop.Segments.Select(x =>
                new SegmentDraftState { Start = x.Start, End = x.End, PointCount = x.PointCount }));
    }

    public void ApplyDraft(PolyLinePropDraft draft, PolyLineProp prop)
    {
        prop.Name = draft.Name;
        prop.LightSize = draft.LightSize;
        prop.AxisRotations = new ObservableCollection<AxisRotationModel>(draft.AxisRotations);
        prop.ReplaceSegments(draft.Segments.Select(x =>
            new Segment(x.Start, x.End, x.PointCount)).ToImmutableList());
    }
}
