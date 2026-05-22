using System.Collections.ObjectModel;
using Props.Abstractions.PropVisualModels;

namespace Props.Abstractions.Setup.Drafts;

/// <summary>
/// Exposes mutable draft rotation state for wizard pages that edit prop rotations.
/// </summary>
public interface IHasAxisRotationsDraft
{
    /// <summary>
    /// Gets the ordered mutable axis rotations for the current wizard flow.
    /// </summary>
    ObservableCollection<AxisRotationModel> AxisRotations { get; }
}
