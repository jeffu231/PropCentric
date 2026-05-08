namespace Props.Abstractions.Props
{
    /// <summary>
    /// Specifies the corner of a prop where element patching begins.
    /// </summary>
    public enum StartLocation
    {
        /// <summary>Patching starts at the bottom-left corner.</summary>
        BottomLeft,

        /// <summary>Patching starts at the bottom-right corner.</summary>
        BottomRight,

        /// <summary>Patching starts at the top-left corner.</summary>
        TopLeft,

        /// <summary>Patching starts at the top-right corner.</summary>
        TopRight
    }
}