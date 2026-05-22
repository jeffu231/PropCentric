using Orc.Wizard;
using Props.Abstractions.Setup.Drafts;

namespace Props.Runtime.Wizards.Core.Pages
{
	public abstract class PropWizardPageBase<TDraft> : WizardPageBase, IPropWizardPageBase
		where TDraft : class, IHasNameDraft
	{
		protected PropWizardPageBase(TDraft draft)
		{
			Draft = draft ?? throw new ArgumentNullException(nameof(draft));
		}

		public TDraft Draft { get; }

		#region IPropWizardPageBase

		/// <inheritdoc/>
		public string Name
		{
			get { return Draft.Name; }
			set
			{
				if (Draft.Name == value)
				{
					return;
				}

				Draft.Name = value;
				RaisePropertyChanged(nameof(Name));
			}
		}

		#endregion
	}
}
