namespace Trains.NET.Rendering
{
    public interface IPath
    {
        void MoveTo(float x, float y);
        void LineTo(float x, float y);
        void ConicTo(float controlX, float controlY, float x, float y, float w);
    }
}
