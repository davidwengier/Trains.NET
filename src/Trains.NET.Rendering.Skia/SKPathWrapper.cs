using System;
using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    public class SKPathWrapper : IPath, IDisposable
    {
        private readonly SKPath _path = new SKPath();

        public SKPath Path => _path;

        public void ArcTo(float radiusX, float radiusY, int xAxisRotate, PathArcSize arcSize, PathDirection direction, float x, int y)
            => _path.ArcTo(radiusX, radiusY, xAxisRotate, arcSize.ToSkia(), direction.ToSkia(), x, y);

        public void LineTo(float x, float y) => _path.LineTo(x, y);

        public void MoveTo(float x, float y) => _path.MoveTo(x, y);

        public void Dispose()
        {
            _path.Dispose();
        }

        public void ConicTo(float controlX, float controlY, float x, float y, float w) =>
                        _path.ConicTo(controlX, controlY, x, y, w);
    }
}
