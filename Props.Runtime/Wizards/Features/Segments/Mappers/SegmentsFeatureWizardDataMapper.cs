using System.Collections.ObjectModel;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Runtime.Wizards.Features.Segments.Pages;

namespace Props.Runtime.Wizards.Features.Segments.Mappers;

/// <summary>
/// Transfers segment page state between a <see cref="SegmentsFeatureWizardPage"/> and an <see cref="IHasSegments"/> prop.
/// </summary>
public sealed class SegmentsFeatureWizardDataMapper(SegmentsFeatureWizardPage page) : IFeatureWizardDataMapper
{
    public void PopulateFrom(IProp prop)
    {
        if (prop is not IHasSegments segmentsProp)
        {
            throw new InvalidOperationException($"Prop {prop.GetType()} does not implement IHasSegments.");
        }

        page.Segments = new ObservableCollection<SegmentFeatureWizardItem>(
            segmentsProp.Segments.Select(segment => new SegmentFeatureWizardItem
            {
                StartDisplay = Format(segment.Start),
                EndDisplay = Format(segment.End),
                PointCount = segment.PointCount
            }));

        page.RefreshTotals();
    }

    public void ApplyTo(IProp prop)
    {
        if (prop is not IHasSegments segmentsProp)
        {
            throw new InvalidOperationException($"Prop {prop.GetType()} does not implement IHasSegments.");
        }

        if (page.Segments.Count != segmentsProp.Segments.Count)
        {
            throw new InvalidOperationException("Segments page state no longer matches the prop segment count.");
        }

        if (page.Segments.Any(segment => segment.PointCount <= 0))
        {
            throw new InvalidOperationException("All segment point counts must be greater than zero.");
        }

        segmentsProp.ReplaceSegments(
            segmentsProp.Segments.Zip(page.Segments, (segment, item) =>
                    segment with { PointCount = item.PointCount })
                .ToArray());
    }

    private static string Format(System.Numerics.Vector2 point) => $"({point.X:0.###}, {point.Y:0.###})";
}
