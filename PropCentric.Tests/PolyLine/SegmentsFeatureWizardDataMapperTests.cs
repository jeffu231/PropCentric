using Props.Runtime.Wizards.Features.Segments.Mappers;
using Props.Runtime.Wizards.Features.Segments.Pages;

namespace PropCentric.Tests.PolyLine;

/// <summary>
/// Verifies segment feature page mapper behavior against polyline props.
/// </summary>
public class SegmentsFeatureWizardDataMapperTests
{
    [Fact]
    public void PopulateFrom_CopiesPropSegmentsIntoPageModel()
    {
        var prop = PolyLineTestData.CreateTreeProp();
        var page = new SegmentsFeatureWizardPage();
        var mapper = new SegmentsFeatureWizardDataMapper(page);

        mapper.PopulateFrom(prop);

        Assert.Equal(prop.Segments.Count, page.Segments.Count);
        Assert.Equal(prop.Segments.Sum(segment => segment.PointCount), page.TotalPoints);

        for (var index = 0; index < prop.Segments.Count; index++)
        {
            Assert.Equal(prop.Segments[index].PointCount, page.Segments[index].PointCount);
        }
    }

    [Fact]
    public void ApplyTo_UpdatesPointCountsWhilePreservingGeometry()
    {
        var prop = PolyLineTestData.CreateTreeProp();
        var page = new SegmentsFeatureWizardPage();
        var mapper = new SegmentsFeatureWizardDataMapper(page);

        mapper.PopulateFrom(prop);
        page.Segments[0].PointCount = 12;
        page.Segments[1].PointCount = 34;

        var originalSegments = prop.Segments.ToArray();
        mapper.ApplyTo(prop);

        Assert.Equal(12, prop.Segments[0].PointCount);
        Assert.Equal(34, prop.Segments[1].PointCount);
        Assert.Equal(originalSegments[0].Start, prop.Segments[0].Start);
        Assert.Equal(originalSegments[0].End, prop.Segments[0].End);
        Assert.Equal(originalSegments[1].Start, prop.Segments[1].Start);
        Assert.Equal(originalSegments[1].End, prop.Segments[1].End);
    }

    [Fact]
    public void ApplyTo_WithInvalidPointCount_ThrowsInvalidOperationException()
    {
        var prop = PolyLineTestData.CreateTreeProp();
        var page = new SegmentsFeatureWizardPage();
        var mapper = new SegmentsFeatureWizardDataMapper(page);

        mapper.PopulateFrom(prop);
        page.Segments[0].PointCount = 0;

        Assert.Throws<InvalidOperationException>(() => mapper.ApplyTo(prop));
    }
}
