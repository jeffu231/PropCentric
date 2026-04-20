using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;

namespace Props.Runtime.Tree;

[PropDescriptor("BCD3FB69-4827-49EE-B877-BD2AE14E792D", "Tree", typeof(TreePropSetupWizard),
    typeof(TreePropVisualModel))]
public class TreeProp : Prop, IHasLights, IHasDimming, IPropVisualModelBuilder
{
    //Conceptual builder pattern
    public IPropVisualModel Build()
    {
        return new PropVisualModel
        {
            Elements = new IVisualElement[]
            {
                new PointCloud
                {
                    //Points = GenerateTreePoints(),
                    Size = 2f
                }
            }
            // FeatureData = new Dictionary<Type, IVisualFeatureData>
            // {
            //     [typeof(SegmentedFeatureData)] = new SegmentedFeatureData
            //     {
            //         SegmentOffsets = BuildSegments()
            //     }
            // }
        };
    }
}