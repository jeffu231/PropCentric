namespace Props.Abstractions.Props;

public abstract class Prop:IProp
{
    public Guid Id { get; init; } = Guid.NewGuid();
}