using Props.Abstractions.PropVisualModels;

namespace PropCentric.Tests.Common;

/// <summary>
/// Verifies the shared axis rotation collection helper behavior.
/// </summary>
public class AxisRotationCollectionFactoryTests
{
    [Fact]
    public void CreateDefaultAxisRotations_ReturnsStandardZeroDegreeCollection()
    {
        var rotations = AxisRotationCollectionFactory.CreateDefaultAxisRotations();

        Assert.Collection(
            rotations,
            rotation =>
            {
                Assert.Equal(Axis.XAxis, rotation.Axis);
                Assert.Equal(0, rotation.RotationAngle);
            },
            rotation =>
            {
                Assert.Equal(Axis.YAxis, rotation.Axis);
                Assert.Equal(0, rotation.RotationAngle);
            },
            rotation =>
            {
                Assert.Equal(Axis.ZAxis, rotation.Axis);
                Assert.Equal(0, rotation.RotationAngle);
            });
    }

    [Fact]
    public void Clone_ReturnsDistinctItemsWithSameValues()
    {
        var source = TestDataHelper.CreateRotations((Axis.XAxis, 15), (Axis.ZAxis, -30));

        var clone = AxisRotationCollectionFactory.Clone(source);

        Assert.Equal(source.Count, clone.Count);

        for (var index = 0; index < source.Count; index++)
        {
            Assert.NotSame(source[index], clone[index]);
            Assert.Equal(source[index].Axis, clone[index].Axis);
            Assert.Equal(source[index].RotationAngle, clone[index].RotationAngle);
        }
    }
}
