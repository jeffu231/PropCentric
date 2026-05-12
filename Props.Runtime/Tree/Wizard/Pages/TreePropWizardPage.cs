using System.ComponentModel;
using Orc.Wizard;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Wizards.Core.Pages;

namespace Props.Runtime.Tree.Wizard.Pages
{
    /// <summary>
    /// Maintains a tree wizard page. Properties delegate to the shared draft so the
    /// preview coordinator always reads current values without a separate sync step.
    /// </summary>
    public class TreePropWizardPage : LightPropWizardPage
    {
        private readonly TreePropDraft _draft;

        #region Constructor

        public TreePropWizardPage(TreePropDraft draft, IWizardPreviewCoordinator<TreePropDraft> coordinator)
        {
            _draft = draft;
            Coordinator = coordinator;

            Title = "Basic Attributes";
            Description = "Enter attributes for Tree";

            // Keep parent-class Catel-stored properties (Name, LightSize) in sync with the draft.
            PropertyChanged += OnParentPropertyChanged;
        }

        #endregion

        #region Properties

        public IWizardPreviewCoordinator<TreePropDraft> Coordinator { get; }

        public TreePropDraft Draft => _draft;

        public int Strings
        {
            get => _draft.Strings;
            set { _draft.Strings = value; RaisePropertyChanged(nameof(Strings)); }
        }

        public int NodesPerString
        {
            get => _draft.NodesPerString;
            set { _draft.NodesPerString = value; RaisePropertyChanged(nameof(NodesPerString)); }
        }

        public int DegreesCoverage
        {
            get => _draft.DegreesCoverage;
            set { _draft.DegreesCoverage = value; RaisePropertyChanged(nameof(DegreesCoverage)); }
        }

        public int DegreeOffset
        {
            get => _draft.DegreeOffset;
            set { _draft.DegreeOffset = value; RaisePropertyChanged(nameof(DegreeOffset)); }
        }

        public int BaseHeight
        {
            get => _draft.BaseHeight;
            set { _draft.BaseHeight = value; RaisePropertyChanged(nameof(BaseHeight)); }
        }

        public int TopHeight
        {
            get => _draft.TopHeight;
            set { _draft.TopHeight = value; RaisePropertyChanged(nameof(TopHeight)); }
        }

        public int TopWidth
        {
            get => _draft.TopWidth;
            set { _draft.TopWidth = value; RaisePropertyChanged(nameof(TopWidth)); }
        }

        public StartLocation StartLocation
        {
            get => _draft.StartLocation;
            set { _draft.StartLocation = value; RaisePropertyChanged(nameof(StartLocation)); }
        }

        public bool ZigZag
        {
            get => _draft.ZigZag;
            set { _draft.ZigZag = value; RaisePropertyChanged(nameof(ZigZag)); }
        }

        public int ZigZagOffset
        {
            get => _draft.ZigZagOffset;
            set { _draft.ZigZagOffset = value; RaisePropertyChanged(nameof(ZigZagOffset)); }
        }

        public float TopRadius
        {
            get => _draft.TopRadius;
            set { _draft.TopRadius = value; RaisePropertyChanged(nameof(TopRadius)); }
        }

        public float BottomRadius
        {
            get => _draft.BottomRadius;
            set { _draft.BottomRadius = value; RaisePropertyChanged(nameof(BottomRadius)); }
        }
        
        //Placeholders for real rotation properties
        public int XRotation
        {
            get
            {
                var axis = _draft.AxisRotations.First(x => x.Axis == Axis.XAxis);
                return axis.RotationAngle;
            }
            set
            {
                var axis = _draft.AxisRotations.First(x => x.Axis == Axis.XAxis);
                axis.RotationAngle = value;
                RaisePropertyChanged(nameof(XRotation));
            }
        }
        
        public int YRotation
        {
            get
            {
                var axis = _draft.AxisRotations.First(x => x.Axis == Axis.YAxis);
                return axis.RotationAngle;
            }
            set
            {
                var axis = _draft.AxisRotations.First(x => x.Axis == Axis.YAxis);
                axis.RotationAngle = value;
                RaisePropertyChanged(nameof(YRotation));
            }
        }
        
        public int ZRotation
        {
            get
            {
                var axis = _draft.AxisRotations.First(x => x.Axis == Axis.ZAxis);
                return axis.RotationAngle;
            }
            set
            {
                var axis = _draft.AxisRotations.First(x => x.Axis == Axis.ZAxis);
                axis.RotationAngle = value;
                RaisePropertyChanged(nameof(ZRotation));
            }
        }

        #endregion

        #region Public Methods

        public override ISummaryItem GetSummary()
        {
            return new SummaryItem
            {
                Title = this.Title,
                Summary = $"Prop Type: Tree\n" +
                          $"Name: {Name}\n" +
                          $"Strings: {Strings}\n" +
                          $"Light Size: {LightSize}\n" +
                          $"Nodes Per String: {NodesPerString}\n" +
                          $"Degree Offset: {DegreeOffset}\n" +
                          $"Degrees Coverage: {DegreesCoverage}\n" +
                          $"Base Height: {BaseHeight}\n" +
                          $"Top Height: {TopHeight}\n" +
                          $"Top Width: {TopWidth}\n" +
                          $"ZigZag: {ZigZag}\n" +
                          $"ZigZag Offset: {ZigZagOffset}\n" +
                          $"Top Radius: {TopRadius}\n" +
                          $"Bottom Radius: {BottomRadius}\n"
            };
        }

        #endregion

        #region Private Methods

        // Keeps parent-class Catel-stored Name and LightSize mirrored to the draft
        // so the preview coordinator always has current values.
        private void OnParentPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Name): _draft.Name = Name; break;
                case nameof(LightSize): _draft.LightSize = LightSize; break;
                case nameof(NodesPerString): _draft.NodesPerString = NodesPerString; break;
                case nameof(DegreesCoverage): _draft.DegreesCoverage = DegreesCoverage; break;
                case nameof(BaseHeight): _draft.BaseHeight = BaseHeight; break;
                case nameof(TopHeight): _draft.TopHeight = TopHeight; break;
                case nameof(TopWidth): _draft.TopWidth = TopWidth; break;
                case nameof(BottomRadius): _draft.BottomRadius = BottomRadius; break;
            }
        }

        #endregion
    }
}
