﻿using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public abstract class CachableLayoutRenderer<T> : LayoutRenderer<T>, ICachableLayerRenderer where T : class, IStaticEntity
    {
        private bool _dirty;

        public bool IsDirty => _dirty;

        public CachableLayoutRenderer(ILayout<T> layout, IRenderer<T> renderer, IImageFactory imageFactory, ITerrainMap terrainMap, IImageCache imageCache)
            : this(layout, new[] { renderer }, imageFactory, terrainMap, imageCache)
        {
        }

        public CachableLayoutRenderer(ILayout<T> layout, IEnumerable<IRenderer<T>> renderers, IImageFactory imageFactory, ITerrainMap terrainMap, IImageCache imageCache)
            : base (layout, renderers, imageFactory, terrainMap, imageCache)
        {
            layout.CollectionChanged += (s, e) => _dirty = true;
        }

        public new void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            base.Render(canvas, width, height, pixelMapper);

            _dirty = false;
        }
    }
}
