using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains.NET.Rendering
{
    [Order(0)]
    internal class TerrainRenderer : ILayerRenderer
    {
        private const int ContourHeight = 40;
        private readonly ITerrainMap _terrainLayout;
        private readonly IPixelMapper _pixelMapper;

        public TerrainRenderer( ITerrainMap terrainLayout, IPixelMapper pixelMapper)
        {
            _terrainLayout = terrainLayout;
            _pixelMapper = pixelMapper;
        }

        public bool Enabled { get; set; } = false;
        public string Name => "Terrain";

        public void Render(ICanvas canvas, int width, int height)
        {
            var contourLevels = GenerateListOfContourPointsForEachContourLevel();

            foreach (var contourLevel in contourLevels.Keys)
            {
                var contourPoints = contourLevels[contourLevel];
                DrawContourLine(canvas, contourPoints);
            }
        }

        private void DrawContourLine(ICanvas canvas, List<ContourPoint> contourPoints)
        {
            foreach (var contourPoint in contourPoints)
            {
                var orderedList = OrderListByDistanceFromPoint(contourPoint, contourPoints);
                var firstNearest = orderedList.Skip(1).First(); // The first element will be the original element since that will have distance zero
                var secondNearest = orderedList.Skip(2).First();

                DrawContourLineInViewport(canvas, contourPoint, firstNearest);
                DrawContourLineInViewport(canvas, contourPoint, secondNearest);
            }
        }

        private void DrawContourLineInViewport(ICanvas canvas, ContourPoint contourPoint1, ContourPoint contourPoint2)
        {
            var grid = new PaintBrush
            {
                Color = Colors.LightGray,
                StrokeWidth = 1,
                Style = PaintStyle.Stroke
            };

            var (col1, row1) = _pixelMapper.CoordsToViewPortPixels(contourPoint1.Column, contourPoint1.Row);
            var (col2, row2) = _pixelMapper.CoordsToViewPortPixels(contourPoint2.Column, contourPoint2.Row);

            canvas.DrawLine(col1, row1, col2, row2, grid);
        }

        private Dictionary<int, List<ContourPoint>> GenerateListOfContourPointsForEachContourLevel()
        {
            var topography = new Dictionary<int, List<ContourPoint>>();
            foreach (var terrain in _terrainLayout)
            {
                var contourLevel = CalculateContourLevel(terrain.Altitude);
                var contourPoints = topography.ContainsKey(contourLevel) ? topography[contourLevel] : new List<ContourPoint>();

                var adjacentTerrainLookups = new List<Func<Terrain, Terrain>>
                {
                    _terrainLayout.GetAdjacentTerrainUp,
                    _terrainLayout.GetAdjacentTerrainDown,
                    _terrainLayout.GetAdjacentTerrainLeft,
                    _terrainLayout.GetAdjacentTerrainRight,
                };

                foreach (var adjacentTerrainLookup in adjacentTerrainLookups)
                {
                    var adjacentContourPoint = CalculateBorderingContourPointIfAdjacentTerrainIsLower(adjacentTerrainLookup, terrain, contourLevel);
                    if (adjacentContourPoint != null && !contourPoints.Contains(adjacentContourPoint.Value))
                        contourPoints.Add(adjacentContourPoint.Value);
                }

                topography[contourLevel] = contourPoints;
            }

            return topography;
        }

        private ContourPoint? CalculateBorderingContourPointIfAdjacentTerrainIsLower(Func<Terrain, Terrain> getAdjacentTerrain, Terrain terrain, int contourLevel)
        {
            var adjacentTerrain = getAdjacentTerrain(terrain);

            var adjacentContourLevel = CalculateContourLevel(adjacentTerrain.Altitude);

            if (adjacentContourLevel >= contourLevel) return null;

           return FindContourPointBetweenAdjacentTerrain(terrain, adjacentTerrain);
        }

        private ContourPoint FindContourPointBetweenAdjacentTerrain(Terrain sourceTerrain, Terrain adjacentTerrain)
        {
            var columnDelta = sourceTerrain.Column - adjacentTerrain.Column;
            var rowDelta = sourceTerrain.Row - adjacentTerrain.Row;

            // Adjacent means that there should only be exactly one difference between either Columns or Rows (And the other should have no difference)
            if ((Math.Abs(columnDelta) + Math.Abs(rowDelta)) != 1)
                throw new ArgumentException("Trying to find contour point between terrain that is not adjacent");

            if (columnDelta != 0)
            {
                // Different columns, must be same row
                return new ContourPoint
                {
                    // columnDelta gt zero means we have source on right, adjacent on left so centre point is between them.. which means source edge (since cell edges are on left)
                    // columnDelta lt zero means we have source on left, adjacent on right so centre point is again between them.. which means adjacent edge 
                    Column = (columnDelta > 0 ? sourceTerrain.Column : adjacentTerrain.Column),
                    Row = (sourceTerrain.Row + 0.5f),
                };
            }

            return new ContourPoint
            {
                Column = (sourceTerrain.Column + 0.5f),
                Row = (rowDelta > 0 ? sourceTerrain.Row : adjacentTerrain.Row)
            };
        }

        private List<ContourPoint> OrderListByDistanceFromPoint(ContourPoint point, List<ContourPoint> pointsList)
        {
            return pointsList.OrderBy(p => CalculateDistanceBetweenTwoPoints(point, p)).ToList();
        }

        private static float CalculateDistanceBetweenTwoPoints(ContourPoint firstPoint, ContourPoint secondPoint)
        {
            return (float)Math.Sqrt((firstPoint.Column - secondPoint.Column) * (firstPoint.Column - secondPoint.Column) + 
                                    (firstPoint.Row - secondPoint.Row) * (firstPoint.Row - secondPoint.Row));
        }

        private static int CalculateContourLevel(int altitude)
        {
            return altitude / ContourHeight; // Integer divide
        }

    }

    internal struct ContourPoint
    {
        public float Column { get; set; }
        public float Row { get; set; }
    }

}
