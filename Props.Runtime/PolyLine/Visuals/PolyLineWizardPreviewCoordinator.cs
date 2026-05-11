using Props.Abstractions.PropVisualModels;
using Props.Abstractions.Visuals;
using Props.Runtime.PolyLine.Setup;

namespace Props.Runtime.PolyLine.Visuals;

/// <summary>
/// Coordinates incremental <see cref="PolyLinePropVisualModel"/> rebuilds during wizard editing,
/// skipping geometry generation when the inputs have not changed.
/// </summary>
public sealed class PolyLineWizardPreviewCoordinator : IWizardPreviewCoordinator<PolyLinePropDraft>
{
    private readonly IVisualInputMapper<PolyLinePropDraft, PolyLineVisualInput> _mapper;
    private readonly IPropVisualModelBuilder<PolyLineVisualInput, PolyLinePropVisualModel> _builder;
    private PolyLineVisualInput? _lastInput;
    private IPropVisualModel? _lastModel;

    /// <summary>Initializes a new instance of the <see cref="PolyLineWizardPreviewCoordinator"/> class.</summary>
    /// <param name="mapper">The mapper that projects a draft onto a <see cref="PolyLineVisualInput"/>.</param>
    /// <param name="builder">The factory that produces a visual model from a <see cref="PolyLineVisualInput"/>.</param>
    public PolyLineWizardPreviewCoordinator(
        IVisualInputMapper<PolyLinePropDraft, PolyLineVisualInput> mapper,
        IPropVisualModelBuilder<PolyLineVisualInput, PolyLinePropVisualModel> builder)
    {
        _mapper = mapper;
        _builder = builder;
    }

    /// <inheritdoc />
    public IPropVisualModel BuildPreview(PolyLinePropDraft draft)
    {
        var input = _mapper.Map(draft);
        if (_lastInput is not null && InputsEqual(input, _lastInput) && _lastModel is not null)
        {
            return _lastModel;
        }

        _lastInput = input;
        _lastModel = _builder.Create(input);
        return _lastModel;
    }

    private static bool InputsEqual(PolyLineVisualInput left, PolyLineVisualInput right)
    {
        if (left.LightSize != right.LightSize)
        {
            return false;
        }

        if (left.Segments.Count != right.Segments.Count || left.AxisRotations.Count != right.AxisRotations.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Segments.Count; index++)
        {
            if (left.Segments[index] != right.Segments[index])
            {
                return false;
            }
        }

        for (var index = 0; index < left.AxisRotations.Count; index++)
        {
            if (left.AxisRotations[index].Axis != right.AxisRotations[index].Axis ||
                left.AxisRotations[index].RotationAngle != right.AxisRotations[index].RotationAngle)
            {
                return false;
            }
        }

        return true;
    }
}
