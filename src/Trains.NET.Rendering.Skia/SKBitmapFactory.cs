using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    public class SKBitmapFactory : IBitmapFactory
    {
        public IBitmap CreateBitmap(int width, int height) => new SKBitmapWrapper(width, height);

        public ICanvas CreateCanvas(IBitmap bitmap) => new SKCanvasWrapper(new SKCanvas(bitmap.ToSkia()));
    }
}
