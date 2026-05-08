using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using Props.Abstractions.Props;
using Props.Abstractions.PropVisualModels;
using Props.Runtime.Tree.Wizard.Pages;
using Props.Runtime.ViewModels;
using Props.Runtime.Wizards.ViewModels;

namespace Props.Runtime.Tree.Wizard.ViewModels;

/// <summary>
/// View model for the tree prop wizard page; binds wizard UI fields to the <see cref="TreePropWizardPage"/> model
/// and wires the preview builder closure.
/// </summary>
public class TreePropWizardPageViewModel : LightWizardPageViewModel<TreePropWizardPage, TreePropVisualModel>, IPropWizardPageViewModel
{
    public TreePropWizardPageViewModel(TreePropWizardPage wizardPage) : base(wizardPage)
    {
        PreviewBuilder = () =>
        {
            // Sync rotation state into the draft immediately before each build so the
            // coordinator always reflects the current rotation selections.
            if (Rotations is not null)
                wizardPage.Draft.AxisRotations = AxisRotationViewModel.ConvertToModel(Rotations);

            return wizardPage.Coordinator.BuildPreview(wizardPage.Draft);
        };
    }

    #region Strings property

    /// <summary>
    /// Gets or sets the Strings value.
    /// </summary>
    [ViewModelToModel]
    public int Strings
    {
        get { return GetValue<int>(NodeCountProperty); }
        set { SetValue(NodeCountProperty, value); }
        //RefreshGraphics();
    }
    public static readonly IPropertyData NodeCountProperty = RegisterProperty<int>(nameof(Strings), 16);

    #endregion

    #region NodesPerString property

    /// <summary>
    /// Gets or sets the NodesPerString value.
    /// </summary>
    [ViewModelToModel]
    public int NodesPerString
    {
        get { return GetValue<int>(NodesPerStringProperty); }
        set { SetValue(NodesPerStringProperty, value); }
        //RefreshGraphics();
    }

    /// <summary>
    /// NodesPerString property data.
    /// </summary>
    public static readonly IPropertyData NodesPerStringProperty = RegisterProperty<int>(nameof(NodesPerString), 50);

    #endregion
    
    #region Public Properties

		[ViewModelToModel]
		public int DegreesCoverage
		{
			get { return GetValue<int>(DegreesCoverageProperty); }
			set 
			{
				SetValue(DegreesCoverageProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData DegreesCoverageProperty = RegisterProperty<int>(nameof(DegreesCoverage));

		[ViewModelToModel]
		public int DegreeOffset
		{
			get { return GetValue<int>(DegreeOffsetProperty); }
			set 
			{
				SetValue(DegreeOffsetProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData DegreeOffsetProperty = RegisterProperty<int>(nameof(DegreeOffset));

		[ViewModelToModel]
		public int BaseHeight
		{
			get { return GetValue<int>(BaseHeightProperty); }
			set 
			{
				SetValue(BaseHeightProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData BaseHeightProperty = RegisterProperty<int>(nameof(BaseHeight));

		[ViewModelToModel]
		public int TopHeight
		{
			get { return GetValue<int>(TopHeightProperty); }
			set 
			{ 
				SetValue(TopHeightProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData TopHeightProperty = RegisterProperty<int>(nameof(TopHeight));

		[ViewModelToModel]
		public int TopWidth
		{
			get { return GetValue<int>(TopWidthProperty); }
			set 
			{ 
				SetValue(TopWidthProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData TopWidthProperty = RegisterProperty<int>(nameof(TopWidth));

		[ViewModelToModel]
		public StartLocation StartLocation
		{
			get { return GetValue<StartLocation>(StartLocationProperty); }
			set 
			{ 
				SetValue(StartLocationProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData StartLocationProperty = RegisterProperty<StartLocation>(nameof(StartLocation));

		[ViewModelToModel]
		public bool ZigZag
		{
			get { return GetValue<bool>(ZigZagProperty); }
			set 
			{ 
				SetValue(ZigZagProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData ZigZagProperty = RegisterProperty<bool>(nameof(ZigZag));

		[ViewModelToModel]
		public int ZigZagOffset
		{
			get { return GetValue<int>(ZigZagOffsetProperty); }
			set 
			{ 
				SetValue(ZigZagOffsetProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData ZigZagOffsetProperty = RegisterProperty<int>(nameof(ZigZagOffset));

		[ViewModelToModel]
		public float TopRadius
		{
			get { return GetValue<float>(TopRadiusProperty); }
			set 
			{ 
				SetValue(TopRadiusProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData TopRadiusProperty = RegisterProperty<float>(nameof(TopRadius));

		[ViewModelToModel]
		public float BottomRadius
		{
			get { return GetValue<float>(BottomRadiusProperty); }
			set 
			{ 
				SetValue(BottomRadiusProperty, value);
				//RefreshGraphics();
			}
        }
		public static readonly IPropertyData BottomRadiusProperty = RegisterProperty<float>(nameof(BottomRadius));

		#endregion

    #region Protected Methods

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);

        if (Strings <= 0)
        {
            validationResults.Add(
                FieldValidationResult.CreateError(nameof(Strings), "String Count must be greater than 0"));
        }

        if (NodesPerString <= 0)
        {
            validationResults.Add(FieldValidationResult.CreateError(nameof(NodesPerString),
                "Nodes per string must be greater than 0"));
        }
    }

    #endregion
}
