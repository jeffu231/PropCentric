using System.ComponentModel;

namespace Props.Abstractions.Features;

/// <summary>
/// Specifies the supported light color modes for a prop.
/// </summary>
public enum LightType
{
    /// <summary>All lights share a single fixed color.</summary>
    [Description("Single color")]
    SingleColor,

    /// <summary>The prop uses a discrete named set of one or more colors.</summary>
    [Description("Multiple discrete colors")]
    MultipleDiscreteColors,

    /// <summary>The prop supports full color mixing with an ordered channel layout.</summary>
    [Description("Full color")]
    FullColor
}
