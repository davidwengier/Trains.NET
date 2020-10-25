using System;
using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(700)]
    public class TunnelRenderer : ILayerRenderer
    {
        private const int BuildModeAlpha = 170;

        private readonly ITerrainMap _terrainMap;
        private readonly ILayout<Track> _trackLayout;
        private readonly IGameBoard _gameBoard;

        private enum Tunnel
        {
            NoTunnels = 0,
            Top = 1,
            Right = 2,
            Bottom = 4,
            Left = 8,

            TopRightCorner = Top | Right,
            BottomRightCorner = Bottom | Right,
            TopLeftCorner = Top | Left,
            BottomLeftCorner = Bottom | Left,

            StraightHorizontal = Left | Right,
            StraightVertical = Top | Bottom,

            ThreewayNoLeft = Right | Top | Bottom,
            ThreewayNoUp = Left | Bottom | Right,
            ThreewayNoRight = Left | Top | Bottom,
            ThreewayNoDown = Left | Top | Right,

            FourWayIntersection = Left | Top | Bottom | Right
        }

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
            Dictionary<(int column, int row), Tunnel> neighbourTunnels = new();

            var lightColour = BuildModeAwareColour(Colors.LightGray);
            var darkColour = BuildModeAwareColour(Colors.Gray);

            foreach (Track track in _trackLayout)
            {
                Terrain terrain = _terrainMap.Get(track.Column, track.Row);
                if (!terrain.IsMountain) continue;

                (int x, int y, _) = pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);
                canvas.DrawRect(x, y, pixelMapper.CellSize, pixelMapper.CellSize,
                                new PaintBrush
                                {
                                    Style = PaintStyle.Fill,
                                    Color = darkColour,
                                });

                TrackNeighbors trackNeighbours = track.GetConnectedNeighbors();

                var currentCellTunnels = Tunnel.NoTunnels;
                var neighbourTrackConfigs = new List<(Track? neighbourTrack, Tunnel currentCellTunnel, Tunnel neighbourTunnel)>
                {
                    (trackNeighbours.Up, Tunnel.Top, Tunnel.Bottom),
                    (trackNeighbours.Right, Tunnel.Right,Tunnel. Left),
                    (trackNeighbours.Down, Tunnel.Bottom, Tunnel.Top),
                    (trackNeighbours.Left, Tunnel.Left, Tunnel.Right)
                };

                foreach (var neighbourTrackConfig in neighbourTrackConfigs)
                {
                    if (!IsTunnelEntrance(neighbourTrackConfig.neighbourTrack)) continue;

                    currentCellTunnels += (int)neighbourTrackConfig.currentCellTunnel;
                    var neighbourTrack = neighbourTrackConfig.neighbourTrack is not null
                        ? neighbourTrackConfig.neighbourTrack
                        : new Track();
                    var key = (neighbourTrack.Column, neighbourTrack.Row);


                    neighbourTunnels.TryGetValue(key, out var currentValue);
                    currentValue += (int)neighbourTrackConfig.neighbourTunnel;
                    neighbourTunnels[key] = currentValue;
                }

                switch (currentCellTunnels)
                {
                    case Tunnel.Top:
                    case Tunnel.Right:
                    case Tunnel.Bottom:
                    case Tunnel.Left:
                    {
                        DrawSingleTunnel(currentCellTunnels, canvas, pixelMapper, x, y, lightColour, darkColour);
                        break;
                    }
                    case Tunnel.StraightVertical:
                    case Tunnel.StraightHorizontal:
                    {
                        DrawStraightTunnel(currentCellTunnels, canvas, pixelMapper, x, y, lightColour, darkColour);
                        break;
                    }
                    case Tunnel.TopRightCorner:
                    case Tunnel.BottomRightCorner:
                    case Tunnel.TopLeftCorner:
                    case Tunnel.BottomLeftCorner:
                    {
                        DrawCornerIntersection(currentCellTunnels, canvas, pixelMapper, x, y, lightColour, darkColour);
                        break;
                    }
                    case Tunnel.ThreewayNoUp:
                    case Tunnel.ThreewayNoRight:
                    case Tunnel.ThreewayNoDown:
                    case Tunnel.ThreewayNoLeft:
                    {
                        DrawThreeWayTunnel(currentCellTunnels, canvas, pixelMapper, x, y, lightColour, darkColour);
                        break;
                    }
                    case Tunnel.FourWayIntersection:
                    {
                        DrawFourWayIntersection(canvas, pixelMapper, x, y, lightColour, darkColour);
                        break;
                    }
                }
            }

            var colours = new[] { darkColour, lightColour, darkColour };
            foreach (var kv in neighbourTunnels)
            {
                (int col, int row) = kv.Key;
                var tunnels = kv.Value;

                (int x, int y, _) = pixelMapper.CoordsToViewPortPixels(col, row);

                switch (tunnels)
                {
                    case Tunnel.Top:
                    case Tunnel.Right:
                    case Tunnel.Bottom:
                    case Tunnel.Left:
                    {
                        DrawSingleEntrance(tunnels, canvas, pixelMapper, x, y, colours);
                        break;
                    }
                    case Tunnel.StraightVertical:
                    case Tunnel.StraightHorizontal:
                    {
                        DrawStraightEntrance(tunnels, canvas, pixelMapper, x, y, colours);
                        break;
                    }
                    case Tunnel.TopRightCorner:
                    case Tunnel.BottomRightCorner:
                    case Tunnel.TopLeftCorner:
                    case Tunnel.BottomLeftCorner:
                    {
                        DrawTwoWayEntranceIntersection(tunnels, canvas, pixelMapper, x, y, colours);
                        break;
                    }
                    case Tunnel.ThreewayNoUp:
                    case Tunnel.ThreewayNoRight:
                    case Tunnel.ThreewayNoDown:
                    case Tunnel.ThreewayNoLeft:
                    {
                        DrawThreeWayEntrance(tunnels, canvas, pixelMapper, x, y, colours);
                        break;
                    }
                    case Tunnel.FourWayIntersection:
                    {
                        DrawFourWayEntranceIntersection(canvas, pixelMapper, x, y, colours);
                        break;
                    }

                }
            }
        }

        private static void DrawStraightEntrance(Tunnel tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = tunnels switch
            {
                Tunnel.StraightHorizontal => 0,
                Tunnel.StraightVertical => 90,
                _ => throw new NotImplementedException(),
            };

            DrawInCell(x, y, canvas, DrawAction, angle, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                c.DrawVerticalGradientRect(0, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, colours);
                c.DrawVerticalGradientRect(0.75f * pixelMapper.CellSize, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, colours);
            }
        }

        private static void DrawThreeWayEntrance(Tunnel tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = tunnels switch
            {
                Tunnel.ThreewayNoLeft => 0,
                Tunnel.ThreewayNoUp => 90,
                Tunnel.ThreewayNoRight => 180,
                Tunnel.ThreewayNoDown => 270,
                _ => throw new NotImplementedException()
            };

            DrawInCell(x, y, canvas, DrawAction, angle, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                var cellSize = pixelMapper.CellSize;
                var quarterCellSize = 0.25f * cellSize;
                var threequarterCellSize = 0.75f * cellSize;

                c.DrawHorizontalGradientRect(0, 0, cellSize, quarterCellSize, colours);
                c.DrawVerticalGradientRect(threequarterCellSize, 0, quarterCellSize, cellSize, colours);
                c.DrawHorizontalGradientRect(0, threequarterCellSize, cellSize, quarterCellSize, colours);
                c.DrawGradientCircle(threequarterCellSize, 0, quarterCellSize, quarterCellSize, cellSize, 0, cellSize, colours);
                c.DrawGradientCircle(threequarterCellSize, threequarterCellSize, quarterCellSize, quarterCellSize, cellSize, cellSize, cellSize, colours);
            }
        }

        private static void DrawFourWayEntranceIntersection(ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            DrawInCell(x, y, canvas, DrawAction, 0, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                var cellSize = pixelMapper.CellSize;
                var quarterCellSize = 0.25f * cellSize;
                var threequarterCellSize = 0.75f * cellSize;

                c.DrawVerticalGradientRect(0, 0, quarterCellSize, cellSize, colours);
                c.DrawHorizontalGradientRect(0, 0, cellSize, quarterCellSize, colours);
                c.DrawVerticalGradientRect(0, threequarterCellSize, quarterCellSize, cellSize, colours);
                c.DrawHorizontalGradientRect(0, threequarterCellSize, cellSize, quarterCellSize, colours);
                c.DrawGradientCircle(0, 0, quarterCellSize, quarterCellSize, 0, 0, cellSize, colours);
                c.DrawGradientCircle(threequarterCellSize, 0, quarterCellSize, quarterCellSize, cellSize, 0, cellSize, colours);
                c.DrawGradientCircle(threequarterCellSize, threequarterCellSize, quarterCellSize, quarterCellSize, cellSize, cellSize, cellSize, colours);
                c.DrawGradientCircle(0, threequarterCellSize, quarterCellSize, quarterCellSize, 0, cellSize, cellSize, colours);
            }
        }

        private static void DrawSingleEntrance(Tunnel tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = (tunnels) switch
            {
                Tunnel.Left => 0,
                Tunnel.Top => 90,
                Tunnel.Right => 180,
                Tunnel.Bottom => 270,
                _ => throw new NotImplementedException()
            };

            DrawInCell(x, y, canvas, DrawAction, angle, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                c.DrawVerticalGradientRect(0, 0, 0.25f * pixelMapper.CellSize, pixelMapper.CellSize, colours);
            }
        }

        private static void DrawTwoWayEntranceIntersection(Tunnel tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color[] colours)
        {
            var angle = tunnels switch
            {
                Tunnel.TopLeftCorner => 0,
                Tunnel.TopRightCorner => 90,
                Tunnel.BottomRightCorner => 180,
                Tunnel.BottomLeftCorner => 270,
                _ => throw new NotImplementedException()
            };

            DrawInCell(x, y, canvas, DrawAction, angle, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * cellSize;
                var quarterCellSize = 0.25f * cellSize;

                c.DrawVerticalGradientRect(0, 0, quarterCellSize, cellSize, colours);
                c.DrawHorizontalGradientRect(0, 0, cellSize, quarterCellSize, colours);
                c.DrawGradientCircle(0, 0, quarterCellSize, quarterCellSize, 0, 0, cellSize, colours);
            }
        }

        private static void DrawStraightTunnel(Tunnel tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = tunnels switch
            {
                Tunnel.StraightHorizontal => 0,
                Tunnel.StraightVertical => 90,
                _ => throw new NotImplementedException(),
            };

            DrawInCell(x, y, canvas, DrawAction, angle, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                c.DrawGradientRect(0, 0, cellSize, cellSize, darkColour, lightColour);
            }
        }

        private static void DrawThreeWayTunnel(Tunnel tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = tunnels switch
            {
                Tunnel.ThreewayNoUp => 0,
                Tunnel.ThreewayNoRight => 90,
                Tunnel.ThreewayNoDown => 180,
                Tunnel.ThreewayNoLeft => 270,
                _ => throw new NotImplementedException()
            };

            DrawInCell(x, y, canvas, DrawAction, angle, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                c.DrawVerticalGradientRect(0, 0, cellSize, halfCellSize, new Color[] { darkColour, lightColour });
                c.DrawGradientCircle(0, halfCellSize, halfCellSize, halfCellSize, 0, cellSize, halfCellSize, new[] { darkColour, lightColour });
                c.DrawGradientCircle(halfCellSize, halfCellSize, halfCellSize, halfCellSize, cellSize, cellSize, halfCellSize, new[] { darkColour, lightColour });
            }
        }

        private static void DrawSingleTunnel(Tunnel tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = tunnels switch
            {
                Tunnel.Left => 0,
                Tunnel.Top => 90,
                Tunnel.Right => 180,
                Tunnel.Bottom => 270,
                _ => throw new NotImplementedException()
            };

            DrawInCell(x, y, canvas, DrawAction, angle, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                c.DrawGradientCircle(0, 0, cellSize, cellSize, 0, halfCellSize, halfCellSize, new[] { lightColour, darkColour });
            }
        }

        private static void DrawCornerIntersection(Tunnel tunnels, ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            var angle = tunnels switch
            {
                Tunnel.TopLeftCorner => 0,
                Tunnel.TopRightCorner => 90,
                Tunnel.BottomRightCorner => 180,
                Tunnel.BottomLeftCorner => 270,
                _ => throw new NotImplementedException()
            };

            DrawInCell(x, y, canvas, DrawAction, angle, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                var gradientColours = new List<Color> { darkColour, lightColour, darkColour };

                c.DrawGradientCircle(0, 0, cellSize, cellSize, 0, 0, cellSize, gradientColours);
            }
        }

        private static void DrawFourWayIntersection(ICanvas canvas, IPixelMapper pixelMapper, int x, int y, Color lightColour, Color darkColour)
        {
            DrawInCell(x, y, canvas, DrawAction, 0, 0.5f * pixelMapper.CellSize);

            void DrawAction(ICanvas c)
            {
                var cellSize = pixelMapper.CellSize;
                var halfCellSize = 0.5f * pixelMapper.CellSize;

                var gradientColours = new List<Color> { darkColour, lightColour };

                c.DrawGradientCircle(0, 0, halfCellSize, halfCellSize, 0, 0, halfCellSize, gradientColours); // Top left
                c.DrawGradientCircle(halfCellSize, 0, halfCellSize, halfCellSize, cellSize, 0, halfCellSize, gradientColours); // Top right
                c.DrawGradientCircle(halfCellSize, halfCellSize, halfCellSize, halfCellSize, cellSize, cellSize, halfCellSize, gradientColours); // Bottom right
                c.DrawGradientCircle(0, halfCellSize, halfCellSize, halfCellSize, 0, cellSize, halfCellSize, gradientColours); // Bottom left
            }
        }

        private static void DrawInCell(int x, int y, ICanvas canvas, Action<ICanvas> drawAction, float angle, float halfCellSize)
        {
            using (canvas.Scope())
            {
                canvas.Translate(x, y);
                canvas.RotateDegrees(angle, halfCellSize, halfCellSize);
                drawAction(canvas);
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
