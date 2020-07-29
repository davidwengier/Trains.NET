using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Trains.NET.Engine;
using Trains.NET.Engine.Terrain;

namespace Trains.NET.Rendering
{
    [Order(1)]
    internal class ContourRenderer : ILayerRenderer
    {
        private const int ContourHeight = 40;
        private readonly ITerrainMap _terrainMap;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _trackParameters;

        private readonly PaintBrush _paintBrush;

        public ContourRenderer(ITerrainMap terrainMap, IPixelMapper pixelMapper, ITrackParameters trackParameters)
        {
            _terrainMap = terrainMap;
            _pixelMapper = pixelMapper;
            _trackParameters = trackParameters;

            _paintBrush = new PaintBrush
            {
                Color = Colors.LightGray,
                StrokeWidth = 1,
                Style = PaintStyle.Stroke
            };
        }

        public bool Enabled { get; set; } = false;
        public string Name => "Contours";

        public void Render(ICanvas canvas, int width, int height)
        {
            DrawContourLines(canvas);
        }

        private void DrawContourLines(ICanvas canvas)
        {
            var contourLevels = GenerateListOfContourPointsForEachContourLevel();

            foreach (var contourLevel in contourLevels.Keys)
            {
                var contourPoints = contourLevels[contourLevel];
                DrawContourLine(canvas, contourPoints);
            }
        }

        private void DrawContourLine(ICanvas canvas, List<ViewportPoint> contourPoints)
        {
            foreach (var contourPoint in contourPoints)
            {
                var orderedList = OrderListByDistanceFromPoint(contourPoint, contourPoints);
                if (orderedList.Count < 3) continue;

                var firstNearest = orderedList.Skip(1).FirstOrDefault(); // The first element will be the original element since that will have distance zero
                var secondNearest = orderedList.Skip(2).FirstOrDefault();

                DrawContourLineInViewport(canvas, contourPoint, firstNearest);
                DrawContourLineInViewport(canvas, contourPoint, secondNearest);
            }
        }

        private void DrawContourLineInViewport(ICanvas canvas, ViewportPoint contourPoint1, ViewportPoint contourPoint2)
        {
            canvas.DrawLine(contourPoint1.PixelX, contourPoint1.PixelY, contourPoint2.PixelX, contourPoint2.PixelY, _paintBrush);
        }

        private Dictionary<int, List<ViewportPoint>> GenerateListOfContourPointsForEachContourLevel()
        {
            var topography = new Dictionary<int, List<ViewportPoint>>();
            foreach (var terrain in _terrainMap)
            {
                var contourLevel = CalculateContourLevel(terrain.Height);
                var contourPoints = topography.ContainsKey(contourLevel) ? topography[contourLevel] : new List<ViewportPoint>();

                foreach (var adjacency in AdjacencyTypes.OrthogonalAdjacents)
                {
                    var adjacentContourPoint = CalculateBorderingContourPointIfAdjacentTerrainIsLower(adjacency, terrain, contourLevel);
                    if (adjacentContourPoint != null && !contourPoints.Contains(adjacentContourPoint.Value))
                    {
                        contourPoints.Add(adjacentContourPoint.Value);
                    }
                }

                topography[contourLevel] = contourPoints;
            }

            return topography;
        }

        private ViewportPoint? CalculateBorderingContourPointIfAdjacentTerrainIsLower(TerrainAdjacency adjacency, TerrainCell terrain, int contourLevel)
        {
            var adjacentTerrain = _terrainMap.GetAdjacentTerrain(terrain, adjacency);

            var adjacentContourLevel = CalculateContourLevel(adjacentTerrain.Height);

            if (adjacentContourLevel >= contourLevel) return null;

           return FindContourPointBetweenAdjacentTerrain(terrain, adjacentTerrain);
        }

        private ViewportPoint FindContourPointBetweenAdjacentTerrain(TerrainCell sourceTerrain, TerrainCell adjacentTerrain)
        {
            var columnDelta = sourceTerrain.Column - adjacentTerrain.Column;
            var rowDelta = sourceTerrain.Row - adjacentTerrain.Row;

            // Adjacent means that there should only be exactly one difference between either Columns or Rows (And the other should have no difference)
            if ((Math.Abs(columnDelta) + Math.Abs(rowDelta)) != 1)
                throw new ArgumentException("Trying to find contour point between terrain that is not adjacent");

            // Different columns, must be same row
            if (columnDelta != 0)
            {
                // columnDelta gt zero means we have source on right, adjacent on left so centre point is between them.. which means source edge (since cell edges are on left)
                // columnDelta lt zero means we have source on left, adjacent on right so centre point is again between them.. which means adjacent edge 

                var selectedColumn = (columnDelta > 0 ? sourceTerrain.Column : adjacentTerrain.Column);
                var (pixX, pixY) = _pixelMapper.CoordsToViewPortPixels(selectedColumn, sourceTerrain.Row);

                return new ViewportPoint
                {
                    PixelX = pixX,
                    PixelY = pixY + 0.5f * _trackParameters.CellSize,
                };
            }

            var selectedRow = (rowDelta > 0 ? sourceTerrain.Row : adjacentTerrain.Row);
            var (pixelX, pixelY) = _pixelMapper.CoordsToViewPortPixels(sourceTerrain.Column, selectedRow);
            return new ViewportPoint
            {
                PixelX = pixelX + 0.5f * _trackParameters.CellSize,
                PixelY = pixelY
            };
        }

        private List<ViewportPoint> OrderListByDistanceFromPoint(ViewportPoint point, List<ViewportPoint> pointsList)
        {
            return pointsList.OrderBy(p => CalculateDistanceBetweenTwoPoints(point, p)).ToList();
        }

        private static float CalculateDistanceBetweenTwoPoints(ViewportPoint firstPoint, ViewportPoint secondPoint)
        {
            return (float)Math.Sqrt((firstPoint.PixelX - secondPoint.PixelX) * (firstPoint.PixelX - secondPoint.PixelX) + 
                                    (firstPoint.PixelY - secondPoint.PixelY) * (firstPoint.PixelY - secondPoint.PixelY));
        }

        private static int CalculateContourLevel(int altitude)
        {
            return altitude / ContourHeight; // Integer divide
        }
    }
}
