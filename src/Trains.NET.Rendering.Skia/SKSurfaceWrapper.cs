using SkiaSharp;

namespace Trains.NET.Rendering.Skia;

public class SKSurfaceWrapper : IImageCanvas
{
    private readonly SKSurface _surface;
    private readonly ICanvas _canvas;
    private readonly int _logicalWidth;
    private readonly int _logicalHeight;
    private readonly float _scaleX;
    private readonly float _scaleY;

    public SKSurfaceWrapper(int width, int height, GRContext? context, float scaleX, float scaleY)
    {
        _logicalWidth = width;
        _logicalHeight = height;
        _scaleX = scaleX;
        _scaleY = scaleY;

        var physicalWidth = (int)Math.Ceiling(width * scaleX);
        var physicalHeight = (int)Math.Ceiling(height * scaleY);
        var info = new SKImageInfo(physicalWidth, physicalHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
        _surface = context != null ?
                    SKSurface.Create(context, true, info) :
                    SKSurface.Create(info);

        _canvas = new SKCanvasWrapper(_surface.Canvas);
        _canvas.Scale(scaleX, scaleY);
    }

    public ICanvas Canvas => _canvas;

    public IImage Render() => new SKImageWrapper(_surface.Snapshot(), _logicalWidth, _logicalHeight, _scaleX, _scaleY);

    public void Dispose()
    {
        _surface.Dispose();
    }
}
