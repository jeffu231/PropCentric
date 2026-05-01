using System.Windows;
using Orc.Theming;

namespace PropCentric;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Console.WriteLine("Startup Initializing");
        try
        {
            ThemeManager.Current.SynchronizeTheme();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}