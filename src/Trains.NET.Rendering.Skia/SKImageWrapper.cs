using SkiaSharp;

namespace Trains.NET.Rendering.Skia;

public class SKImageWrapper(SKImage sKImage) : IImage
{
    private readonly SKImage _image = sKImage;

    public SKImage Image => _image;

    public void Dispose()
    {
        _image.Dispose();
    }
}
