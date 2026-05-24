using System.Drawing;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Visuals;
using Vixen.Controls.Theme;

namespace Props.Runtime.PolyLine;

/// <summary>
/// A PolyLine Prop that consists of one or more line segments.
/// </summary>
/// <param name="inputMapper"></param>
/// <param name="builder"></param>
[PropDescriptor("9C4E33A3-C6F4-443F-8B31-610FE3F50805", "PolyLine", typeof(PolyLinePropSetup))]
public class PolyLineProp(
    IVisualInputMapper<PolyLineProp, PolyLineVisualInput> inputMapper,
    IPropVisualModelBuilder<PolyLineVisualInput, PolyLinePropVisualModel> builder)
    : BaseLightProp<PolyLinePropVisualModel>("PolyLine 1"), IHasLights, IHasColor, IHasSegments
{
   #region Properties

    public IReadOnlyList<Segment> Segments { get; private set; } = new List<Segment>();

    #endregion

    public void ReplaceSegments(IReadOnlyList<Segment> segments)
    {
        Segments = segments.ToArray();
    }

    protected override Task<PolyLinePropVisualModel> BuildVisualModelAsync()
    {
        var input = inputMapper.Map(this);
        return Task.FromResult(builder.Create(input));
    }

    public override string GetSummary()
    {
        var count = 0;
        
        foreach (var segment in Segments)
        {
            count += segment.PointCount;
        }
        
        string summary =
            "<style>" +
            $"  h2   {{color: #{ColorTranslator.ToHtml(ThemeColorTable.ForeColor)}; margin-top: 0; margin-bottom: 0; text-decoration: underline;}}" +
            $"  body {{color: #{ColorTranslator.ToHtml(ThemeColorTable.ForeColor)}; margin-top: 0;}}" +
            $"  b    {{color: #{ColorTranslator.ToHtml(ThemeColorTable.ForeColorDisabled)}; margin-left: 20px;}}" +
            "</style>" +
            $"<h2>Basic Attributes</h2>" +
            "<body>" +
            $"<b>Prop Type:</b> PolyLine<br>" +
            $"<b>Name:</b> {Name}<br>" +
            $"<b>Segments:</b> {Segments.Count}<br>" +
            $"<b>Total points:</b> {count}<br>" +
            "</body>" +
            "<h2>Additional Props</h2>" +
            "<body>" +
            //				$"<b>Left and Right Tree:</b> {LeftRight}<br>" +
            "</body>" +
            GetDimmingSummary() +
            GetColorSummary() +
            GetBaseSummary();

        return summary;
    }

    protected override Task GenerateElementsAsync()
    {
        //TODO pass this off to a common adapter or service that is outside this POC scope.
        return Task.CompletedTask;
    }
}
