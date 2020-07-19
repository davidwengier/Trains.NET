using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains.NET.Rendering
{
    [Order(0)]
    internal class TerrainTypeRenderer : ILayerRenderer
    {
        private readonly ITerrainMap _terrainMap;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _trackParameters;

        public TerrainTypeRenderer(ITerrainMap terrainMap, IPixelMapper pixelMapper, ITrackParameters trackParameters)
        {
            _terrainMap = terrainMap;
            _pixelMapper = pixelMapper;
            _trackParameters = trackParameters;
        }

        public bool Enabled { get; set; } = false;
        public string Name => "TerrainType";

        public void Render(ICanvas canvas, int width, int height)
        {
            DrawTerrainTypes(canvas);
        }

        private void DrawTerrainTypes(ICanvas canvas)
        {
            foreach (var terrain in _terrainMap)
            {
                var color = terrain.TerrainType switch
                {
                    TerrainType.Grass => Colors.LightGreen,
                    TerrainType.Sand => Colors.LightYellow,
                    TerrainType.Water => Colors.LightBlue,
                    _ => Colors.Empty,
                };

                var paintBrush = new PaintBrush
                {
                    Color = color,
                    Style = PaintStyle.Fill
                };

                (int x, int y) = _pixelMapper.CoordsToViewPortPixels(terrain.Column, terrain.Row);
                canvas.DrawRect(x, y, _trackParameters.CellSize, _trackParameters.CellSize, paintBrush);
            }
        }

    }
}
