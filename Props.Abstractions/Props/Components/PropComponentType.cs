namespace Props.Abstractions.Props.Components
{
    /// <summary>
    /// Specifies whether a prop component was defined by the prop itself or by the user.
    /// </summary>
    public enum PropComponentType
    {
        /// <summary>The component is defined and managed by the prop implementation.</summary>
        PropDefined,

        /// <summary>The component was added by the user at runtime.</summary>
        UserDefined
    }
}