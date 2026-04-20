using System.Numerics;
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
        var points = GeneratePlaceholderPoints();
        return new PropVisualModel
        {
            ReferencePoint = points[0].Position,
            Elements = [new LightPointCloud { Points = points, PointSize = 2f }]
        };
    }

    private static IReadOnlyList<LightPoint> GeneratePlaceholderPoints()
    {
        // Placeholder: 10 points arranged vertically until real ElementNodes are wired
        return Enumerable.Range(0, 10)
            .Select(i => new LightPoint
            {
                Position = new Vector3(0, i * 0.1f, 0),
                ElementId = Guid.NewGuid()
            })
            .ToList();
    }
}