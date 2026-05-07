using OpenTK.Graphics.OpenGL;
using Props.Abstractions.PropVisualModels;
using Props.OpenGlCommon.Constructs.DrawingEngine.Shape;
using Props.OpenGlCommon.Constructs.Vertex;

namespace Props.OpenGlCommon.Constructs.DrawingEngine;

public class LightCloudShape : IOpenGLLightBasedDrawable, IDisposable
{
    private readonly LightPointCloud _cloud;
    private List<float>? _pointsCache;

    public LightCloudShape(LightPointCloud cloud)
    {
        _cloud = cloud;
    }

    public VBO<float> VertexBufferObject { get; set; }
    public int VAO { get; set; }
    public int PointsBufferSize { get; set; }

    public List<float> GetPoints()
    {
        if (_pointsCache is not null)
            return _pointsCache;

        _pointsCache = new List<float>(_cloud.Points.Count * 8);
        foreach (var point in _cloud.Points)
        {
            _pointsCache.Add(point.Position.X);
            _pointsCache.Add(point.Position.Y);
            _pointsCache.Add(point.Position.Z);
            _pointsCache.Add(255f);
            _pointsCache.Add(255f);
            _pointsCache.Add(255f);
            _pointsCache.Add(255f);
            _pointsCache.Add(_cloud.PointSize);
        }
        return _pointsCache;
    }

    public void Dispose()
    {
        VertexBufferObject?.Dispose();
        if (VAO != 0)
        {
            GL.DeleteVertexArray(VAO);
            VAO = 0;
        }
    }
}
