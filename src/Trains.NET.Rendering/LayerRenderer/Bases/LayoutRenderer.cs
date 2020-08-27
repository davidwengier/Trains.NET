using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public abstract class LayoutRenderer<T> : ILayerRenderer where T : class, IStaticEntity
    {
        private readonly ILayout<T> _layout;
        private readonly IRenderer<T> _renderer;
        private readonly IGameParameters _gameParameters;
        private readonly IImageFactory _imageFactory;
        private readonly Dictionary<string, IImage> _cache = new Dictionary<string, IImage>();

        public bool Enabled { get; set; }
        public string Name { get; internal set; } = string.Empty;

        public LayoutRenderer(ILayout<T> layout, IRenderer<T> renderer, IGameParameters gameParameters, IImageFactory imageFactory)
        {
            _layout = layout;
            _renderer = renderer;
            _gameParameters = gameParameters;
            _gameParameters.GameScaleChanged += (s, e) =>
            {
                _cache.Clear();
            };
            _imageFactory = imageFactory;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (T entity in _layout)
            {
                (int x, int y) = pixelMapper.CoordsToViewPortPixels(entity.Column, entity.Row);

                if (x < -_gameParameters.CellSize || y < -_gameParameters.CellSize || x > width || y > height) continue;

                canvas.Save();

                canvas.Translate(x, y);

                canvas.ClipRect(new Rectangle(0, 0, _gameParameters.CellSize, _gameParameters.CellSize), false);

                if(_renderer is ICachableRenderer<T> cachableRenderer)
                {
                    string key = cachableRenderer.GetCacheKey(entity);

                    if (!_cache.TryGetValue(key, out IImage cachedImage))
                    {
                        using IImageCanvas imageCanvas = _imageFactory.CreateImageCanvas(_gameParameters.CellSize, _gameParameters.CellSize);

                        float scale = _gameParameters.CellSize / 100.0f;

                        imageCanvas.Canvas.Scale(scale, scale);

                        _renderer.Render(imageCanvas.Canvas, entity);

                        cachedImage = imageCanvas.Render();

                        _cache[key] = cachedImage;
                    }

                    canvas.DrawImage(cachedImage, 0, 0);
                }
                else
                {
                    _renderer.Render(canvas, entity);
                }

                canvas.Restore();
            }
        }
    }
}
