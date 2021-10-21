using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    public class SKSurfaceWrapper : IImageCanvas
    {
        private readonly SKSurface _surface;

        public SKSurfaceWrapper(int width, int height, GRContext? context)
        {
            var info = new SKImageInfo(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            _surface = context != null ?
                        SKSurface.Create(context, true, info) :
                        SKSurface.Create(info);
        }

        public ICanvas Canvas => new SKCanvasWrapper(_surface.Canvas);

        public IImage Render() => new SKImageWrapper(_surface.Snapshot());

        public void Dispose()
        {
            _surface.Dispose();
        }
    }
}
