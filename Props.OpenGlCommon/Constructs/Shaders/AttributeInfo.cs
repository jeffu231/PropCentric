namespace Props.OpenGlCommon.Constructs.Shaders;

public class AttributeInfo
{
    public string Name { get; set; } = string.Empty;
    public int Address { get; set; }
    public int Size { get; set; }
    public ActiveAttribType AttribType { get; set; }
}
