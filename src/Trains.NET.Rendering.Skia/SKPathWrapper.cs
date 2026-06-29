using SkiaSharp;

namespace Trains.NET.Rendering.Skia;

public class SKPathWrapper : IPath, IDisposable
{
    private readonly SKPathBuilder _builder = new();
    private SKPath? _path;

    public SKPath Path => _path ??= _builder.Snapshot();

    public void LineTo(float x, float y)
    {
        _builder.LineTo(x, y);
        InvalidatePath();
    }

    public void MoveTo(float x, float y)
    {
        _builder.MoveTo(x, y);
        InvalidatePath();
    }

    public void Dispose()
    {
        _builder.Dispose();
        _path?.Dispose();
    }

    public void ConicTo(float controlX, float controlY, float x, float y, float w)
    {
        _builder.ConicTo(controlX, controlY, x, y, w);
        InvalidatePath();
    }

    private void InvalidatePath()
    {
        _path?.Dispose();
        _path = null;
    }
}
