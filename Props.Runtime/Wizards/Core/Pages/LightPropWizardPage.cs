using Catel.Data;
using Props.Abstractions.Setup.Drafts;

namespace Props.Runtime.Wizards.Core.Pages
{
	/// <summary>
	/// Maintains base Light Prop Wizard page data.
	/// </summary>
	public abstract class LightPropWizardPage<TDraft> : PropWizardPageBase<TDraft>, ILightPropWizardPage
		where TDraft : class, IHasLightSettingsDraft
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		protected LightPropWizardPage(TDraft draft)
			: base(draft)
		{
			LightSizeMinimum = 1;
			LightSizeMaximum = 50;
		}

		#endregion

		#region ILightPropWizardPage

		/// <summary>
		/// Gets or sets the size of each light.
		/// </summary>
		/// <remarks>
		/// The size of the lights is constrained by <see cref="LightSizeMinimum"/> and <see cref="LightSizeMaximum"/>, consequently
		/// set these values prior to setting LightSize.
		/// </remarks>
		public int LightSize
		{
			get { return Draft.LightSize; }
			set
			{
				var clampedValue = Math.Clamp(value, LightSizeMinimum, LightSizeMaximum);
				if (Draft.LightSize == clampedValue)
				{
					return;
				}

				Draft.LightSize = clampedValue;
				RaisePropertyChanged(nameof(LightSize));
			}
		}

		#endregion

		#region Protected Properties

		protected int LightSizeMinimum
		{
			get { return GetValue<int>(LightSizeMinimumProperty); }
			set
			{
				if (LightSize < value)
				{
					LightSize = value;
				}
				SetValue(LightSizeMinimumProperty, value);
			}
		}

		private static readonly IPropertyData LightSizeMinimumProperty = RegisterProperty<int>(nameof(LightSizeMinimum));

		protected int LightSizeMaximum
		{
			get { return GetValue<int>(LightSizeMaximumProperty); }
			set
			{
				if (LightSize > value)
				{
					LightSize = value;
				}
				SetValue(LightSizeMaximumProperty, value);
			}
		}

		private static readonly IPropertyData LightSizeMaximumProperty = RegisterProperty<int>(nameof(LightSizeMaximum));
		#endregion
	}
}
