using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public class TerrainMapRenderer : ITerrainMapRenderer
    {
        private readonly ITerrainMap _terrainMap;
        private readonly IImageFactory _imageFactory;
        private readonly IImageCache _imageCache;
        private readonly IPixelMapper _pixelMapper;

        public event EventHandler? Changed;

        public TerrainMapRenderer(ITerrainMap terrainMap, IImageFactory imageFactory, IImageCache imageCache, IPixelMapper pixelMapper)
        {
            _terrainMap = terrainMap;
            _imageFactory = imageFactory;
            _imageCache = imageCache;
            _pixelMapper = pixelMapper;

            _terrainMap.CollectionChanged += (s, e) => _imageCache.SetDirty(this);
        }
        public IImage GetTerrainImage()
        {
            if (_imageCache.IsDirty(this))
            {
                int width = _pixelMapper.Columns;
                int _height = _pixelMapper.Rows;

                using IImageCanvas textureImage = _imageFactory.CreateImageCanvas(width, _height);

                textureImage.Canvas.DrawRect(0, 0, width, _height, GetPaint(TerrainColourLookup.DefaultColour));

                foreach (Terrain terrain in _terrainMap)
                {
                    Color colour = TerrainColourLookup.GetTerrainColour(terrain);

                    if (colour == TerrainColourLookup.DefaultColour) continue;

                    textureImage.Canvas.DrawRect(terrain.Column, terrain.Row, 1, 1, GetPaint(colour));
                }

                _imageCache.Set(this, textureImage.Render());

                Changed?.Invoke(this, EventArgs.Empty);
            }

            return _imageCache.Get(this)!;
        }

        private static PaintBrush GetPaint(Color colour)
            => new PaintBrush
            {
                Style = PaintStyle.Fill,
                Color = colour
            };
    }
}
