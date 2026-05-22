using Catel.Data;
using Orc.Wizard;

namespace Props.Runtime.Wizards.Core.Pages
{
	public abstract class PropWizardPageBase : WizardPageBase, IPropWizardPageBase
	{
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
