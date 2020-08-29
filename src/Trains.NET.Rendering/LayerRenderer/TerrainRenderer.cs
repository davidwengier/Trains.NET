using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(0)]
    public class TerrainRenderer : ICachableLayerRenderer
    {
        private readonly ITerrainMap _terrainMap;
        private readonly IImageFactory _imageFactory;
        private IImage? _terrainImage;
        private bool _dirty;

        public TerrainRenderer(ITerrainMap terrainMap, IImageFactory imageFactory)
        {
            _terrainMap = terrainMap;
            _imageFactory = imageFactory;
            _terrainMap.CollectionChanged += (s, e) => _dirty = true;
        }

        public bool Enabled { get; set; } = true;
        public string Name => "Terrain";
        public bool IsDirty => _dirty;

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            if (_terrainMap.IsEmpty())
            {
                return;
            }

            if (_dirty == true || _terrainImage == null)
            {
                // Should we be getting this from here?
                int columns = pixelMapper.MaxGridSize / pixelMapper.CellSize;
                int rows = pixelMapper.MaxGridSize / pixelMapper.CellSize;

                // If we try to build before we know the size of the world, stay marked as dirty/null
                if (columns < 1 || rows < 1)
                {
                    return;
                }

                _terrainImage = BuildTerrainImage(columns, rows);

                _dirty = false;
            }

            (Rectangle source, Rectangle destination) = GetSourceAndDestinationRectangles(pixelMapper);

            canvas.DrawImage(_terrainImage, source, destination);
        }

        private static (Rectangle Source, Rectangle Destination) GetSourceAndDestinationRectangles(IPixelMapper pixelMapper)
        {
            (int topLeftColumn, int topLeftRow) = pixelMapper.ViewPortPixelsToCoords(0, 0);
            (int bottomRightColumn, int bottomRightRow) = pixelMapper.ViewPortPixelsToCoords(pixelMapper.ViewPortWidth, pixelMapper.ViewPortHeight);

            bottomRightColumn += 1;
            bottomRightRow += 1;

            Rectangle source = new(topLeftColumn, topLeftRow, bottomRightColumn, bottomRightRow);

            (int destinationTopLeftX, int destinationTopLeftY, _) = pixelMapper.CoordsToViewPortPixels(topLeftColumn, topLeftRow);
            (int destinationBottomRightX, int destinationBottomRightY, _) = pixelMapper.CoordsToViewPortPixels(bottomRightColumn, bottomRightRow);

            Rectangle destination = new(destinationTopLeftX, destinationTopLeftY, destinationBottomRightX, destinationBottomRightY);

            return (source, destination);
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
