using SkiaSharp;

namespace Trains.NET.Rendering.Skia;

public class SKImageWrapper(
    SKImage sKImage,
    int logicalWidth,
    int logicalHeight,
    float scaleX,
    float scaleY) : IImage
{
    private readonly SKImage _image = sKImage;

    public SKImage Image => _image;
    public int LogicalWidth { get; } = logicalWidth;
    public int LogicalHeight { get; } = logicalHeight;
    public float ScaleX { get; } = scaleX;
    public float ScaleY { get; } = scaleY;

    public void Dispose()
    {
        _image.Dispose();
    }
}
