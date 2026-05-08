using System.ComponentModel;

namespace Vixen.Sys.Props
{
    /// <summary>
    /// Specifies the color wiring mode of the light strings on a prop.
    /// </summary>
    public enum StringTypes
    {
        /// <summary>All lights share a single fixed color.</summary>
        [Description("All Lights are a single color")]
        SingleColor,

        /// <summary>Each string carries an independent color signal.</summary>
        [Description("Multiple Independent Color Strings")]
        MultiColor,

        /// <summary>Lights support full per-pixel RGB color mixing.</summary>
        [Description("Full RGB color mixing")]
        ColorMixingRGB
    }
}