namespace Props.Abstractions.Features;

/// <summary>
/// Marks a prop as tracking a discrete operational state.
/// </summary>
[PropFeature(PropFeatureFlags.State)]
public interface IHasState
{
}