using System;
using System.Collections.Generic;
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

        private const int NO_TUNNELS = 0;
        private const int TOP = 1;
        private const int RIGHT = 2;
        private const int BOTTOM = 4;
        private const int LEFT = 8;

        private const int TOPRIGHT_CORNER = 3;
        private const int BOTTOMRIGHT_CORNER = 6;
        private const int TOPLEFT_CORNER = 9;
        private const int BOTTOMLEFT_CORNER = 12;

        private const int STRAIGHT_HORIZONTAL = 10;
        private const int STRAIGHT_VERTICAL = 5;

        private const int THREEWAY_NO_LEFT = 7;
        private const int THREEWAY_NO_UP = 14;
        private const int THREEWAY_NO_RIGHT = 13;
        private const int THREEWAY_NO_DOWN = 11;

        private const int FOUR_WAY = 15;

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
            Dictionary<(int column, int row), int> neighbourTunnels = new();

            var lightColour = BuildModeAwareColour(Colors.LightGray);
            var darkColour = BuildModeAwareColour(Colors.Gray);

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
                                    Color = darkColour,
                                });

                TrackNeighbors trackNeighbours = track.GetNeighbors();

                var currentCellTunnels = NO_TUNNELS;
                var neighbourTrackConfigs = new List<(Track neighbourTrack, int currentCellTunnel, int neighbourTunnel)>
                {
                    (trackNeighbours.Up, TOP, BOTTOM),
                    (trackNeighbours.Right, RIGHT, LEFT),
                    (trackNeighbours.Down, BOTTOM, TOP),
                    (trackNeighbours.Left, LEFT, RIGHT)
                };

                foreach (var neighbourTrackConfig in neighbourTrackConfigs)
                {
                    if (!IsTunnelEntrance(neighbourTrackConfig.neighbourTrack)) continue;

                    currentCellTunnels += neighbourTrackConfig.currentCellTunnel;
                    var key = (column: neighbourTrackConfig.neighbourTrack.Column, row: neighbourTrackConfig.neighbourTrack.Row);
                    neighbourTunnels.IncrementValue(key, neighbourTrackConfig.neighbourTunnel);
                }

                switch (currentCellTunnels) {
                    case NO_TUNNELS: break;
                    case TOP:
                    case RIGHT:
                    case BOTTOM:
                    case LEFT: DrawSingleTunnel(currentCellTunnels, canvas, pixelMapper, x, y, lightColour, darkColour); break;
                    case STRAIGHT_VERTICAL:
                    case STRAIGHT_HORIZONTAL: DrawStraightTunnel(currentCellTunnels, canvas, pixelMapper, x, y, lightColour, darkColour); break;
                    case TOPRIGHT_CORNER:
                    case BOTTOMRIGHT_CORNER:
                    case TOPLEFT_CORNER:
                    case BOTTOMLEFT_CORNER: DrawCornerIntersection(currentCellTunnels, canvas, pixelMapper, x, y, lightColour, darkColour); break;
                    case THREEWAY_NO_UP:
                    case THREEWAY_NO_RIGHT:
                    case THREEWAY_NO_DOWN:
                    case THREEWAY_NO_LEFT: DrawThreeWayTunnel(currentCellTunnels, canvas, pixelMapper, x, y, lightColour, darkColour); break;
                    case FOUR_WAY: DrawFourWayIntersection(canvas, pixelMapper, x, y, lightColour, darkColour); break;

                }
            }

            var colours = new[] { darkColour, lightColour, darkColour };
            foreach (var kv in neighbourTunnels)
            {
                (int col, int row) = kv.Key;
                var tunnels = kv.Value;

                (int x, int y, bool onScreen) = pixelMapper.CoordsToViewPortPixels(col, row);

                switch (tunnels)
                {
                    case NO_TUNNELS: break;
                    case TOP:
                    case RIGHT:
                    case BOTTOM:
                    case LEFT: DrawSingleEntrance(tunnels, canvas, pixelMapper, x, y, colours); break;
                    case STRAIGHT_VERTICAL:
                    case STRAIGHT_HORIZONTAL: DrawStraightEntrance(tunnels, canvas, pixelMapper, x, y, colours); break;
                    case TOPRIGHT_CORNER:
                    case BOTTOMRIGHT_CORNER:
                    case TOPLEFT_CORNER:
                    case BOTTOMLEFT_CORNER: DrawTwoWayEntranceIntersection(tunnels, canvas, pixelMapper, x, y, colours); break;
                    case THREEWAY_NO_UP:
                    case THREEWAY_NO_RIGHT:
                    case THREEWAY_NO_DOWN:
                    case THREEWAY_NO_LEFT: DrawThreeWayEntrance(tunnels, canvas, pixelMapper, x, y, colours); break;
                    case FOUR_WAY: DrawFourWayEntranceIntersection(canvas, pixelMapper, x, y, colours); break;

                }
            }
        }

        private static void DrawStraightEntrance(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = (tunnels) switch
            {
                STRAIGHT_HORIZONTAL => 0,
                STRAIGHT_VERTICAL => 90,
                _ => throw new NotImplementedException(),
            };

            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                c.GradientRect(0, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, colours);
                c.GradientRect(0.75f * pixelMapper.CellSize, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, colours);
            };

            DrawInCell(x, y, canvas, drawAction, angle, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawThreeWayEntrance(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = (tunnels) switch
            {
                THREEWAY_NO_LEFT => 0,
                THREEWAY_NO_UP => 90,
                THREEWAY_NO_RIGHT => 180,
                THREEWAY_NO_DOWN => 270,
                _ => throw new NotImplementedException()
            };

            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                var cellSize = pixelMapper.CellSize;
                var quarterCellSize = 0.25f * cellSize;
                var threequarterCellSize = 0.75f * cellSize;

                c.GradientRectLeftRight(0, 0, cellSize, quarterCellSize, colours);
                c.GradientRect(threequarterCellSize, 0, quarterCellSize, cellSize, colours);
                c.GradientRectLeftRight(0, threequarterCellSize, cellSize, quarterCellSize, colours);
                c.GradientCircle(threequarterCellSize, 0, quarterCellSize, quarterCellSize, cellSize, 0, cellSize, colours);
                c.GradientCircle(threequarterCellSize, threequarterCellSize, quarterCellSize, quarterCellSize, cellSize, cellSize, cellSize, colours);
            };

            DrawInCell(x, y, canvas, drawAction, angle, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawFourWayEntranceIntersection(ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                var cellSize = pixelMapper.CellSize;
                var quarterCellSize = 0.25f * cellSize;
                var threequarterCellSize = 0.75f * cellSize;

                c.GradientRect(0, 0, quarterCellSize, cellSize, colours);
                c.GradientRectLeftRight(0, 0, cellSize, quarterCellSize, colours);
                c.GradientRect(0, threequarterCellSize, quarterCellSize, cellSize, colours);
                c.GradientRectLeftRight(0, threequarterCellSize, cellSize, quarterCellSize, colours);
                c.GradientCircle(0, 0, quarterCellSize, quarterCellSize, 0, 0, cellSize, colours);
                c.GradientCircle(threequarterCellSize, 0, quarterCellSize, quarterCellSize, cellSize, 0, cellSize, colours);
                c.GradientCircle(threequarterCellSize, threequarterCellSize, quarterCellSize, quarterCellSize, cellSize, cellSize, cellSize, colours);
                c.GradientCircle(0, threequarterCellSize, quarterCellSize, quarterCellSize, 0, cellSize, cellSize, colours);
            };

            DrawInCell(x, y, canvas, drawAction, 0, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawSingleEntrance(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = (tunnels) switch
            {
                LEFT => 0,
                TOP => 90,
                RIGHT => 180,
                BOTTOM => 270,
                _ => throw new NotImplementedException()
            };

            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                c.GradientRect(0, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, colours);
            };

            DrawInCell(x, y, canvas, drawAction, angle, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawTwoWayEntranceIntersection(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = (tunnels) switch
            {
                TOPLEFT_CORNER => 0,
                TOPRIGHT_CORNER => 90,
                BOTTOMRIGHT_CORNER => 180,
                BOTTOMLEFT_CORNER => 270,
                _ => throw new NotImplementedException()
            };

            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * cellSize;
                var quarterCellSize = 0.25f * cellSize;

                c.GradientRect(0, 0, quarterCellSize, cellSize, colours);
                c.GradientRectLeftRight(0, 0, cellSize, quarterCellSize, colours);
                c.GradientCircle(0, 0, quarterCellSize, quarterCellSize, 0, 0, cellSize, colours);
            };

            DrawInCell(x, y, canvas, drawAction, angle, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawStraightTunnel(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = (tunnels) switch
            {
                STRAIGHT_HORIZONTAL => 0,
                STRAIGHT_VERTICAL => 90,
                _ => throw new NotImplementedException(),
            };

            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                c.GradientRect(0, 0, cellSize, cellSize, darkColour, lightColour);
            };

            DrawInCell(x, y, canvas, drawAction, angle, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawThreeWayTunnel(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = (tunnels) switch
            {
                THREEWAY_NO_UP => 0,
                THREEWAY_NO_RIGHT => 90,
                THREEWAY_NO_DOWN => 180,
                THREEWAY_NO_LEFT => 270,
                _ => throw new NotImplementedException()
            };

            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                c.GradientRect(0, 0, cellSize, halfCellSize, new Color[] { darkColour, lightColour });
                c.GradientCircle(0, halfCellSize, halfCellSize, halfCellSize, 0, cellSize, halfCellSize, new[] { darkColour, lightColour });
                c.GradientCircle(halfCellSize, halfCellSize, halfCellSize, halfCellSize, cellSize, cellSize, halfCellSize, new[] { darkColour, lightColour });
            };

            DrawInCell(x, y, canvas, drawAction, angle, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawSingleTunnel(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = (tunnels) switch
            {
                LEFT => 0,
                TOP => 90,
                RIGHT => 180,
                BOTTOM => 270,
                _ => throw new NotImplementedException()
            };

            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                c.GradientCircle(0, 0, cellSize, cellSize, 0, halfCellSize, halfCellSize, new[] { lightColour, darkColour });
            };

            DrawInCell(x, y, canvas, drawAction, angle, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawCornerIntersection(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = (tunnels) switch
            {
                TOPLEFT_CORNER => 0,
                TOPRIGHT_CORNER => 90,
                BOTTOMRIGHT_CORNER => 180,
                BOTTOMLEFT_CORNER => 270,
                _ => throw new NotImplementedException()
            };

            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                var gradientColours = new List<Color> { darkColour, lightColour, darkColour };

                c.GradientCircle(0, 0, cellSize, cellSize, 0, 0, cellSize, gradientColours);
            };

            DrawInCell(x, y, canvas, drawAction, angle, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawFourWayIntersection(ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            Action<ICanvas> drawAction = (ICanvas c) =>
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                var gradientColours = new List<Color> { darkColour, lightColour };

                c.GradientCircle(0, 0, halfCellSize, halfCellSize, 0, 0, halfCellSize, gradientColours); // Top left
                c.GradientCircle(halfCellSize, 0, halfCellSize, halfCellSize, cellSize, 0, halfCellSize, gradientColours); // Top right
                c.GradientCircle(halfCellSize, halfCellSize, halfCellSize, halfCellSize, cellSize, cellSize, halfCellSize, gradientColours); // Bottom right
                c.GradientCircle(0, halfCellSize, halfCellSize, halfCellSize, 0, cellSize, halfCellSize, gradientColours); // Bottom left
            };

            DrawInCell(x, y, canvas, drawAction, 0, 0.5f * pixelMapper.CellSize);
        }

        private static void DrawInCell(int x, int y, ICanvas canvas, Action<ICanvas> drawAction, float angle, float halfCellSize)
        {
            canvas.Save();
            canvas.Translate(x, y);
            canvas.RotateDegrees(angle, halfCellSize, halfCellSize);
            drawAction(canvas);
            canvas.Restore();
        }

        private bool IsTunnelEntrance(Track? track)
            => track is not null && !_terrainMap.Get(track.Column, track.Row).IsMountain;

        private Color BuildModeAwareColour(Color color)
            => !_gameBoard.Enabled
                ? color with { A = BuildModeAlpha }
                : color;
    }

    internal static class CellDictionaryExtensions
    {
        public static void IncrementValue(this Dictionary<(int column, int row), int> cells, (int column, int row) key, int value)
        {
            cells.TryGetValue(key, out var currentValue);
            currentValue += value;
            cells[key] = currentValue;
        }

    }
}
