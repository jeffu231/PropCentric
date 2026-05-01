namespace Props.Abstractions.Props;

/// <summary>
/// DTO for a collection of props.
/// </summary>
public class PropGroup<TProp> : IPropGroup<TProp> where TProp : IProp
{
    #region IPropGroup

    ///<inheritdoc/>
    public bool CreateGroup { get; set; }

    ///<inheritdoc/>
    public string GroupName { get; set; } = String.Empty;

    ///<inheritdoc/>
    public IList<TProp> Props { get; init; } = new List<TProp>();

    #endregion
}