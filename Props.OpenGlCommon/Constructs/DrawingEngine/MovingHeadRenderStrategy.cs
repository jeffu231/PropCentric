using Props.OpenGlCommon.Constructs.DrawingEngine.Shape;

namespace Props.OpenGlCommon.Constructs.DrawingEngine;

public class MovingHeadRenderStrategy
{
    public List<IRenderMovingHeadOpenGL> Shapes { get; } = [];

    public void Initialize() { }

    public void RenderVolumes(Matrix4 perspective, Vector3 cameraPosition, Matrix4 viewMatrix, int backgroundAlpha) { }
}
