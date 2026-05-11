using Catel.IoC;
using Orc.Wizard;
using Props.Runtime.PolyLine.Wizard.Pages;
using Props.Runtime.Wizards.Core;
using System.Diagnostics;

namespace Props.Runtime.PolyLine.Wizard;

/// <summary>
/// Wizard shell for polyline prop create/edit flows.
/// </summary>
public sealed class PolyLinePropWizard : PropWizardBase
{
    private const string HelpUrl =
        "https://www.vixenlights.com/docs/usage/preview/smart-objects/";

    public PolyLinePropWizard(ITypeFactory typeFactory, PolyLinePropWizardPage page) : base(typeFactory)
    {
        Title = "PolyLine Prop";
        ShowInTaskbar = true;

        this.AddPage(page);
    }

    public override Task ShowHelpAsync()
    {
        return Task.FromResult(Process.Start(new ProcessStartInfo(HelpUrl) { UseShellExecute = true }));
    }
}
