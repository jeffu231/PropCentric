using Vixen.Sys.Props;

namespace Props.Runtime.Wizards.Core.Pages
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

		/// <summary>
		/// Type of light string used by the prop.
		/// </summary>
		StringTypes StringType { get; set; }
	}
}
