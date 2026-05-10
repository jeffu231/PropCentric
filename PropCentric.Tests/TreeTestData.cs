using System.Collections.ObjectModel;
using PropCentric.Tests.Common;
using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Props;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Visuals;

namespace PropCentric.Tests;

/// <summary>
/// Provides shared test helpers for constructing tree props and rotation data.
/// </summary>
internal static class TreeTestData
{
    /// <summary>
    /// Creates a configured <see cref="TreeProp"/> that can be reused across mapping and builder tests.
    /// </summary>
    /// <returns>A configured <see cref="TreeProp"/> instance.</returns>
    internal static TreeProp CreateTreeProp()
    {
        var prop = new TreeProp(new TreePropToVisualInputMapper(), new TreeVisualModelBuilder())
        {
            Name = "Configured Tree",
            Strings = 16,
            NodesPerString = 50,
            LightSize = 2,
            DegreesCoverage = 270,
            DegreeOffset = 30,
            BaseHeight = 45,
            TopHeight = 18,
            TopWidth = 24,
            StartLocation = StartLocation.BottomRight,
            ZigZag = true,
            ZigZagOffset = 20,
            TopRadius = 12,
            BottomRadius = 90,
            AxisRotations = TestDataHelper.CreateRotations((Axis.XAxis, 5), (Axis.YAxis, 10), (Axis.ZAxis, 15))
        };

        return prop;
    }

   
}
