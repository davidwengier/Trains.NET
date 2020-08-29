using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public class TerrainMapRenderer : ITerrainMapRenderer
    {
        private readonly ITerrainMap _terrainMap;
        private readonly IImageFactory _imageFactory;
        private IImage? _terrainImage;
        private bool _dirty;

        public TerrainMapRenderer(ITerrainMap terrainMap, IImageFactory imageFactory)
        {
            _terrainMap = terrainMap;
            _imageFactory = imageFactory;
            _terrainMap.CollectionChanged += (s, e) => _dirty = true;
        }
        public bool TryGetTerrainImage([NotNullWhen(true)] out IImage? image)
        {
            if (_terrainMap.IsEmpty())
            {
                image = null;
                return false;
            }

            if (_dirty == true || _terrainImage == null)
            {
                // Should we be getting this from here?
                int columns = _terrainMap.Max(x => x.Column);
                int rows = _terrainMap.Max(x => x.Row);

                // If we try to build before we know the size of the world, stay marked as dirty/null
                if (columns < 1 || rows < 1)
                {
                    image = null;
                    return false;
                }

                _terrainImage = BuildTerrainImage(columns, rows);

                _dirty = false;
            }

            image = _terrainImage;
            return true;
        }

        private IImage BuildTerrainImage(int columns, int rows)
        {
            using IImageCanvas textureImage = _imageFactory.CreateImageCanvas(columns, rows);

            textureImage.Canvas.DrawRect(0, 0, columns, rows,
                                    new PaintBrush
                                    {
                                        Style = PaintStyle.Fill,
                                        Color = TerrainColourLookup.DefaultColour
                                    });

            foreach (Terrain terrain in _terrainMap)
            {
                Color colour = TerrainColourLookup.GetTerrainColour(terrain);

                if (colour == TerrainColourLookup.DefaultColour) continue;

                textureImage.Canvas.DrawRect(terrain.Column, terrain.Row, 1, 1,
                                    new PaintBrush
                                    {
                                        Style = PaintStyle.Fill,
                                        Color = colour
                                    });
            }

            return textureImage.Render();
        }
    }
}
