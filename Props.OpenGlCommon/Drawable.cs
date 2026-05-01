using OpenTK.Mathematics;

namespace Props.OpenGlCommon
{
	public interface IDrawable:IDisposable
	{
		void Draw(Matrix4 fov, Matrix4 cameraView);
	}
}