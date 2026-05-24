using Common.WPFCommon.Converters;
using Orc.Wizard;
using Props.Abstractions.Props;
using Props.Abstractions.Visuals;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Wizards.Core.Pages;

namespace Props.Runtime.Tree.Wizard.Pages
{
    /// <summary>
    /// Maintains a tree wizard page. Properties delegate to the shared draft so the
    /// preview coordinator always reads current values without a separate sync step.
    /// </summary>
    public class TreePropWizardPage : LightPropWizardPage<TreePropDraft>
    {
        #region Constructor

        public TreePropWizardPage(TreePropDraft draft, IWizardPreviewCoordinator<TreePropDraft> coordinator)
            : base(draft)
        {
            Coordinator = coordinator;

            Title = "Basic Attributes";
            Description = "Enter attributes for Tree";
        }

        #endregion

        #region Properties

        public IWizardPreviewCoordinator<TreePropDraft> Coordinator { get; }

        public int Strings
        {
            get => Draft.Strings;
            set { Draft.Strings = value; RaisePropertyChanged(nameof(Strings)); }
        }

        public int NodesPerString
        {
            get => Draft.NodesPerString;
            set { Draft.NodesPerString = value; RaisePropertyChanged(nameof(NodesPerString)); }
        }

        public int DegreesCoverage
        {
            get => Draft.DegreesCoverage;
            set { Draft.DegreesCoverage = value; RaisePropertyChanged(nameof(DegreesCoverage)); }
        }

        public int DegreeOffset
        {
            get => Draft.DegreeOffset;
            set { Draft.DegreeOffset = value; RaisePropertyChanged(nameof(DegreeOffset)); }
        }

        public int BaseHeight
        {
            get => Draft.BaseHeight;
            set { Draft.BaseHeight = value; RaisePropertyChanged(nameof(BaseHeight)); }
        }

        public int TopHeight
        {
            get => Draft.TopHeight;
            set { Draft.TopHeight = value; RaisePropertyChanged(nameof(TopHeight)); }
        }

        public int TopWidth
        {
            get => Draft.TopWidth;
            set { Draft.TopWidth = value; RaisePropertyChanged(nameof(TopWidth)); }
        }

        public StartLocation StartLocation
        {
            get => Draft.StartLocation;
            set { Draft.StartLocation = value; RaisePropertyChanged(nameof(StartLocation)); }
        }

        public bool ZigZag
        {
            get => Draft.ZigZag;
            set { Draft.ZigZag = value; RaisePropertyChanged(nameof(ZigZag)); }
        }

        public int ZigZagOffset
        {
            get => Draft.ZigZagOffset;
            set { Draft.ZigZagOffset = value; RaisePropertyChanged(nameof(ZigZagOffset)); }
        }

        public float TopRadius
        {
            get => Draft.TopRadius;
            set { Draft.TopRadius = value; RaisePropertyChanged(nameof(TopRadius)); }
        }

        public float BottomRadius
        {
            get => Draft.BottomRadius;
            set { Draft.BottomRadius = value; RaisePropertyChanged(nameof(BottomRadius)); }
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
                          $"Nodes Per String: {NodesPerString}\n" +
                          $"Light Size: {LightSize}\n" +
                          $"Degree Offset: {DegreeOffset}\n" +
                          $"Degrees Coverage: {DegreesCoverage}\n" +
                          $"Start Location: {EnumValueTypeConverter.GetDescription(StartLocation)}\n" +
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
    }
}
