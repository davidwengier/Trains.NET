namespace Trains.NET.Rendering
{
    public interface IPath
    {
        public void MoveTo(float x, float y);
        public void LineTo(float x, float y);
        public void ArcTo(float radiusX, float radiusY, int xAxisRotate, PathArcSize arcSize, PathDirection direction, float x, int y);
        public void QuadTo(float controlX, float controlY, float x, float y);
    }
}
