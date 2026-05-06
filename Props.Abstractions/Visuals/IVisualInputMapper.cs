namespace Props.Abstractions.Visuals;

public interface IVisualInputMapper<in TSource, out TVisualInput>
{
    TVisualInput Map(TSource source);
}
