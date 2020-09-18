using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public abstract class LayoutRenderer<T> : ILayerRenderer where T : class, IStaticEntity
    {
        private readonly ILayout<T> _layout;
        private readonly IEnumerable<IRenderer<T>> _renderers;
        private readonly IImageFactory _imageFactory;
        private readonly ITerrainMap _terrainMap;
        private readonly IImageCache _imageCache;

        public bool Enabled { get; set; }
        public string Name { get; internal set; } = string.Empty;
        public bool IsScaledByHeight { get; set; }

        private int _lastCellSize;

        public LayoutRenderer(ILayout<T> layout, IEnumerable<IRenderer<T>> renderers, IImageFactory imageFactory, ITerrainMap terrainMap, IImageCache imageCache)
        {
            _layout = layout;
            _renderers = renderers;
            _imageFactory = imageFactory;
            _terrainMap = terrainMap;
            _imageCache = imageCache;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            if (pixelMapper.CellSize != _lastCellSize)
            {
                _imageCache.Clear();
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

                    float heightScale = 1;
                    if (this.IsScaledByHeight)
                    {
                        heightScale = _terrainMap.Get(entity.Column, entity.Row).GetScaleFactor();
                    }

                    if (renderer is ICachableRenderer<T> cachableRenderer)
                    {
                        canvas.ClipRect(new Rectangle(0, 0, pixelMapper.CellSize, pixelMapper.CellSize), false);

                        string key = $"{renderer.GetType().Name}.{cachableRenderer.GetCacheKey(entity)}.{heightScale}";

                        if (_imageCache.IsDirty(key))
                        {
                            using IImageCanvas imageCanvas = _imageFactory.CreateImageCanvas(pixelMapper.CellSize, pixelMapper.CellSize);

                            float scale = pixelMapper.CellSize / 100.0f;

                            imageCanvas.Canvas.Scale(scale, scale);

                            if (heightScale < 1)
                            {
                                imageCanvas.Canvas.Scale(heightScale, heightScale, 50, 50);
                            }

                            renderer.Render(imageCanvas.Canvas, entity);

                            _imageCache.Set(key, imageCanvas.Render());
                        }

                        canvas.DrawImage(_imageCache.Get(key)!, 0, 0);
                    }
                    else
                    {
                        float scale = pixelMapper.CellSize / 100.0f;
                        canvas.Scale(scale, scale);
                        if (heightScale < 1)
                        {
                            canvas.Scale(heightScale, heightScale, 50, 50);
                        }
                    }

                }

                canvas.Restore();
            }
        }
    }
}
