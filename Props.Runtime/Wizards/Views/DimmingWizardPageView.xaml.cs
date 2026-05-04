using System.ComponentModel;
using System.Windows;
using Props.Runtime.Wizards.ViewModels;

namespace Props.Runtime.Wizards.Views
{
	public partial class DimmingWizardPageView : INotifyPropertyChanged
	{
		public DimmingWizardPageView()
		{
			InitializeComponent();
		}

		//TODO this should be a command in MVVM not an event
		private void AdvancedButton_Click(object sender, RoutedEventArgs e)
		{
			if (DataContext is DimmingWizardPageViewModel viewModel)
			{
				System.Windows.MessageBox.Show("Advanced option to be implemented at future date.", "Advanced Options", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
	}
}
