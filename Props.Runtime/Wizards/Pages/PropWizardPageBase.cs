using Catel.Data;
using Orc.Wizard;

namespace Props.Runtime.Wizards.Pages
{
	public abstract class PropWizardPageBase : WizardPageBase, IPropWizardPageBase
	{
		#region Constructor

		protected PropWizardPageBase()
		{

		}

		#endregion

		#region IPropWizardPageBase

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
