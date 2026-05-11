using Props.Runtime.Wizards.Core.Views;

namespace Props.Runtime.Wizards.Features.Segments.Views;

/// <summary>
/// Code-behind for the segments feature wizard page view.
/// </summary>
public partial class SegmentsFeatureWizardPageView : WizardPageViewBase
{
    public SegmentsFeatureWizardPageView()
    {
        InitializeComponent();

        OpenTkCntrl = OpenTkControl;
        Initialize();
    }
}
