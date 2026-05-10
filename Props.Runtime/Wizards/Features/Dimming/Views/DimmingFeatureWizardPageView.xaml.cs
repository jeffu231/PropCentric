using System.ComponentModel;
using System.Windows;
using Props.Runtime.Wizards.Features.Dimming.ViewModels;

namespace Props.Runtime.Wizards.Features.Dimming.Views
{
	/// <summary>
	/// Code-behind for the dimming wizard page view.
	/// </summary>
	public partial class DimmingFeatureWizardPageView : INotifyPropertyChanged
	{
		/// <summary>Initializes a new instance of the <see cref="DimmingFeatureWizardPageView"/> class.</summary>
		public DimmingFeatureWizardPageView()
		{
			InitializeComponent();
		}

		//TODO this should be a command in MVVM not an event
		private void AdvancedButton_Click(object sender, RoutedEventArgs e)
		{
			if (DataContext is DimmingFeatureWizardPageViewModel viewModel)
			{
				System.Windows.MessageBox.Show("Advanced option to be implemented at future date.", "Advanced Options", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
	}
}
