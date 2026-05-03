namespace Props.Abstractions.Features;

[PropFeature(PropFeatureFlags.Dimming)]
public interface IHasDimming
{
    double Brightness { get; set; }
    
    double Gamma { get; set; }
}