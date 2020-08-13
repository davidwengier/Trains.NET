using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(1)]
    internal class ContourRenderer : ILayerRenderer
    {
        private const int ContourHeight = 40;
        private readonly ITerrainMap _terrainMap;
        private readonly IGameParameters _gameParameters;

        private readonly PaintBrush _paintBrush;

        public ContourRenderer(ITerrainMap terrainMap, IGameParameters gameParameters)
        {
            _terrainMap = terrainMap;
            _gameParameters = gameParameters;

            _paintBrush = new PaintBrush
            {
                Color = Colors.LightGray,
                StrokeWidth = 1,
                Style = PaintStyle.Stroke
            };
        }

        public bool Enabled { get; set; }
        public string Name => "Contours";

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            Dictionary<int, List<ViewportPoint>>? contourLevels = GenerateListOfContourPointsForEachContourLevel(pixelMapper);

            foreach (int contourLevel in contourLevels.Keys)
            {
                List<ViewportPoint>? contourPoints = contourLevels[contourLevel];
                DrawContourLine(canvas, contourPoints);
            }
        }

        private void DrawContourLine(ICanvas canvas, List<ViewportPoint> contourPoints)
        {
            foreach (ViewportPoint contourPoint in contourPoints)
            {
                List<ViewportPoint>? orderedList = OrderListByDistanceFromPoint(contourPoint, contourPoints);
                if (orderedList.Count < 3) continue;

                ViewportPoint firstNearest = orderedList.Skip(1).FirstOrDefault(); // The first element will be the original element since that will have distance zero
                ViewportPoint secondNearest = orderedList.Skip(2).FirstOrDefault();

                DrawContourLineInViewport(canvas, contourPoint, firstNearest);
                DrawContourLineInViewport(canvas, contourPoint, secondNearest);
            }
        }

        private void DrawContourLineInViewport(ICanvas canvas, ViewportPoint contourPoint1, ViewportPoint contourPoint2)
        {
            canvas.DrawLine(contourPoint1.PixelX, contourPoint1.PixelY, contourPoint2.PixelX, contourPoint2.PixelY, _paintBrush);
        }

        private Dictionary<int, List<ViewportPoint>> GenerateListOfContourPointsForEachContourLevel(IPixelMapper pixelMapper)
        {
            var topography = new Dictionary<int, List<ViewportPoint>>();
            foreach (Terrain? terrain in _terrainMap)
            {
                int contourLevel = CalculateContourLevel(terrain.Height);
                List<ViewportPoint>? contourPoints = topography.ContainsKey(contourLevel) ? topography[contourLevel] : new List<ViewportPoint>();
                var adjacentTerrainLookups = new List<Func<Terrain, Terrain>>
                {
                    _terrainMap.GetAdjacentTerrainUp,
                    _terrainMap.GetAdjacentTerrainDown,
                    _terrainMap.GetAdjacentTerrainLeft,
                    _terrainMap.GetAdjacentTerrainRight,
                };

                foreach (Func<Terrain, Terrain>? adjacentTerrainLookup in adjacentTerrainLookups)
                {
                    ViewportPoint? adjacentContourPoint = CalculateBorderingContourPointIfAdjacentTerrainIsLower(adjacentTerrainLookup, terrain, contourLevel, pixelMapper);
                    if (adjacentContourPoint != null && !contourPoints.Contains(adjacentContourPoint.Value))
                    {
                        contourPoints.Add(adjacentContourPoint.Value);
                    }
                }

                topography[contourLevel] = contourPoints;
            }

            return topography;
        }

        private ViewportPoint? CalculateBorderingContourPointIfAdjacentTerrainIsLower(Func<Terrain, Terrain> getAdjacentTerrain, Terrain terrain, int contourLevel, IPixelMapper pixelMapper)
        {
            Terrain? adjacentTerrain = getAdjacentTerrain(terrain);

            int adjacentContourLevel = CalculateContourLevel(adjacentTerrain.Height);

            if (adjacentContourLevel >= contourLevel) return null;

           return FindContourPointBetweenAdjacentTerrain(terrain, adjacentTerrain, pixelMapper);
        }

        private ViewportPoint FindContourPointBetweenAdjacentTerrain(Terrain sourceTerrain, Terrain adjacentTerrain, IPixelMapper pixelMapper)
        {
            int columnDelta = sourceTerrain.Column - adjacentTerrain.Column;
            int rowDelta = sourceTerrain.Row - adjacentTerrain.Row;

            // Adjacent means that there should only be exactly one difference between either Columns or Rows (And the other should have no difference)
            if ((Math.Abs(columnDelta) + Math.Abs(rowDelta)) != 1)
                throw new ArgumentException("Trying to find contour point between terrain that is not adjacent");

            // Different columns, must be same row
            if (columnDelta != 0)
            {
                // columnDelta gt zero means we have source on right, adjacent on left so centre point is between them.. which means source edge (since cell edges are on left)
                // columnDelta lt zero means we have source on left, adjacent on right so centre point is again between them.. which means adjacent edge 

                int selectedColumn = (columnDelta > 0 ? sourceTerrain.Column : adjacentTerrain.Column);
                (int pixX, int pixY) = pixelMapper.CoordsToViewPortPixels(selectedColumn, sourceTerrain.Row);

                return new ViewportPoint
                {
                    PixelX = pixX,
                    PixelY = pixY + 0.5f * _gameParameters.CellSize,
                };
            }

            int selectedRow = (rowDelta > 0 ? sourceTerrain.Row : adjacentTerrain.Row);
            (int pixelX, int pixelY) = pixelMapper.CoordsToViewPortPixels(sourceTerrain.Column, selectedRow);
            return new ViewportPoint
            {
                PixelX = pixelX + 0.5f * _gameParameters.CellSize,
                PixelY = pixelY
            };
        }

        private static List<ViewportPoint> OrderListByDistanceFromPoint(ViewportPoint point, List<ViewportPoint> pointsList)
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
