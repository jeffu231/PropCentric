using Props.Runtime.Wizards.Core.Views;

namespace Props.Runtime.PolyLine.Wizard.Views;

/// <summary>
/// View for the polyline prop wizard page.
/// </summary>
public partial class PolyLinePropWizardPageView : WizardPageViewBase
{
    public PolyLinePropWizardPageView()
    {
        InitializeComponent();

        OpenTkCntrl = OpenTkControl;
        Initialize();
    }
}
