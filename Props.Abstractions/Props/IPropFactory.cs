namespace Props.Abstractions.Props;

public interface IPropFactory
{
    IProp Create(Guid id);
    TProp Create<TProp>() where TProp : IProp;
}