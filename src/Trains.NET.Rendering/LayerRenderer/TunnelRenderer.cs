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
            Dictionary<(int column, int row), int> nonMountainCellEntrances = new();

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

                var tunnels = 0;
                if (IsTunnelEntrance(trackNeighbours.Up))
                {
                    tunnels += 1;

                    var key = (column: trackNeighbours.Up.Column, row: trackNeighbours.Up.Row);
                    nonMountainCellEntrances.IncrementValue(key, 4);
                }

                if (IsTunnelEntrance(trackNeighbours.Right))
                {
                    tunnels += 2;

                    var key = (column: trackNeighbours.Right.Column, row: trackNeighbours.Right.Row);
                    nonMountainCellEntrances.IncrementValue(key, 8);
                }

                if (IsTunnelEntrance(trackNeighbours.Down))
                {
                    tunnels += 4;

                    var key = (column: trackNeighbours.Down.Column, row: trackNeighbours.Down.Row);
                    nonMountainCellEntrances.IncrementValue(key, 1);
                }

                if (IsTunnelEntrance(trackNeighbours.Left))
                {
                    tunnels += 8;

                    var key = (column: trackNeighbours.Left.Column, row: trackNeighbours.Left.Row);
                    nonMountainCellEntrances.IncrementValue(key, 2);
                }

                switch (tunnels) {
                    case 0: break;
                    case 15: DrawFourWayIntersection(canvas, pixelMapper, x, y, lightColour, darkColour); break;
                    case 3:
                    case 6:
                    case 9:
                    case 12: DrawTwoWayIntersection(tunnels, canvas, pixelMapper, x, y, lightColour, darkColour); break;
                    case 1:
                    case 2:
                    case 4:
                    case 8: DrawSingleTunnel(tunnels, canvas, pixelMapper, x, y, lightColour, darkColour); break;
                    case 14:
                    case 13:
                    case 11:
                    case 7: DrawThreeWayTunnel(tunnels, canvas, pixelMapper, x, y, lightColour, darkColour); break;
                    case 5:
                    case 10: DrawStraightTunnel(tunnels, canvas, pixelMapper, x, y, lightColour, darkColour); break;
                }
            }

            var colours = new[] { darkColour, lightColour, darkColour };
            foreach (var kv in nonMountainCellEntrances)
            {
                (int col, int row) = kv.Key;
                var tunnels = kv.Value;

                (int x, int y, bool onScreen) = pixelMapper.CoordsToViewPortPixels(col, row);

                switch (tunnels)
                {
                    case 0: break;
                    case 15: DrawFourWayEntranceIntersection(canvas, pixelMapper, x, y, colours); break;
                    case 3:
                    case 6:
                    case 9:
                    case 12: DrawTwoWayEntranceIntersection(tunnels, canvas, pixelMapper, x, y, colours); break;
                    case 1:
                    case 2:
                    case 4:
                    case 8: DrawSingleEntrance(tunnels, canvas, pixelMapper, x, y, colours); break;
                    case 14:
                    case 13:
                    case 11:
                    case 7: DrawThreeWayEntrance(tunnels, canvas, pixelMapper, x, y, colours); break;
                    case 5:
                    case 10: DrawStraightEntrance(tunnels, canvas, pixelMapper, x, y, colours); break;
                }
            }
        }

        private static void DrawStraightEntrance(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = (tunnels) switch
            {
                10 => 0,
                5 => 90,
                _ => throw new System.NotImplementedException(),
            };

            canvas.Save();
            canvas.Translate(x, y);
            canvas.RotateDegrees(angle, 0.5f * pixelMapper.CellSize, 0.5f * pixelMapper.CellSize);
            canvas.GradientRect(0, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, colours);
            canvas.GradientRect(0.75f * pixelMapper.CellSize, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, colours);

            canvas.Restore();
        }

        private static void DrawThreeWayEntrance(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = (tunnels) switch
            {
                7 => 0,
                11 => 270,
                13 => 180,
                14 => 90,
                _ => throw new System.NotImplementedException(),
            };

            var cellSize = pixelMapper.CellSize;
            var quarterCellSize = 0.25f * cellSize;
            var threequarterCellSize = 0.75f * cellSize;

            canvas.Save();
            canvas.Translate(x, y);
            canvas.RotateDegrees(angle, 0.5f * pixelMapper.CellSize, 0.5f * pixelMapper.CellSize);

            canvas.GradientRectLeftRight(0, 0, cellSize, quarterCellSize, colours);
            canvas.GradientRect(threequarterCellSize, 0, quarterCellSize, cellSize, colours);
            canvas.GradientRectLeftRight(0, threequarterCellSize, cellSize, quarterCellSize, colours);
            canvas.GradientCircle(threequarterCellSize, 0, quarterCellSize, quarterCellSize, cellSize, 0, cellSize, colours);
            canvas.GradientCircle(threequarterCellSize, threequarterCellSize, quarterCellSize, quarterCellSize, cellSize, cellSize, cellSize, colours);

            canvas.Restore();
        }

        private static void DrawFourWayEntranceIntersection(ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var cellSize = pixelMapper.CellSize;
            var quarterCellSize = 0.25f * cellSize;
            var threequarterCellSize = 0.75f * cellSize;

            canvas.Save();
            canvas.Translate(x, y);
            canvas.GradientRect(0, 0, quarterCellSize, cellSize, colours);
            canvas.GradientRectLeftRight(0, 0, cellSize, quarterCellSize, colours);
            canvas.GradientRect(0, threequarterCellSize, quarterCellSize, cellSize, colours);
            canvas.GradientRectLeftRight(0, threequarterCellSize, cellSize, quarterCellSize, colours);
            canvas.GradientCircle(0, 0, quarterCellSize, quarterCellSize, 0, 0, cellSize, colours);
            canvas.GradientCircle(threequarterCellSize, 0, quarterCellSize, quarterCellSize, cellSize, 0, cellSize, colours);
            canvas.GradientCircle(threequarterCellSize, threequarterCellSize, quarterCellSize, quarterCellSize, cellSize, cellSize, cellSize, colours);
            canvas.GradientCircle(0, threequarterCellSize, quarterCellSize, quarterCellSize, 0, cellSize, cellSize, colours);

            canvas.Restore();
        }

        private static void DrawSingleEntrance(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = (tunnels) switch
            {
                8 => 0,
                1 => 90,
                2 => 180,
                4 => 270,
                _ => throw new System.NotImplementedException()
            };

            canvas.Save();
            canvas.Translate(x, y);
            canvas.RotateDegrees(angle, 0.5f * pixelMapper.CellSize, 0.5f * pixelMapper.CellSize);
            canvas.GradientRect(0, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, colours);

            canvas.Restore();
        }

        private static void DrawTwoWayEntranceIntersection(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = (tunnels) switch
            {
                3 => 90,
                6 => 180,
                9 => 0,
                12 => 270,
                _ => throw new System.NotImplementedException()
            };

            var cellSize = pixelMapper.CellSize;
            var halfCellSize = 0.5f * cellSize;
            var quarterCellSize = 0.25f * cellSize;

            canvas.Save();
            canvas.Translate(x, y);
            canvas.RotateDegrees(angle, halfCellSize, halfCellSize);
            canvas.GradientRect(0, 0, quarterCellSize, cellSize, colours);
            canvas.GradientRectLeftRight(0, 0, cellSize, quarterCellSize, colours);
            canvas.GradientCircle(0, 0, quarterCellSize, quarterCellSize, 0, 0, cellSize, colours);

            canvas.Restore();
        }

        private static void DrawStraightTunnel(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = (tunnels) switch
            {
                10 => 0,
                5 => 90,
                _ => throw new System.NotImplementedException(),
            };

            var cellSize = pixelMapper.CellSize;
            var halfCellSize = 0.5f * pixelMapper.CellSize;

            canvas.Save();
            canvas.Translate(x, y);

            canvas.RotateDegrees(angle, halfCellSize, halfCellSize);

            canvas.GradientRect(0, 0, cellSize, cellSize, darkColour, lightColour);
            canvas.Restore();
        }

        private static void DrawThreeWayTunnel(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = (tunnels) switch
            {
                14 => 0,
                13 => 90,
                11 => 180,
                7 => 270,
                _ => throw new System.NotImplementedException()
            };

            var cellSize = pixelMapper.CellSize;
            var halfCellSize = 0.5f * pixelMapper.CellSize;


            canvas.Save();
            canvas.Translate(x, y);

            canvas.RotateDegrees(angle, halfCellSize, halfCellSize);


            canvas.GradientRect(0, 0, cellSize, halfCellSize, new Color[] { darkColour, lightColour });
            canvas.GradientCircle(0, halfCellSize, halfCellSize, halfCellSize, 0, cellSize, halfCellSize, new[] { darkColour, lightColour });
            canvas.GradientCircle(halfCellSize, halfCellSize, halfCellSize, halfCellSize, cellSize, cellSize, halfCellSize, new[] { darkColour, lightColour });
            canvas.Restore();
        }

        private static void DrawSingleTunnel(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = (tunnels) switch
            {
                8 => 0,
                1 => 90,
                2 => 180,
                4 => 270,
                _ => throw new System.NotImplementedException()
            };

            var cellSize = pixelMapper.CellSize;
            var halfCellSize = 0.5f * pixelMapper.CellSize;


            canvas.Save();
            canvas.Translate(x, y);

            canvas.RotateDegrees(angle, halfCellSize, halfCellSize);

            canvas.GradientCircle(0, 0, cellSize, cellSize, 0, halfCellSize, halfCellSize, new[] { lightColour, darkColour });
            canvas.Restore();
        }

        private static void DrawTwoWayIntersection(int tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = (tunnels / 3) switch
            {
                1 => 90,
                2 => 180,
                3 => 0,
                4 => 270,
                _ => throw new System.NotImplementedException()
            };

            var cellSize = pixelMapper.CellSize;
            var halfCellSize = 0.5f * pixelMapper.CellSize;

            var gradientColours = new List<Color> { darkColour, lightColour, darkColour };

            canvas.Save();
            canvas.Translate(x, y);

            canvas.RotateDegrees(angle, halfCellSize, halfCellSize);

            canvas.GradientCircle(0, 0, cellSize, cellSize, 0, 0, cellSize, gradientColours);

            canvas.Restore();
        }

        private static void DrawFourWayIntersection(ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            canvas.Save();
            canvas.Translate(x, y);

            var cellSize = pixelMapper.CellSize;
            var halfCellSize = 0.5f * pixelMapper.CellSize;

            var gradientColours = new List<Color> { darkColour, lightColour };
            canvas.GradientCircle(0, 0, halfCellSize, halfCellSize, 0, 0, halfCellSize, gradientColours); // Top left
            canvas.GradientCircle(halfCellSize, 0, halfCellSize, halfCellSize, cellSize, 0, halfCellSize, gradientColours); // Top right
            canvas.GradientCircle(halfCellSize, halfCellSize, halfCellSize, halfCellSize, cellSize, cellSize, halfCellSize, gradientColours); // Bottom right
            canvas.GradientCircle(0, halfCellSize, halfCellSize, halfCellSize, 0, cellSize, halfCellSize, gradientColours); // Bottom left

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
