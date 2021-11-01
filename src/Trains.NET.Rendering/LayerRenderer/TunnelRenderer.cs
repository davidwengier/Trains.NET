using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(700)]
public partial class TunnelRenderer : ILayerRenderer
{
    private const int BuildModeAlpha = 170;

    private readonly ITerrainMap _terrainMap;
    private readonly ILayout<Track> _trackLayout;
    private readonly IGameBoard _gameBoard;

    public bool Enabled { get; set; } = true;
    public string Name => "Tunnels";

    public TunnelRenderer(ITerrainMap terrainMap, ILayout<Track> layout, IGameBoard gameBoard)
    {
        _terrainMap = terrainMap;
        _trackLayout = layout;
        _gameBoard = gameBoard;
    }

    public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
    {
        var tunnelRoofColour = BuildModeAwareColour(Colors.LightGray);
        var firstMountain = new Terrain()
        {
            Height = Terrain.FirstMountainHeight
        };
        var tunnelBaseColour = BuildModeAwareColour(TerrainMapRenderer.GetTerrainColour(firstMountain));
        var entranceColourArray = new[] { tunnelBaseColour, tunnelRoofColour, tunnelBaseColour };

        Dictionary<(int column, int row), Tunnel> entrances = new();

        foreach (Track track in _trackLayout)
        {
            var terrain = _terrainMap.Get(track.Column, track.Row);
            if (!terrain.IsMountain) continue;

            (int x, int y, _) = pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);

            // Paint over the tracks with the colour of the terrain. Would be awesome to remove this in future somehow
            var terrainColour = BuildModeAwareColour(TerrainMapRenderer.GetTerrainColour(terrain));
            canvas.DrawRect(x, y, pixelMapper.CellSize, pixelMapper.CellSize,
                            new PaintBrush
                            {
                                Style = PaintStyle.Fill,
                                Color = terrainColour,
                            });

            var trackNeighbours = TrackNeighbors.GetConnectedNeighbours(_trackLayout.ToLayout(), track.Column, track.Row, false, false);

            BuildEntrances(trackNeighbours.Up, Tunnel.Bottom, entrances);
            BuildEntrances(trackNeighbours.Right, Tunnel.Left, entrances);
            BuildEntrances(trackNeighbours.Down, Tunnel.Top, entrances);
            BuildEntrances(trackNeighbours.Left, Tunnel.Right, entrances);

            var currentCellTunnels = (IsEntrance(trackNeighbours.Up) ? Tunnel.Top : Tunnel.NoTunnels) |
                                     (IsEntrance(trackNeighbours.Right) ? Tunnel.Right : Tunnel.NoTunnels) |
                                     (IsEntrance(trackNeighbours.Down) ? Tunnel.Bottom : Tunnel.NoTunnels) |
                                     (IsEntrance(trackNeighbours.Left) ? Tunnel.Left : Tunnel.NoTunnels);

            DrawTunnel(canvas, pixelMapper, tunnelRoofColour, tunnelBaseColour, x, y, currentCellTunnels);
        }

        foreach (var (col, row, tunnels) in entrances)
        {
            DrawEntrance(canvas, pixelMapper, entranceColourArray, col, row, tunnels);
        }
    }

    private static void DrawTunnel(ICanvas canvas, IPixelMapper pixelMapper, Color tunnelRoofColour, Color tunnelBaseColour, int x, int y, Tunnel currentCellTunnels)
    {
        switch (currentCellTunnels)
        {
            case Tunnel.Top:
            case Tunnel.Right:
            case Tunnel.Bottom:
            case Tunnel.Left:
            {
                DrawSingleTunnel(currentCellTunnels, canvas, pixelMapper, x, y, tunnelRoofColour, tunnelBaseColour);
                break;
            }
            case Tunnel.StraightVertical:
            case Tunnel.StraightHorizontal:
            {
                DrawStraightTunnel(currentCellTunnels, canvas, pixelMapper, x, y, tunnelRoofColour, tunnelBaseColour);
                break;
            }
            case Tunnel.TopRightCorner:
            case Tunnel.BottomRightCorner:
            case Tunnel.TopLeftCorner:
            case Tunnel.BottomLeftCorner:
            {
                DrawCornerIntersection(currentCellTunnels, canvas, pixelMapper, x, y, tunnelRoofColour, tunnelBaseColour);
                break;
            }
            case Tunnel.ThreewayNoUp:
            case Tunnel.ThreewayNoRight:
            case Tunnel.ThreewayNoDown:
            case Tunnel.ThreewayNoLeft:
            {
                DrawThreeWayTunnel(currentCellTunnels, canvas, pixelMapper, x, y, tunnelRoofColour, tunnelBaseColour);
                break;
            }
            case Tunnel.FourWayIntersection:
            {
                DrawFourWayIntersection(canvas, pixelMapper, x, y, tunnelRoofColour, tunnelBaseColour);
                break;
            }
        }
    }

    private static void DrawEntrance(ICanvas canvas, IPixelMapper pixelMapper, Color[] entranceColourArray, int col, int row, Tunnel tunnels)
    {
        (int x, int y, _) = pixelMapper.CoordsToViewPortPixels(col, row);

        switch (tunnels)
        {
            case Tunnel.Top:
            case Tunnel.Right:
            case Tunnel.Bottom:
            case Tunnel.Left:
            {
                DrawSingleEntrance(tunnels, canvas, pixelMapper, x, y, entranceColourArray);
                break;
            }
            case Tunnel.StraightVertical:
            case Tunnel.StraightHorizontal:
            {
                DrawStraightEntrance(tunnels, canvas, pixelMapper, x, y, entranceColourArray);
                break;
            }
            case Tunnel.TopRightCorner:
            case Tunnel.BottomRightCorner:
            case Tunnel.TopLeftCorner:
            case Tunnel.BottomLeftCorner:
            {
                DrawTwoWayEntranceIntersection(tunnels, canvas, pixelMapper, x, y, entranceColourArray);
                break;
            }
            case Tunnel.ThreewayNoUp:
            case Tunnel.ThreewayNoRight:
            case Tunnel.ThreewayNoDown:
            case Tunnel.ThreewayNoLeft:
            {
                DrawThreeWayEntrance(tunnels, canvas, pixelMapper, x, y, entranceColourArray);
                break;
            }
            case Tunnel.FourWayIntersection:
            {
                DrawFourWayEntranceIntersection(canvas, pixelMapper, x, y, entranceColourArray);
                break;
            }
        }
    }

    private void BuildEntrances(Track? neighbour, Tunnel neighbourCellTunnel, Dictionary<(int column, int row), Tunnel> entrances)
    {
        if (!IsEntrance(neighbour)) return;

        var key = (neighbour.Column, neighbour.Row);

        entrances.TryGetValue(key, out var neighbourCellTunnels);
        neighbourCellTunnels |= neighbourCellTunnel;
        entrances[key] = neighbourCellTunnels;
    }

    private bool IsEntrance([NotNullWhen(returnValue: true)] Track? track)
        => track is not null && !_terrainMap.Get(track.Column, track.Row).IsMountain;

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

    private Color BuildModeAwareColour(Color color)
        => !_gameBoard.Enabled
            ? color with { A = BuildModeAlpha }
            : color;
}
