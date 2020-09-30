using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(700)]
    public class TunnelRenderer : ILayerRenderer
    {
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout<Track> _trackLayout;
        private readonly IGameBoard _gameBoard;
        private const int BuildModeAlpha = 170;

        public TunnelRenderer(ITerrainMap terrainMap, ILayout<Track> layout, IGameBoard gameBoard)
        {
            _terrainMap = terrainMap;
            _trackLayout = layout;
            _gameBoard = gameBoard;
        }

        public bool Enabled { get; set; }
        public string Name => "Tunnels";

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Track track in _trackLayout)
            {
                Terrain terrain = _terrainMap.Get(track.Column, track.Row);
                if (!terrain.IsMountain) continue;

                Color colour = TerrainMapRenderer.GetTerrainColour(terrain);

                (int x, int y, _) = pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);
                canvas.DrawRect(x, y, pixelMapper.CellSize, pixelMapper.CellSize,
                                new PaintBrush
                                {
                                    Style = PaintStyle.Fill,
                                    Color = BuildModeAwareColour(colour),
                                });

                TrackNeighbors trackNeighbours = track.GetNeighbors();

                Color lightColour = BuildModeAwareColour(Colors.LightGray);
                Color darkColour = BuildModeAwareColour(colour);

                if (IsTunnelEntrance(trackNeighbours.Up))
                {
                    DrawTunnelEntrance(canvas, pixelMapper, x, y, 270, lightColour, darkColour);
                }

                if (IsTunnelEntrance(trackNeighbours.Down))
                {
                    DrawTunnelEntrance(canvas, pixelMapper, x, y, 90, lightColour, darkColour);
                }

                if (IsTunnelEntrance(trackNeighbours.Left))
                {
                    DrawTunnelEntrance(canvas, pixelMapper, x, y, 180, lightColour, darkColour);
                }

                if (IsTunnelEntrance(trackNeighbours.Right))
                {
                    DrawTunnelEntrance(canvas, pixelMapper, x, y, 0, lightColour, darkColour);
                }
            }
        }

        private static void DrawTunnelEntrance(ICanvas canvas, IPixelMapper pixelMapper, int x, int y, float angle, Color lightColour, Color darkColor)
        {
            using (canvas.Scope())
            {
                canvas.Translate(x, y);
                canvas.RotateDegrees(angle, 0.5f * pixelMapper.CellSize, 0.5f * pixelMapper.CellSize);
                canvas.GradientRect(pixelMapper.CellSize, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, darkColor, lightColour);
                canvas.GradientCircle(0.5f * pixelMapper.CellSize,
                                      0,
                                      0.5f * pixelMapper.CellSize,
                                      pixelMapper.CellSize,
                                      pixelMapper.CellSize,
                                      0.5f * pixelMapper.CellSize,
                                      0.5f * pixelMapper.CellSize,
                                      new[] { lightColour, darkColor });
            }
        }

        private bool IsTunnelEntrance(Track? track)
            => track is not null && !_terrainMap.Get(track.Column, track.Row).IsMountain;

        private Color BuildModeAwareColour(Color color)
            => !_gameBoard.Enabled
                ? color with { A = BuildModeAlpha }
                : color;
    }
}
