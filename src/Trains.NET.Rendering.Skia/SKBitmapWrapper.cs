using System;
using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    internal class SKBitmapWrapper : IBitmap, IDisposable
    {
        private readonly SKBitmap _bitmap;

        public SKBitmapWrapper(int width, int height)
        {
            _bitmap = new SKBitmap(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
        }

        public SKBitmap Bitmap => _bitmap;

        public void Dispose()
        {
            ((IDisposable)_bitmap).Dispose();
        }
    }
}
