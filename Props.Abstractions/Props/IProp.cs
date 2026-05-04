using System.Collections.ObjectModel;
using Props.Abstractions.Props.Components;
using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Props;

public interface IProp
{
   /// <summary>
		/// Unique id of the Prop
		/// </summary>
		Guid Id { get; init; }

		/// <summary>
		/// Friendly name of the Prop
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets or sets the name of the user who created the prop.
		/// </summary>
		/// <remarks>
		/// This property is used to track the creator of the prop for auditing or informational purposes.
		/// </remarks>
		string CreatedBy { get; set; }

		/// <summary>
		/// Gets the date and time when the property was created.
		/// </summary>
		/// <remarks>
		/// This property is initialized at the time of the property's creation and cannot be modified afterward.
		/// </remarks>
		DateTime CreationDate { get; }

		/// <summary>
		/// Gets or sets the date and time when the property was last modified.
		/// </summary>
		/// <remarks>
		/// This property is updated whenever changes are made to the property.
		/// </remarks>
		DateTime ModifiedDate { get; set; }

		/// <summary>
		/// Model for rendering the visual
		/// </summary>
		IPropVisualModel PropVisualModel { get; }

		public abstract string GetSummary();


		/// <summary>
		/// Gets the collection of components associated with the prop.
		/// </summary>
		/// <remarks>
		/// Each component in the collection represents a distinct part of the prop, 
		/// allowing for modular and flexible configuration of the prop's structure and behavior.
		/// These are defined internally by the Prop.
		/// </remarks>
		ObservableCollection<IPropComponent> PropComponents { get; }

		/// <summary>
		/// Gets the collection of user-defined components associated with the prop.
		/// </summary>
		/// <remarks>
		/// User-defined components are custom components created by the user, 
		/// which can be added to the prop to extend its functionality or representation.
		/// </remarks>
		ObservableCollection<IPropComponent> UserDefinedPropComponents { get; }

		/// <summary>
		/// Should be called when the IProp is being removed so it can clean up any related items
		/// </summary>
		void CleanUp();
    IPropVisualModel VisualModel { get; }
    event EventHandler? VisualModelChanged;
}