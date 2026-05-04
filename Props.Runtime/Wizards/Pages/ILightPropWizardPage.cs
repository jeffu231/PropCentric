namespace Props.Runtime.Wizards.Pages
{
	/// <summary>
	/// Maintains light prop wizard page data.
	/// </summary>
	public interface ILightPropWizardPage : IPropWizardPageBase
	{
		/// <summary>
		/// Size of the lights.
		/// </summary>
		int LightSize { get; set; }
	}
}
