using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public abstract class LayoutRenderer<T> : ILayerRenderer where T : class, IStaticEntity
    {
        private readonly ILayout<T> _layout;
        private readonly IEnumerable<IRenderer<T>> _renderers;
        private readonly IImageFactory _imageFactory;
        private readonly Dictionary<string, IImage> _cache = new Dictionary<string, IImage>();

        public bool Enabled { get; set; }
        public string Name { get; internal set; } = string.Empty;

        private int _lastCellSize;

        public LayoutRenderer(ILayout<T> layout, IEnumerable<IRenderer<T>> renderers, IImageFactory imageFactory)
        {
            _layout = layout;
            _renderers = renderers;
            _imageFactory = imageFactory;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            if (pixelMapper.CellSize != _lastCellSize)
            {
                _cache.Clear();
                _lastCellSize = pixelMapper.CellSize;
            }

            foreach (T entity in _layout)
            {
                (int x, int y, bool onScreen) = pixelMapper.CoordsToViewPortPixels(entity.Column, entity.Row);

                if (!onScreen) continue;

                canvas.Save();

                canvas.Translate(x, y);

                foreach (IRenderer<T> renderer in _renderers)
                {
                    if (!renderer.ShouldRender(entity))
                    {
                        continue;
                    }

                    if (renderer is ICachableRenderer<T> cachableRenderer)
                    {
                        canvas.ClipRect(new Rectangle(0, 0, pixelMapper.CellSize, pixelMapper.CellSize), false);

                        string key = renderer.GetType().Name + "." + cachableRenderer.GetCacheKey(entity);

                        if (!_cache.TryGetValue(key, out IImage cachedImage))
                        {
                            using IImageCanvas imageCanvas = _imageFactory.CreateImageCanvas(pixelMapper.CellSize, pixelMapper.CellSize);

                            float scale = pixelMapper.CellSize / 100.0f;

                            imageCanvas.Canvas.Scale(scale, scale);

                            renderer.Render(imageCanvas.Canvas, entity);

                            cachedImage = imageCanvas.Render();

                            _cache[key] = cachedImage;
                        }

                        canvas.DrawImage(cachedImage, 0, 0);
                    }
                    else
                    {
                        float scale = pixelMapper.CellSize / 100.0f;

                        canvas.Scale(scale, scale);

                        renderer.Render(canvas, entity);
                    }
                }

                canvas.Restore();
            }
        }
    }
}
