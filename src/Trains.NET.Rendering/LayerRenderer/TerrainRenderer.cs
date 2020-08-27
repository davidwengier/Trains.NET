using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(0)]
    public class TerrainRenderer : ICachableLayerRenderer
    {
        private bool _dirty;

        private readonly ITerrainMap _terrainMap;
        private readonly IGameParameters _gameParameters;

        public TerrainRenderer(ITerrainMap terrainMap, IGameParameters gameParameters)
        {
            _terrainMap = terrainMap;
            _gameParameters = gameParameters;

            _terrainMap.CollectionChanged += (s, e) => _dirty = true;
            // Dirty Hack
            gameParameters.GameScaleChanged += (s, e) => _dirty = true;
        }

        public bool Enabled { get; set; } = true;
        public string Name => "Terrain";
        public bool IsDirty => _dirty;

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            if (_terrainMap.IsEmpty())
            {
                canvas.DrawRect(0, 0, pixelMapper.ViewPortWidth, pixelMapper.ViewPortHeight, new PaintBrush { Style = PaintStyle.Fill, Color = TerrainColourLookup.DefaultColour });
                return;
            }

            // Draw any non-grass cells
            foreach (Terrain terrain in _terrainMap)
            {
                Color colour = TerrainColourLookup.GetTerrainColour(terrain);

                (int x, int y) = pixelMapper.CoordsToViewPortPixels(terrain.Column, terrain.Row);
                canvas.DrawRect(x, y, _gameParameters.CellSize, _gameParameters.CellSize, new PaintBrush { Style = PaintStyle.Fill, Color = colour });
                // Debug, this draws coord and height onto cells
                //canvas.DrawText($"{terrain.Column},{terrain.Row}", x + 2, y + 0.3f * _gameParameters.CellSize, new PaintBrush { Style = PaintStyle.Fill, Color = Colors.Black });
                //canvas.DrawText($"{terrain.Height}", x + 2, y + 0.7f * _gameParameters.CellSize, new PaintBrush { Style = PaintStyle.Fill, Color = Colors.Black });
            }

            _dirty = false;
        }
    }
}
