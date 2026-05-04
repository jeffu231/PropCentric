using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Props.Runtime.Wizards.Pages;

namespace Props.Runtime.Wizards.ViewModels
{
	public class DimmingWizardPageViewModel : WizardPageViewModelBase<DimmingWizardPage>
	{
		public DimmingWizardPageViewModel(DimmingWizardPage wizardPage) : base(wizardPage)
		{
		}

		[ViewModelToModel]
		public int Brightness
		{
			get { return GetValue<int>(BrightnessProperty); }
			set { SetValue(BrightnessProperty, value); }
		}
		private static readonly IPropertyData BrightnessProperty = RegisterProperty<int>(nameof(Brightness));

		[ViewModelToModel]
		public double Gamma
		{
			get { return GetValue<double>(GammaProperty); }
			set { SetValue(GammaProperty, value); }
		}
		private static readonly IPropertyData GammaProperty = RegisterProperty<double>(nameof(Gamma));
		
	}
}