using Props.OpenGlCommon;

namespace Props.Runtime.Wizards.ViewModels;

public interface IPropWizardPageViewModel
{
    OpenGLPropDrawingEngine DrawingEngine { get; }
    bool IsDrawingEngineInitialized { get; }
}