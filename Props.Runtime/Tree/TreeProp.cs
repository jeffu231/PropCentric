using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Vixen.Common.WPFCommon.Converters;
using Vixen.Controls.Theme;

namespace Props.Runtime.Tree;

[PropDescriptor("BCD3FB69-4827-49EE-B877-BD2AE14E792D", "Tree", typeof(TreePropSetup))]
public class TreeProp : BaseLightProp<TreePropVisualModel>, IHasLights
{
    
    #region Constructors

    public TreeProp() : base("Tree 1")
    {
	    //Sensible defaults
	    ZigZagOffset = 50;
	    StartLocation = StartLocation.BottomLeft;
	    TopWidth = 20;
	    TopHeight = TopWidth / 2;
	    BaseHeight = 40;
	    DegreesCoverage = 360;
	    DegreeOffset = 0;
	    Strings = 16;
	    NodesPerString = 50;
	    LightSize = 2;
	    TopRadius = 10;
	    BottomRadius = 100;	
    }

    #endregion
    
    #region Public Properties

		/// <summary>
		/// The number of light strings
		/// </summary>
		public int Strings
		{
			get => field;
			set
			{
				if (value <= 0) return;
				field = value;
				OnPropertyChanged(nameof(Strings));
			}
		}

		/// <summary>
		/// The number of light nodes per string
		/// </summary>
		public int NodesPerString
		{
			get => field;
			set
			{
				if (value <= 0) return;
				if (value == field)
				{
					return;
				}
				field = value;
				OnPropertyChanged(nameof(NodesPerString));
			}
		}

		/// <summary>
		/// The degrees of coverage for the Tree. ex. 180 for a half tree.
		/// </summary>
		
		public int DegreesCoverage
		{
			get => field;
			set
			{
				if (value > 360 || value <= 0) return;
				if (value == field)
				{
					return;
				}
				field = value;
				OnPropertyChanged(nameof(DegreesCoverage));
			}
		}

		/// <summary>
		/// Offset in the rotation of where string one occurs in degrees.
		/// </summary>
		public int DegreeOffset
		{
			get => field;
			set
			{
				if (value > 359 || value < -359) return;
				if (value == field)
				{
					return;
				}

				field = value;
				OnPropertyChanged(nameof(DegreeOffset));
			}
		}

	
		public int BaseHeight
		{
			get => field;
			set
			{
				if (value <= 0 || value == field)
				{
					return;
				}

				field = value;
				OnPropertyChanged(nameof(BaseHeight));
			}
		}

	
		public int TopHeight
		{
			get => field;
			set
			{
				if (value <= 0 || value == field)
				{
					return;
				}

				field = value;
				OnPropertyChanged(nameof(TopHeight));
			}
		}

		public int TopWidth
		{
			get => field;
			set
			{
				if (value <= 0 || value == field)
				{
					return;
				}

				field = value;
				OnPropertyChanged(nameof(TopWidth));
			}
		}

		private StartLocation _startLocation;
		public StartLocation StartLocation
		{
			get => _startLocation;
			set => SetProperty(ref _startLocation, value);
		}

		private bool _zigZag;
		public bool ZigZag
		{
			get => _zigZag;
			set => SetProperty(ref _zigZag, value);
		}

		private int _zigZagOffset;
		public int ZigZagOffset
		{
			get => _zigZagOffset;
			set
			{
				if (value <= 0) return;
				SetProperty(ref _zigZagOffset, value);
			}
		}

		/// <summary>
		/// Top radius of the tree as a percentage.
		/// </summary>
		public float TopRadius
		{
			get => field;
			set
			{
				field = value;
				OnPropertyChanged(nameof(TopRadius));
			}
		}

		/// <summary>
		/// Bottom radius of the tree as a percentage.
		/// </summary>
		public float BottomRadius
		{
			get => field;
			set
			{
				field = value;
				OnPropertyChanged(nameof(BottomRadius));
			}
		}
		
		//TODO Map element structure to model nodes
					
		#endregion
	

    protected override Task GenerateElementsAsync()
    {
	    //TODO pass this off to a common adapter or service that is outside this POC scope.
	    throw new NotImplementedException();
    }

   protected override IPropVisualModel BuildVisualModel()
    {
	    Console.WriteLine("Building Visual");
        var points = GenerateVisual();
        List<LightPointCloud> cloudPoints = new List<LightPointCloud>();
        foreach (var point in points)
        {
	        cloudPoints.Add(new LightPointCloud { Points = point, PointSize = LightSize });
        }
        return new TreePropVisualModel
        {
            ReferencePoint = points[0][0].Position,
            Elements = cloudPoints
        };
    }

    public override string GetSummary()
    {
	    string summary =
		    "<style>" +
		    $"  h2   {{color: #{ColorTranslator.ToHtml(ThemeColorTable.ForeColor)}; margin-top: 0; margin-bottom: 0; text-decoration: underline;}}" +
		    $"  body {{color: #{ColorTranslator.ToHtml(ThemeColorTable.ForeColor)}; margin-top: 0;}}" +
		    $"  b    {{color: #{ColorTranslator.ToHtml(ThemeColorTable.ForeColorDisabled)}; margin-left: 20px;}}" +
		    "</style>" +
		    $"<h2>Basic Attributes</h2>" +
		    "<body>" +
		    $"<b>Prop Type:</b> Tree<br>" +
		    $"<b>Name:</b> {Name}<br>" +
		    $"<b>Strings:</b> {Strings}<br>" +
		    $"<b>Nodes per String:</b> {NodesPerString}<br>" +
		    $"<b>Light Size:</b> {LightSize}<br>" +
		    $"<b>Degrees Coverage:</b> {DegreesCoverage}<br>" +
		    $"<b>Degree offset:</b> {DegreeOffset}<br>" +
		    $"<b>Base Height:</b> {BaseHeight}<br>" +
		    $"<b>Top Height:</b> {TopHeight}<br>" +
		    $"<b>Top Width:</b> {TopWidth}<br>" +
		    $"<b>Nodes per String:</b> {NodesPerString}<br>" +
		    $"<b>Start Location:</b> {EnumValueTypeConverter.GetDescription(StartLocation)}<br>" +
		    $"<b>ZigZag:</b> {ZigZag}<br>" +
		    $"<b>ZigZag Offset:</b> {ZigZagOffset}<br>" +
		    $"<b>Top Radius:</b> {TopRadius}<br>" +
		    $"<b>Bottom Radius:</b> {BottomRadius}<br>" +
		    $"<b>{AxisRotations[0].Axis} Rotation:</b> {AxisRotations[0].RotationAngle}\u00B0<br>" +
		    $"<b>{AxisRotations[1].Axis} Rotation:</b> {AxisRotations[1].RotationAngle}\u00B0<br>" +
		    $"<b>{AxisRotations[2].Axis} Rotation:</b> {AxisRotations[2].RotationAngle}\u00B0<br>" +
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

    private IReadOnlyList<List<LightPoint>> GenerateVisual()
    {
	    List<List<LightPoint>> points = new List<List<LightPoint>>();
	    for (int i = 0; i < Strings; i++)
	    {
		    points.Add(Enumerable.Range(0, NodesPerString)
			    .Select(i => new LightPoint
			    {
				    Position = new Vector3(0, i * 2, 0),
				    ElementId = Guid.NewGuid()
			    })
			    .ToList()
		    );
	    }

	    return points;
    }
}