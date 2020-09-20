using System;
using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public class TerrainMapRenderer : ITerrainMapRenderer
    {
        private static readonly Color[] s_colourMap = BuildColourMap();

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

                foreach (Terrain terrain in _terrainMap)
                {
                    Color colour = GetTerrainColour(terrain);
                    PaintBrush paint = GetPaint(colour);

                    textureImage.Canvas.DrawRect(terrain.Column, terrain.Row, 1, 1, paint);
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

        public static Color GetTerrainColour(Terrain terrain)
            => s_colourMap[terrain.TerrainLevel];

        private static Color[] BuildColourMap()
        {
            var colours = new List<Color>();
            colours.AddRange(GetGradientColours(Colors.DarkBlue, Colors.LightBlue, Terrain.NumberWaterLevels));
            colours.AddRange(GetGradientColours(Colors.LightYellow, Colors.LightYellow, Terrain.NumberSandLevels)); // LOL
            colours.AddRange(GetGradientColours(Colors.LightGreen, Colors.DarkGreen, Terrain.NumberLandLevels));
            colours.AddRange(GetGradientColours(Colors.Gray, Colors.DirtyWhite, Terrain.NumberMountainLevels));
            return colours.ToArray();
        }

        private static IEnumerable<Color> GetGradientColours(Color start, Color end, int steps)
        {
            // special case for sand
            if (steps == 1)
            {
                yield return start;
                yield break;
            }

            int stepA = (end.A - start.A) / (steps - 1);
            int stepR = (end.R - start.R) / (steps - 1);
            int stepG = (end.G - start.G) / (steps - 1);
            int stepB = (end.B - start.B) / (steps - 1);

            for (int i = 0; i < steps; i++)
            {
                yield return new Color(start.A + (stepA * i),
                                       start.R + (stepR * i),
                                       start.G + (stepG * i),
                                       start.B + (stepB * i));
            }
        }
    }
}
