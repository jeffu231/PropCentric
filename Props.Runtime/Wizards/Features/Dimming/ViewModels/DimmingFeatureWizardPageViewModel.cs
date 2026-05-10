using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Props.Runtime.Wizards.Features.Dimming.Pages;

namespace Props.Runtime.Wizards.Features.Dimming.ViewModels
{
	/// <summary>
	/// View model for the <see cref="DimmingFeatureWizardPage"/> that exposes brightness and gamma for data binding.
	/// </summary>
	public class DimmingFeatureWizardPageViewModel : WizardPageViewModelBase<DimmingFeatureWizardPage>
	{
		/// <summary>Initializes a new instance of the <see cref="DimmingFeatureWizardPageViewModel"/> class.</summary>
		/// <param name="featureWizardPage">The dimming wizard page that backs this view model.</param>
		public DimmingFeatureWizardPageViewModel(DimmingFeatureWizardPage featureWizardPage) : base(featureWizardPage)
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