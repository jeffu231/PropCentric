using System.Drawing;
using Props.Abstractions.Features;
using Props.Registry;

namespace PropCentric.Tests.ColorCatalog;

/// <summary>
/// Verifies predefined and custom color catalog behavior.
/// </summary>
public class ColorConfigurationCatalogTests
{
    [Fact]
    public void GetDiscreteColorSets_ReturnsExpectedPredefinedSets()
    {
        var catalog = new InMemoryColorConfigurationCatalog();

        var sets = catalog.GetDiscreteColorSets();

        Assert.Collection(
            sets,
            rgb =>
            {
                Assert.Equal("RGB", rgb.Name);
                Assert.Equal([Color.Red, Color.Lime, Color.Blue], rgb.Colors);
            },
            rgbw =>
            {
                Assert.Equal("RGBW", rgbw.Name);
                Assert.Equal([Color.Red, Color.Lime, Color.Blue, Color.White], rgbw.Colors);
            },
            grbw =>
            {
                Assert.Equal("GRBW", grbw.Name);
                Assert.Equal([Color.Lime, Color.Red, Color.Blue, Color.White], grbw.Colors);
            });
    }

    [Fact]
    public void GetFullColorOrders_ReturnsExpectedPredefinedOrders()
    {
        var catalog = new InMemoryColorConfigurationCatalog();

        var orders = catalog.GetFullColorOrders();

        Assert.Equal(["RGB", "RBG", "GBR", "GRB", "BRG", "BGR", "RGBW", "GRWB"], orders.Select(order => order.Name));
        Assert.Equal(
            [LightColorChannel.Green, LightColorChannel.Red, LightColorChannel.White, LightColorChannel.Blue],
            orders.Single(order => order.Name == "GRWB").Channels);
    }

    [Fact]
    public void SaveDiscreteColorSet_AddsCustomSetToFutureQueries()
    {
        var catalog = new InMemoryColorConfigurationCatalog();
        var customSet = new DiscreteColorSetDefinition("Cool Whites", [Color.White, Color.AliceBlue]);

        catalog.SaveDiscreteColorSet(customSet);

        var reloaded = Assert.Single(catalog.GetDiscreteColorSets(), set => set.Name == "Cool Whites");
        Assert.Equal(customSet.Colors, reloaded.Colors);
        Assert.NotSame(customSet.Colors, reloaded.Colors);
    }

    [Fact]
    public void SaveDiscreteColorSet_RejectsDuplicateNamesIgnoringCase()
    {
        var catalog = new InMemoryColorConfigurationCatalog();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            catalog.SaveDiscreteColorSet(new DiscreteColorSetDefinition("rgb", [Color.Red])));

        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public void SaveDiscreteColorSet_RejectsEmptyColorLists()
    {
        var catalog = new InMemoryColorConfigurationCatalog();

        Assert.Throws<ArgumentException>(() =>
            catalog.SaveDiscreteColorSet(new DiscreteColorSetDefinition("Empty", [])));
    }
}
