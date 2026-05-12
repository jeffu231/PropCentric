using System.Collections.ObjectModel;
using Catel.Data;
using Orc.Wizard;
using Props.Abstractions.PropVisualModels;
using Props.Runtime.ViewModels;

namespace Props.Runtime.Wizards.Core.Pages
{
	public abstract class PropWizardPageBase : WizardPageBase, IPropWizardPageBase
	{
		#region Constructor

		protected PropWizardPageBase()
		{
			// Initialize the Rotation collection
			ObservableCollection<AxisRotationModel> rotations = new ObservableCollection<AxisRotationModel>();
			rotations.Add(new AxisRotationModel() { Axis = Axis.XAxis, RotationAngle = 0 });
			rotations.Add(new AxisRotationModel() { Axis = Axis.YAxis, RotationAngle = 0 });
			rotations.Add(new AxisRotationModel() { Axis = Axis.ZAxis, RotationAngle = 0 });
			Rotations = AxisRotationViewModel.ConvertToViewModel(rotations);
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
		
		/// <inheritdoc/>
		public ObservableCollection<AxisRotationViewModel> Rotations
		{
			get { return GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty); }
			set { SetValue(RotationsProperty, value); }
		}

		private static readonly IPropertyData RotationsProperty = RegisterProperty<ObservableCollection<AxisRotationViewModel>>(nameof(Rotations));

		#endregion
	}
}
