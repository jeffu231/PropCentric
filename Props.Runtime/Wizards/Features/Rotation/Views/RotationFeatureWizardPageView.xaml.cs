using Props.Runtime.Wizards.Core.Views;

namespace Props.Runtime.Wizards.Features.Rotation.Views;

/// <summary>
/// Code-behind for the rotation feature wizard page view.
/// </summary>
public partial class RotationFeatureWizardPageView : WizardPageViewBase
{
    public RotationFeatureWizardPageView()
    {
        InitializeComponent();

        OpenTkCntrl = OpenTkControl;
        Initialize();
    }
}
