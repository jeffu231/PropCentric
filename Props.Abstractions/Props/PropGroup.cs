namespace Props.Abstractions.Props;

/// <summary>
/// DTO for a collection of props.
/// </summary>
public class PropGroup : IPropGroup
{
    #region IPropGroup

    ///<inheritdoc/>
    public bool CreateGroup { get; set; }

    ///<inheritdoc/>
    public string GroupName { get; set; } = String.Empty;

    ///<inheritdoc/>
    public IList<IProp> Props { get; init; } = new List<IProp>();

    #endregion
}