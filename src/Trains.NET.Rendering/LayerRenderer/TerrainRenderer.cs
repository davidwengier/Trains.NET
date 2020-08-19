using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(0)]
    public class TerrainRenderer : ICachableLayerRenderer
    {
        private bool _dirty;

        private readonly ITerrainMap _terrainMap;
        private readonly IGameParameters _gameParameters;

        private static readonly Color s_grassColor = new Color("#78B159");
        private static readonly Color s_sandColor = new Color("#FDCB58");
        private static readonly Color s_waterColor = new Color("#55ACEE");

        public TerrainRenderer(ITerrainMap terrainMap, IGameParameters gameParameters)
        {
            _terrainMap = terrainMap;
            _gameParameters = gameParameters;

            _terrainMap.CollectionChanged += (s, e) => _dirty = true;
        }

        public bool Enabled { get; set; } = true;
        public string Name => "Terrain";
        public bool IsDirty => _dirty;

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            // Draw grass background for viewport
            canvas.DrawRect(0, 0, pixelMapper.ViewPortWidth, pixelMapper.ViewPortHeight, new PaintBrush { Style = PaintStyle.Fill, Color = s_grassColor });

            // Draw any non-grass cells
            foreach (Terrain terrain in _terrainMap)
            {
                Color colour = GetColour(terrain.TerrainType);

                if (colour == s_grassColor) continue;

                (int x, int y) = pixelMapper.CoordsToViewPortPixels(terrain.Column, terrain.Row);
                canvas.DrawRect(x, y, _gameParameters.CellSize, _gameParameters.CellSize, new PaintBrush { Style = PaintStyle.Fill, Color = colour });
            }

            _dirty = false;
        }

        public static Color GetColour(TerrainType terrain)
            => terrain switch
            {
                TerrainType.Sand => s_sandColor,
                TerrainType.Water => s_waterColor,
                _ => s_grassColor,
            };
    }
}
