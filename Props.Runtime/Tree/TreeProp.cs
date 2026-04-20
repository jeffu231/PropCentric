using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;

namespace Props.Runtime.Tree;

[PropDescriptor("BCD3FB69-4827-49EE-B877-BD2AE14E792D", "Tree", typeof(TreePropSetupWizard))]
public class TreeProp : Prop, IHasLights, IHasDimming
{
    private double _brightness = 0.5;
    public double Brightness
    {
        get => _brightness;
        set { _brightness = value; InvalidateVisualModel(); }
    }

    protected override IPropVisualModel BuildVisualModel()
    {
        return new PropVisualModel
        {
            Elements = [new PointCloud { Size = 2f }]
        };
    }
}