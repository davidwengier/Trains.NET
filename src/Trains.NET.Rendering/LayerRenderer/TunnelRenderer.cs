using Trains.NET.Engine;
using Trains.NET.Rendering.Drawing;

namespace Trains.NET.Rendering
{
    [Order(700)]
    public class TunnelRenderer : ILayerRenderer
    {
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout<Track> _trackLayout;
        private readonly IGameBoard _gameBoard;
        private const string BuildModeAlpha = "aa";

        public TunnelRenderer(ITerrainMap terrainMap, ILayout<Track> layout, IGameBoard gameBoard)
        {
            _terrainMap = terrainMap;
            _trackLayout = layout;
            _gameBoard = gameBoard;
        }

        public bool Enabled { get; set; } = true;
        public string Name => "Tunnels";

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (var track in _trackLayout)
            {
                var terrain = _terrainMap.GetTerrainOrDefault(track.Column, track.Row);
                if (terrain.Height < TerrainColourLookup.GetMountainLevel()) continue;

                Color colour = TerrainColourLookup.GetTerrainColour(terrain);

                if (colour == TerrainColourLookup.DefaultColour) continue;

                (int x, int y, bool onScreen) = pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);
                canvas.DrawRect(x, y, pixelMapper.CellSize, pixelMapper.CellSize,
                                new PaintBrush
                                {
                                    Style = PaintStyle.Fill,
                                    Color = BuildModeAwareColour(colour),
                                });

                var trackNeighbours = track.GetNeighbors();

                var lightColour = BuildModeAwareColour(Colors.LightGray);
                var darkColour = BuildModeAwareColour(colour);

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
            canvas.Save();
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
            canvas.Restore();
        }

        private bool IsTunnelEntrance(Track? track)
            => track is not null && !_terrainMap.GetTerrainOrDefault(track.Column, track.Row).IsMountain();

        private Color BuildModeAwareColour(Color color)
            => !_gameBoard.Enabled
                ? color.WithAlpha(BuildModeAlpha)
                : color;
    }
}
