using OpenTK.Mathematics;
using Props.Abstractions.Drawing;
using Props.Abstractions.PropVisualModels;
using Props.OpenGlCommon.Constructs.DrawingEngine;
using Props.OpenGlCommon.Constructs.DrawingEngine.Shape;

namespace Props.OpenGlCommon;

public class OpenGLPropDrawingEngine : OpenGLDrawingEngineBase<LightCloudShape>, IPropDrawingEngine
{
    private List<IPropVisualModel> _models = [];
    private List<LightCloudShape> _shapes = [];
    private bool _modelsDirty = false;

    public IReadOnlyList<IPropVisualModel> Models => _models.AsReadOnly();

    public void SetModels(IReadOnlyList<IPropVisualModel> models)
    {
        _models = [.. models];
        _modelsDirty = true;
    }

    public bool IsInitialized => !FormLoading;

    public void Render() => RenderPreview();

    // OpenTKDrawingAreaChanged computes FocalDepth from pixel height, but this engine
    // uses world-space coordinates (tree at ~[-0.5, 0.5]).  After the base call we
    // correct FocalDepth back to world-space so CalculatePointScaleFactor stays sane.
    public new void OpenTKDrawingAreaChanged(double width, double height)
    {
        base.OpenTKDrawingAreaChanged(width, height);
        if (!FormLoading && height > 0)
        {
            ExecuteCenterPreview();
            FocalDepth = FocalDepth * (float)(ProjectionHeight / height);
            CalculatePointScaleFactor();
        }
    }

    protected override IEnumerable<LightCloudShape> GetLightShapes() => _shapes;

    protected override void RenderPreviewInternal()
    {
        if (_modelsDirty)
        {
            foreach (var shape in _shapes)
                shape.Dispose();

            _shapes = _models
                .SelectMany(m => m.Elements)
                .OfType<LightPointCloud>()
                .Select(c => new LightCloudShape(c))
                .ToList();

            _modelsDirty = false;
        }

        ClearScreen();
        UpdateLightShapePoints(GetReferenceHeight());
        Matrix4 mvp = Camera.ViewMatrix * CreatePerspective();
        DrawPoints(mvp);
    }

    protected override void UpdateLightShapePoints(float referenceHeightY) { }

    protected override void DrawPoints(LightCloudShape shape) => DrawPointsUtility.DrawPoints(shape);

    protected override float GetReferenceHeight() => 1.0f;

    protected override float GetReferenceWidth() => 1.0f;

    protected override Vector3 GetCameraCenterPosition() => new Vector3(0, -.1f, 2);

    protected override float GetZoomFactor() => ProjectionHeight;

    protected override float GetFocalDepthMultiplier() => 1.4f;

    protected override int GetBackgroundAlpha() => 0;

    protected override IEnumerable<IOpenGLMovingHeadShape> GetMovingHeadShapes() => [];

    protected override IEnumerable<IDrawStaticPreviewShape> GetIDrawStaticPreviewShapes() => [];

    protected override void InitializeBackground()
    {
        GL.ClearColor(0f, 0f, 0f, 1f);
    }

    protected override float CalculatePointScale() => GetFocalDepthMultiplier() * 2.0f;

    protected override void ResetTimers() { }
}
