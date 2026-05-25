using Catel.Data;
using Catel.MVVM;
using Props.Abstractions.Props;
using Props.Runtime.Tree.Wizard.Pages;
using Props.Runtime.Wizards.Core.ViewModels;

namespace Props.Runtime.Tree.Wizard.ViewModels;

/// <summary>
/// View model for the tree prop wizard page; binds wizard UI fields to the <see cref="TreePropWizardPage"/> model
/// and wires the preview builder closure.
/// </summary>
public class TreePropWizardPageViewModel : LightWizardPageViewModel<TreePropWizardPage>
{
    public TreePropWizardPageViewModel(TreePropWizardPage wizardPage) : base(wizardPage)
    {
        PreviewBuilder = cancellationToken => wizardPage.Coordinator.BuildPreviewAsync(wizardPage.Draft, cancellationToken);
    }

    #region Strings property

    /// <summary>
    /// Gets or sets the Strings value.
    /// </summary>
    [ViewModelToModel]
    public int Strings
    {
        get => GetValue<int>(NodeCountProperty);
        set => SetValue(NodeCountProperty, value);
    }

    private static readonly IPropertyData NodeCountProperty = RegisterProperty(nameof(Strings), 16);

    #endregion

    #region NodesPerString property

    /// <summary>
    /// Gets or sets the NodesPerString value.
    /// </summary>
    [ViewModelToModel]
    public int NodesPerString
    {
        get => GetValue<int>(NodesPerStringProperty);
        set => SetValue(NodesPerStringProperty, value);
    }

    /// <summary>
    /// NodesPerString property data.
    /// </summary>
    private static readonly IPropertyData NodesPerStringProperty = RegisterProperty(nameof(NodesPerString), 50);

    #endregion
    
    
    #region Public Properties

		[ViewModelToModel]
		public int DegreesCoverage
		{
			get => GetValue<int>(DegreesCoverageProperty);
			set => SetValue(DegreesCoverageProperty, value);
		}

		private static readonly IPropertyData DegreesCoverageProperty = RegisterProperty<int>(nameof(DegreesCoverage));

		[ViewModelToModel]
		public int DegreeOffset
		{
			get => GetValue<int>(DegreeOffsetProperty);
			set => SetValue(DegreeOffsetProperty, value);
		}

		private static readonly IPropertyData DegreeOffsetProperty = RegisterProperty<int>(nameof(DegreeOffset));

		[ViewModelToModel]
		public int BaseHeight
		{
			get => GetValue<int>(BaseHeightProperty);
			set => SetValue(BaseHeightProperty, value);
		}

		private static readonly IPropertyData BaseHeightProperty = RegisterProperty<int>(nameof(BaseHeight));

		[ViewModelToModel]
		public int TopHeight
		{
			get => GetValue<int>(TopHeightProperty);
			set => SetValue(TopHeightProperty, value);
		}

		private static readonly IPropertyData TopHeightProperty = RegisterProperty<int>(nameof(TopHeight));

		[ViewModelToModel]
		public int TopWidth
		{
			get => GetValue<int>(TopWidthProperty);
			set => SetValue(TopWidthProperty, value);
		}

		private static readonly IPropertyData TopWidthProperty = RegisterProperty<int>(nameof(TopWidth));

		[ViewModelToModel]
		public StartLocation StartLocation
		{
			get => GetValue<StartLocation>(StartLocationProperty);
			set => SetValue(StartLocationProperty, value);
		}

		private static readonly IPropertyData StartLocationProperty = RegisterProperty<StartLocation>(nameof(StartLocation));

		[ViewModelToModel]
		public bool ZigZag
		{
			get => GetValue<bool>(ZigZagProperty);
			set => SetValue(ZigZagProperty, value);
		}

		private static readonly IPropertyData ZigZagProperty = RegisterProperty<bool>(nameof(ZigZag));

		[ViewModelToModel]
		public int ZigZagOffset
		{
			get => GetValue<int>(ZigZagOffsetProperty);
			set => SetValue(ZigZagOffsetProperty, value);
		}

		private static readonly IPropertyData ZigZagOffsetProperty = RegisterProperty<int>(nameof(ZigZagOffset));

		[ViewModelToModel]
		public float TopRadius
		{
			get => GetValue<float>(TopRadiusProperty);
			set => SetValue(TopRadiusProperty, value);
		}

		private static readonly IPropertyData TopRadiusProperty = RegisterProperty<float>(nameof(TopRadius));

		[ViewModelToModel]
		public float BottomRadius
		{
			get => GetValue<float>(BottomRadiusProperty);
			set => SetValue(BottomRadiusProperty, value);
		}

		private static readonly IPropertyData BottomRadiusProperty = RegisterProperty<float>(nameof(BottomRadius));

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
