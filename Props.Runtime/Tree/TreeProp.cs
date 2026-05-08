using System.ComponentModel;
using System.Drawing;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree.Visuals;
using Vixen.Common.WPFCommon.Converters;
using Vixen.Controls.Theme;

namespace Props.Runtime.Tree;

/// <summary>
/// A conical pixel-tree prop consisting of individually addressable light strings arranged radially.
/// </summary>
[PropDescriptor("BCD3FB69-4827-49EE-B877-BD2AE14E792D", "Tree", typeof(TreePropSetup))]
public class TreeProp : BaseLightProp<TreePropVisualModel>, IHasLights
{
    private readonly IVisualInputMapper<TreeProp, TreeVisualInput> _inputMapper;
    private readonly IPropVisualModelFactory<TreeVisualInput> _factory;

    #region Constructors

    public TreeProp(
        IVisualInputMapper<TreeProp, TreeVisualInput> inputMapper,
        IPropVisualModelFactory<TreeVisualInput> factory) : base("Tree 1")
    {
        _inputMapper = inputMapper;
        _factory = factory;
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

		/// <summary>Gets or sets the number of light strings on the tree.</summary>
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

		/// <summary>Gets or sets the number of individually addressable nodes per string.</summary>
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

		/// <summary>Gets or sets the arc of the tree covered by strings, in degrees (1–360).</summary>
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

		/// <summary>Gets or sets the rotational offset of string 1 from the default position, in degrees.</summary>
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

	
		/// <summary>Gets or sets the visual height of the tree base as a percentage.</summary>
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

		/// <summary>Gets or sets the visual height of the tree top as a percentage.</summary>
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

		/// <summary>Gets or sets the visual width of the tree top as a percentage.</summary>
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

		/// <summary>Gets or sets the corner from which element patching begins.</summary>
		public StartLocation StartLocation
		{
			get => _startLocation;
			set => SetProperty(ref _startLocation, value);
		}

		private bool _zigZag;

		/// <summary>Gets or sets a value that indicates whether the patching order alternates direction between strings.</summary>
		/// <value><see langword="true"/> if zig-zag patching is enabled; otherwise, <see langword="false"/>.</value>
		public bool ZigZag
		{
			get => _zigZag;
			set => SetProperty(ref _zigZag, value);
		}

		private int _zigZagOffset;

		/// <summary>Gets or sets the number of elements per string before the zig-zag direction reverses.</summary>
		public int ZigZagOffset
		{
			get => _zigZagOffset;
			set
			{
				if (value <= 0) return;
				SetProperty(ref _zigZagOffset, value);
			}
		}

		/// <summary>Gets or sets the radius at the top of the tree as a percentage of the maximum width.</summary>
		public float TopRadius
		{
			get => field;
			set
			{
				field = value;
				OnPropertyChanged(nameof(TopRadius));
			}
		}

		/// <summary>Gets or sets the radius at the base of the tree as a percentage of the maximum width.</summary>
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
	    return Task.CompletedTask;
    }

    protected override IPropVisualModel BuildVisualModel()
    {
        var input = _inputMapper.Map(this);
        return _factory.Create(input);
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

}