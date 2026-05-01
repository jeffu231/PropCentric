using Catel.Data;

namespace Props.Abstractions.PropVisualModels
{
    /// <summary>
    /// Defines the coordinate system axis.
    /// </summary>
    public enum Axis
    {
        XAxis,
        YAxis,
        ZAxis
    };

    /// <summary>
    /// Maintains a rotation around an axis.
    /// </summary>
    public class AxisRotationModel : ModelBase
    {
        /// <summary>
        /// Axis to rotate about.
        /// </summary>
        public Axis Axis { get; set; }
        
        /// <summary>
        /// Rotation angle in degrees.
        /// </summary>
        public int RotationAngle
        {
            get { return GetValue<int>(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }
        
        /// <summary>
        /// Name property data.
        /// </summary>
        public static readonly IPropertyData RotationAngleProperty = RegisterProperty<int>(nameof(RotationAngle));

        /// <summary>
        /// Converts from axis string to enumeration.
        /// </summary>
        /// <param name="axis">String to convert</param>
        /// <returns>Equivalent enumeration of the string</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Axis ConvertAxis(string axis)
        {
            return Axis = axis switch
            {
                "X" => Axis.XAxis,
                "Y" => Axis.YAxis,
                "Z" => Axis.ZAxis,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), "Unsupported rotation axis")
            };
        }

        /// <summary>
        /// Converts from enumeration to axis string.
        /// </summary>
        /// <param name="axis">Enumeration to convert</param>
        /// <returns>Returns a <see cref="string"/> equivalent of the enumeration</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ConvertAxis(Axis axis)
        {
            return axis switch
            {
                Axis.XAxis => "X",
                Axis.YAxis => "Y",
                Axis.ZAxis => "Z",
                _ => throw new ArgumentOutOfRangeException(nameof(axis), "Unsupported rotation axis")
            };
        }

        public override string ToString()
        {
            return $"{RotationAngle}";
        }
    }
}