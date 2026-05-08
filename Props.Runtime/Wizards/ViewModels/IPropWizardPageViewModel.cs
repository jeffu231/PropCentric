using Props.OpenGlCommon;

namespace Props.Runtime.Wizards.ViewModels;

/// <summary>
/// Exposes the OpenGL drawing engine state for wizard page view models that render a live 3-D preview.
/// </summary>
public interface IPropWizardPageViewModel
{
    /// <summary>Gets the OpenGL drawing engine used to render the prop preview.</summary>
    /// <value>The <see cref="OpenGLPropDrawingEngine"/> instance shared with the preview control.</value>
    OpenGLPropDrawingEngine DrawingEngine { get; }

    /// <summary>Gets a value that indicates whether the drawing engine has been initialized by the view.</summary>
    /// <value><see langword="true"/> if the engine is ready to accept model updates; otherwise, <see langword="false"/>.</value>
    bool IsDrawingEngineInitialized { get; }
}