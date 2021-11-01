using System;
using SkiaSharp;

namespace Trains.NET.Rendering.Skia;

public class SKPathWrapper : IPath, IDisposable
{
    private readonly SKPath _path = new();

    public SKPath Path => _path;

    public void LineTo(float x, float y) => _path.LineTo(x, y);

    public void MoveTo(float x, float y) => _path.MoveTo(x, y);

    public void Dispose()
    {
        _path.Dispose();
    }

    public void ConicTo(float controlX, float controlY, float x, float y, float w) =>
                    _path.ConicTo(controlX, controlY, x, y, w);
}
