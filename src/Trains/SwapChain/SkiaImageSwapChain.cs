using System;
using SkiaSharp;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains
{
    [Order(10)]
    public class SkiaImageSwapChain //: ISwapChain // Order didn't want to work, so commenting out for the moment
    {
        private readonly IImageCache _imageCache;
        private readonly IImageFactory _imageFactory;
        private int _width;
        private int _height;

        public SkiaImageSwapChain(IImageCache imageCache, IImageFactory imageFactory)
        {
            _imageCache = imageCache;
            _imageFactory = imageFactory;
        }
        public void DrawNext(Action<ICanvas> draw)
        {
            using (_ = _imageCache.SuspendSetDirtyCalls())
            {
                using IImageCanvas imageCanvas = _imageFactory.CreateImageCanvas(_width, _height);

                draw(imageCanvas.Canvas);

                _imageCache.Set(this, imageCanvas.Render());
            }
        }
        public void SetSize(int width, int height)
        {
            if (width == _width && height == _height)
            {
                return;
            }

            _width = width;
            _height = height;

            _imageCache.Clear();
        }

        public void PresentCurrent(Action<SKImage> present)
        {
            var image = _imageCache.Get(this);

            if (_width < 1 || _height < 1 || image == null)
            {
                return;
            }

            present(image.ToSkia());
        }
    }
}
