using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Setup;

namespace Props.Runtime.Tree.Setup;

/// <summary>
/// Copies field values between a <see cref="TreePropDraft"/> and a <see cref="TreeProp"/>.
/// </summary>
public sealed class TreePropDraftMapper : IPropDraftMapper<TreePropDraft, TreeProp>
{
    public void PopulateDraft(TreePropDraft draft, TreeProp prop)
    {
        draft.Name = prop.Name;
        draft.Strings = prop.Strings;
        draft.NodesPerString = prop.NodesPerString;
        draft.LightSize = prop.LightSize;
        draft.ColorConfiguration = prop.ColorConfiguration.DeepClone();
        draft.DegreesCoverage = prop.DegreesCoverage;
        draft.DegreeOffset = prop.DegreeOffset;
        draft.BaseHeight = prop.BaseHeight;
        draft.TopHeight = prop.TopHeight;
        draft.TopWidth = prop.TopWidth;
        draft.StartLocation = prop.StartLocation;
        draft.ZigZag = prop.ZigZag;
        draft.ZigZagOffset = prop.ZigZagOffset;
        draft.TopRadius = prop.TopRadius;
        draft.BottomRadius = prop.BottomRadius;
        draft.AxisRotations = AxisRotationCollectionFactory.Clone(prop.AxisRotations);
    }

    public void ApplyDraft(TreePropDraft draft, TreeProp prop)
    {
        prop.Name = draft.Name;
        prop.Strings = draft.Strings;
        prop.NodesPerString = draft.NodesPerString;
        prop.LightSize = draft.LightSize;
        prop.ColorConfiguration = draft.ColorConfiguration.DeepClone();
        prop.DegreesCoverage = draft.DegreesCoverage;
        prop.DegreeOffset = draft.DegreeOffset;
        prop.BaseHeight = draft.BaseHeight;
        prop.TopHeight = draft.TopHeight;
        prop.TopWidth = draft.TopWidth;
        prop.StartLocation = draft.StartLocation;
        prop.ZigZag = draft.ZigZag;
        prop.ZigZagOffset = draft.ZigZagOffset;
        prop.TopRadius = draft.TopRadius;
        prop.BottomRadius = draft.BottomRadius;
        prop.AxisRotations = AxisRotationCollectionFactory.Clone(draft.AxisRotations);
    }
}
