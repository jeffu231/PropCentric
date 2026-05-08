using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Props.Runtime.Wizards.Pages;

namespace Props.Runtime.Wizards.ViewModels
{
	/// <summary>
	/// View model for the <see cref="DimmingWizardPage"/> that exposes brightness and gamma for data binding.
	/// </summary>
	public class DimmingWizardPageViewModel : WizardPageViewModelBase<DimmingWizardPage>
	{
		/// <summary>Initializes a new instance of the <see cref="DimmingWizardPageViewModel"/> class.</summary>
		/// <param name="wizardPage">The dimming wizard page that backs this view model.</param>
		public DimmingWizardPageViewModel(DimmingWizardPage wizardPage) : base(wizardPage)
		{
		}

		/// <summary>Gets or sets the maximum brightness level as an integer percentage (0–100).</summary>
		[ViewModelToModel]
		public int Brightness
		{
			get { return GetValue<int>(BrightnessProperty); }
			set { SetValue(BrightnessProperty, value); }
		}
		private static readonly IPropertyData BrightnessProperty = RegisterProperty<int>(nameof(Brightness));

		/// <summary>Gets or sets the gamma correction factor.</summary>
		[ViewModelToModel]
		public double Gamma
		{
			get { return GetValue<double>(GammaProperty); }
			set { SetValue(GammaProperty, value); }
		}
		private static readonly IPropertyData GammaProperty = RegisterProperty<double>(nameof(Gamma));
		
	}
}