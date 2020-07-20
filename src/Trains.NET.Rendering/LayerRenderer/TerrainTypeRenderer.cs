using System;
using System.Collections.Generic;
using System.IO;
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

        private const int GradientWidth = 10;

        public TerrainTypeRenderer(ITerrainMap terrainMap, IPixelMapper pixelMapper, ITrackParameters trackParameters)
        {
            _terrainMap = terrainMap;
            _pixelMapper = pixelMapper;
            _trackParameters = trackParameters;
        }

        public bool Enabled { get; set; } = true;
        public string Name => "TerrainType";

        public void Render(ICanvas canvas, int width, int height)
        {
            DrawTerrainTypes(canvas);
        }

        private void DrawTerrainTypes(ICanvas canvas)
        {
            foreach (var terrain in _terrainMap)
            {
                DrawTerrain(canvas, terrain);
            }
        }

        private void DrawTerrain(ICanvas canvas, Terrain terrain)
        {
            var paintBrush = new PaintBrush
            {
                Color = GetColourForTerrainType(terrain.TerrainType),
                Style = PaintStyle.Fill,
                IsAntialias = true
            };

            (int x, int y) = _pixelMapper.CoordsToViewPortPixels(terrain.Column, terrain.Row);

            canvas.DrawRect(x, y, _trackParameters.CellSize, _trackParameters.CellSize, paintBrush);


            // Right
            var rightNeighbour = _terrainMap.GetAdjacentTerrainRight(terrain);
            if (rightNeighbour.TerrainType != TerrainType.Empty && GetWeightForTerrain(rightNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.GradientRectLeftRight(
                         x: x + _trackParameters.CellSize - GradientWidth,
                         y: y,
                         width: GetGradientWidthBetweenTerrainTypes(rightNeighbour.TerrainType, terrain.TerrainType),
                         height: _trackParameters.CellSize,
                        colours: GetColoursForGradient(terrain, rightNeighbour)
                );
            }


            // Left
            var leftNeighbour = _terrainMap.GetAdjacentTerrainLeft(terrain);
            if (leftNeighbour.TerrainType != TerrainType.Empty && GetWeightForTerrain(leftNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.GradientRectLeftRight(
                         x: x,
                         y: y,
                         width: GetGradientWidthBetweenTerrainTypes(leftNeighbour.TerrainType, terrain.TerrainType),
                         height: _trackParameters.CellSize,
                         colours: GetColoursForGradient(leftNeighbour, terrain)
                );
            }

            // Up
            var upNeighbour = _terrainMap.GetAdjacentTerrainUp(terrain);
            if (upNeighbour.TerrainType != TerrainType.Empty && GetWeightForTerrain(upNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.GradientRectUpDown(
                         x: x,
                         y: y,
                         width: _trackParameters.CellSize,
                         height: GetGradientWidthBetweenTerrainTypes(upNeighbour.TerrainType, terrain.TerrainType),
                         colours: GetColoursForGradient(upNeighbour, terrain)
                );
            }

            // Down
            var downNeighbour = _terrainMap.GetAdjacentTerrainDown(terrain);
            if (downNeighbour.TerrainType != TerrainType.Empty && GetWeightForTerrain(downNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.GradientRectUpDown(
                         x: x,
                         y: y + _trackParameters.CellSize - GradientWidth,
                         width: _trackParameters.CellSize,
                         height: GetGradientWidthBetweenTerrainTypes(downNeighbour.TerrainType, terrain.TerrainType),
                         colours: GetColoursForGradient(terrain, downNeighbour)
                );
            }

            //Top Left Corner smoothing gradient
            if (upNeighbour.TerrainType == leftNeighbour.TerrainType && GetWeightForTerrain(upNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.CircularGradient(
                    x: x,
                    y: y,
                    width: GradientWidth,
                    height: GradientWidth,
                    circleCentreX: x + GradientWidth,
                    circleCentreY: y + GradientWidth,
                    circleRadius: GetGradientWidthBetweenTerrainTypes(upNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(terrain, upNeighbour)
                );
            }

            //Top Right Corner smoothing gradient
            if (upNeighbour.TerrainType == rightNeighbour.TerrainType && GetWeightForTerrain(upNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.CircularGradient(
                    x: x + _trackParameters.CellSize - GradientWidth,
                    y: y,
                    width: GradientWidth,
                    height: GradientWidth,
                    circleCentreX: x + _trackParameters.CellSize - GradientWidth,
                    circleCentreY: y + GradientWidth,
                    circleRadius: GetGradientWidthBetweenTerrainTypes(upNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(terrain, upNeighbour) 
                );
            }

            //Bottom Left Corner smoothing gradient
            if (downNeighbour.TerrainType == leftNeighbour.TerrainType && GetWeightForTerrain(downNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.CircularGradient(
                    x: x,
                    y: y + _trackParameters.CellSize - GradientWidth,
                    width: GradientWidth,
                    height: GradientWidth,
                    circleCentreX: x + GradientWidth,
                    circleCentreY: y + _trackParameters.CellSize - GradientWidth,
                    circleRadius: GetGradientWidthBetweenTerrainTypes(downNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(terrain, downNeighbour) 
                );
            }

            //Bottom Right Corner smoothing gradient
            if (downNeighbour.TerrainType == rightNeighbour.TerrainType && GetWeightForTerrain(downNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.CircularGradient(
                    x: x + _trackParameters.CellSize - GradientWidth,
                    y: y + _trackParameters.CellSize - GradientWidth,
                    width: GradientWidth,
                    height: GradientWidth,
                    circleCentreX: x +_trackParameters.CellSize - GradientWidth,
                    circleCentreY: y + _trackParameters.CellSize - GradientWidth,
                    circleRadius: GetGradientWidthBetweenTerrainTypes(downNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(terrain, downNeighbour)
                );
            }


            //Top Left Corner fragment gradient
            var upLeftNeighbour = _terrainMap.GetAdjacentTerrainUpLeft(terrain);
            if (terrain.TerrainType == upNeighbour.TerrainType && upNeighbour.TerrainType == leftNeighbour.TerrainType && GetWeightForTerrain(upLeftNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.CircularGradient(
                    x: x,
                    y: y,
                    width: GradientWidth,
                    height: GradientWidth,
                    circleCentreX: x,
                    circleCentreY: y,
                    circleRadius: GetGradientWidthBetweenTerrainTypes(upLeftNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(upLeftNeighbour, terrain)
                );
            }

            //Top Right Corner fragment gradient
            var upRightNeighbour = _terrainMap.GetAdjacentTerrainUpRight(terrain);
            if (terrain.TerrainType == upNeighbour.TerrainType && upNeighbour.TerrainType == rightNeighbour.TerrainType && GetWeightForTerrain(upRightNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.CircularGradient(
                    x: x + _trackParameters.CellSize - GradientWidth,
                    y: y,
                    width: GradientWidth,
                    height: GradientWidth,
                    circleCentreX: x + _trackParameters.CellSize,
                    circleCentreY: y,
                    circleRadius: GetGradientWidthBetweenTerrainTypes(upLeftNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(upRightNeighbour, terrain)
                );
            }

            //Down Left Corner fragment gradient
            var downLeftNeighbour = _terrainMap.GetAdjacentTerrainDownLeft(terrain);
            if (terrain.TerrainType == downNeighbour.TerrainType && downNeighbour.TerrainType == leftNeighbour.TerrainType && GetWeightForTerrain(downLeftNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.CircularGradient(
                    x: x,
                    y: y + _trackParameters.CellSize - GradientWidth,
                    width: GradientWidth,
                    height: GradientWidth,
                    circleCentreX: x,
                    circleCentreY: y + _trackParameters.CellSize,
                    circleRadius: GetGradientWidthBetweenTerrainTypes(downLeftNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(downLeftNeighbour, terrain)
                );
            }

            //Down Right Corner fragment gradient
            var downRightNeighbour = _terrainMap.GetAdjacentTerrainDownRight(terrain);
            if (terrain.TerrainType == downNeighbour.TerrainType && downNeighbour.TerrainType == rightNeighbour.TerrainType && GetWeightForTerrain(downRightNeighbour) > GetWeightForTerrain(terrain))
            {
                canvas.CircularGradient(
                    x: x + _trackParameters.CellSize - GradientWidth,
                    y: y + _trackParameters.CellSize - GradientWidth,
                    width: GradientWidth,
                    height: GradientWidth,
                    circleCentreX: x + _trackParameters.CellSize,
                    circleCentreY: y + _trackParameters.CellSize,
                    circleRadius: GetGradientWidthBetweenTerrainTypes(downRightNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(downRightNeighbour, terrain)
                );
            }

            //Top Left Corner 3 way, orthogonal sand
            if (terrain.TerrainType == TerrainType.Water
               &&  upNeighbour.TerrainType == TerrainType.Sand 
               && leftNeighbour.TerrainType == TerrainType.Sand 
               && upLeftNeighbour.TerrainType == TerrainType.Grass)
            {
                canvas.GradientRectTopLeftBottomtRight(
                    x: x,
                    y: y,
                    width: GetGradientWidthBetweenTerrainTypes(upLeftNeighbour.TerrainType, terrain.TerrainType),
                    height: GetGradientWidthBetweenTerrainTypes(upLeftNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(upLeftNeighbour, terrain)
                );
            }

            //Top Right Corner 3 way, orthogonal sand
            if (terrain.TerrainType == TerrainType.Water
               && upNeighbour.TerrainType == TerrainType.Sand
               && rightNeighbour.TerrainType == TerrainType.Sand
               && upRightNeighbour.TerrainType == TerrainType.Grass)
            {
                canvas.GradientRectTopRightBottomLeft(
                    x: x + _trackParameters.CellSize - GradientWidth,
                    y: y,
                    width: GetGradientWidthBetweenTerrainTypes(upRightNeighbour.TerrainType, terrain.TerrainType),
                    height: GetGradientWidthBetweenTerrainTypes(upRightNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(upRightNeighbour, terrain)
                );
            }

            //Down left Corner 3 way, orthogonal sand
            if (terrain.TerrainType == TerrainType.Water
               && downNeighbour.TerrainType == TerrainType.Sand
               && leftNeighbour.TerrainType == TerrainType.Sand
               && downLeftNeighbour.TerrainType == TerrainType.Grass)
            {
                canvas.GradientRectTopRightBottomLeft(
                    x: x,
                    y: y + _trackParameters.CellSize - GradientWidth,
                    width: GetGradientWidthBetweenTerrainTypes(downLeftNeighbour.TerrainType, terrain.TerrainType),
                    height: GetGradientWidthBetweenTerrainTypes(downLeftNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(terrain, downLeftNeighbour)
                );
            }

            //Down right Corner 3 way, orthogonal sand
            if (terrain.TerrainType == TerrainType.Water
               && downNeighbour.TerrainType == TerrainType.Sand
               && rightNeighbour.TerrainType == TerrainType.Sand
               && downRightNeighbour.TerrainType == TerrainType.Grass)
            {
                canvas.GradientRectTopLeftBottomtRight(
                    x: x + _trackParameters.CellSize - GradientWidth,
                    y: y + _trackParameters.CellSize - GradientWidth,
                    width: GetGradientWidthBetweenTerrainTypes(downRightNeighbour.TerrainType, terrain.TerrainType),
                    height: GetGradientWidthBetweenTerrainTypes(downRightNeighbour.TerrainType, terrain.TerrainType),
                    colours: GetColoursForGradient(terrain, downRightNeighbour)
                );
            }

            //Top Left Corner, orthogonal different
            if (terrain.TerrainType == TerrainType.Water
               && upNeighbour.TerrainType == TerrainType.Grass
               && leftNeighbour.TerrainType == TerrainType.Sand)
            {
                canvas.CircularGradient(
                     x: x,
                     y: y + GradientWidth,
                     width: GradientWidth,
                     height: GradientWidth,
                     circleCentreX: x + GradientWidth,
                     circleCentreY: y + GradientWidth * 2,
                     circleRadius: 2 * GradientWidth,
                     colours: new Color[] { Colors.LightBlue, Colors.LightYellow }//GetColoursForGradient(terrain, leftNeighbour)
                 );
            }
        }

        private static Color[] GetColoursForGradient(Terrain terrain1, Terrain terrain2)
        {
            // Special case for grass => water since we want to drop some sand in between
            if ((terrain1.TerrainType == TerrainType.Grass && terrain2.TerrainType == TerrainType.Water) ||
                (terrain2.TerrainType == TerrainType.Grass && terrain1.TerrainType == TerrainType.Water))
            {
                return new Color[] {
                    GetColourForTerrainType(terrain1.TerrainType),
                    GetColourForTerrainType(TerrainType.Sand),
                    GetColourForTerrainType(terrain2.TerrainType)
                };
            }

            return new Color[]
            {
                GetColourForTerrainType(terrain1.TerrainType),
                GetColourForTerrainType(terrain2.TerrainType)
            };
        }

        private static Color GetColourForTerrainType(TerrainType terrainType)
        {
            return terrainType switch
            {
                TerrainType.Grass => Colors.LightGreen,
                TerrainType.Sand => Colors.LightYellow,
                TerrainType.Water => Colors.LightBlue,
                TerrainType.Empty => Colors.Empty
            };
        }

        private static float GetGradientWidthBetweenTerrainTypes(TerrainType terrainTypeFrom, TerrainType terrainTypeTo)
        {
            if (terrainTypeFrom == TerrainType.Grass && terrainTypeTo == TerrainType.Water) return 2 * GradientWidth;

            return GradientWidth;
        }

        private static int GetWeightForTerrain(Terrain terrain)
        {
            return terrain.TerrainType switch
            {
                TerrainType.Grass => 3,
                TerrainType.Sand => 2,
                TerrainType.Water => 1,
                TerrainType.Empty => 0
            };
        }
    }
}
