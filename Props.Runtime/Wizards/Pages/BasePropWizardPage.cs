using Catel.Data;
using Orc.Wizard;

namespace Props.Runtime.Wizards.Pages
{
	/// <summary>
	/// Maintains base prop wizard page data.
	/// </summary>
	public abstract class BasePropWizardPage : WizardPageBase, IBasePropWizardPage
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		protected BasePropWizardPage()
		{
			
		}

		#endregion

		#region IBasePropWizardPage

		/// <inheritdoc/>		
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		private static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));
		
		

		#endregion
	}
}
