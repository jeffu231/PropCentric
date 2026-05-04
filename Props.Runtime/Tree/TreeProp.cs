using System.Numerics;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Vixen.Sys.Props;

namespace Props.Runtime.Tree;

[PropDescriptor("BCD3FB69-4827-49EE-B877-BD2AE14E792D", "Tree", typeof(TreePropSetup))]
public class TreeProp : BaseLightProp<TreePropVisualModel>, IHasLights, IHasDimming
{
    
    #region Constructors

    public TreeProp() : this("Tree 1", 0, 0)
    {
        //Set initial to 0 so creation does not trigger element generation.
    }

    public TreeProp(string name, int strings, int nodesPerString) : this(name, strings, nodesPerString, StringTypes.ColorMixingRGB)
    {
    }

    public TreeProp(string name, int strings = 0, int nodesPerString = 0, StringTypes stringType = StringTypes.ColorMixingRGB) : base(name)
    {			
        Name = name;
        StringType = stringType;
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
	    throw new NotImplementedException();
    }

   protected override IPropVisualModel BuildVisualModel()
    {
        var points = GeneratePlaceholderPoints();
        return new TreePropVisualModel
        {
            ReferencePoint = points[0].Position,
            Elements = [new LightPointCloud { Points = points, PointSize = 2f }]
        };
    }

    public override string GetSummary()
    {
	    return "TODO";
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