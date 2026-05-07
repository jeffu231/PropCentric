namespace Props.OpenGlCommon.Constructs.Shaders;

public class UniformInfo
{
    public string Name { get; set; } = string.Empty;
    public int Address { get; set; }
    public int Size { get; set; }
    public ActiveUniformType UniformType { get; set; }
}
