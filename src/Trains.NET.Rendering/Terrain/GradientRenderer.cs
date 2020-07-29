using System;
using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine.Terrain;

namespace Trains.NET.Rendering.Terrain
{
    public class GradientRenderer : IGradientRenderer
    {
        private const int GradientWidth = 10;

        private readonly ITrackParameters _trackParameters;
        private readonly IPixelMapper _pixelMapper;

        public GradientRenderer(ITrackParameters trackParameters, IPixelMapper pixelMapper)
        {
            _trackParameters = trackParameters;
            _pixelMapper = pixelMapper;
        }

        public void Render(ICanvas canvas, TerrainCellGradient gradient)
        {
            if (gradient.GradientType == GradientType.None) return;

            if (AdjacencyTypes.OrthogonalAdjacents.Contains(gradient.Adjacency))
            {
                RenderEdge(canvas, gradient);
                return;
            }

            RenderCorner(canvas, gradient);
        }

        private void RenderCorner(ICanvas canvas, TerrainCellGradient gradient)
        {
            var cornerGradient = gradient.GradientTypeJoin.HasValue
                ? CalculateJoinCornerGradient(gradient.GradientType, gradient.GradientTypeJoin.Value, gradient.Adjacency)
                : CalculateInverseCornerGradient(gradient.GradientType, gradient.Adjacency);

            (int x, int y) = _pixelMapper.CoordsToViewPortPixels(gradient.Terrain.Column, gradient.Terrain.Row);

            canvas.CircularGradient(
                   x: x + cornerGradient.XOffset,
                   y: y + cornerGradient.YOffset,
                   width: cornerGradient.Width,
                   height: cornerGradient.Height,
                   circleCentreX: x + cornerGradient.CircleCentreX,
                   circleCentreY: y + cornerGradient.CircleCentreY,
                   circleRadius: cornerGradient.CircleRadius,
                   colours: cornerGradient.Colours);
        }

        private CornerGradient CalculateInverseCornerGradient(GradientType gradientType, TerrainAdjacency adjacency)
        {
            var gradientWidth = GetGradientWidth(gradientType);
            var colours = GetColoursForGradientType(gradientType);

            return adjacency switch
            {
                TerrainAdjacency.UpLeft => new CornerGradient
                    {
                        XOffset = 0,
                        YOffset = 0,
                        Width = gradientWidth,
                        Height = gradientWidth,
                        CircleCentreX = 0,
                        CircleCentreY = 0,
                        CircleRadius = gradientWidth,
                        Colours = colours
                },
                TerrainAdjacency.UpRight => new CornerGradient
                    {
                        XOffset = 0 + _trackParameters.CellSize - gradientWidth,
                        YOffset = 0,
                        Width = gradientWidth,
                        Height = gradientWidth,
                        CircleCentreX = _trackParameters.CellSize,
                        CircleCentreY = 0,
                        CircleRadius = gradientWidth,
                        Colours = colours
                },
                TerrainAdjacency.DownRight => new CornerGradient
                {
                    XOffset = 0 + _trackParameters.CellSize - gradientWidth,
                    YOffset = 0 + _trackParameters.CellSize - gradientWidth,
                    Width = gradientWidth,
                    Height = gradientWidth,
                    CircleCentreX = 0 + _trackParameters.CellSize,
                    CircleCentreY = 0 + _trackParameters.CellSize,
                    CircleRadius = gradientWidth,
                    Colours = colours
                },
                TerrainAdjacency.DownLeft => new CornerGradient
                {
                    XOffset = 0,
                    YOffset = 0 + _trackParameters.CellSize - gradientWidth,
                    Width = gradientWidth,
                    Height = gradientWidth,
                    CircleCentreX = 0,
                    CircleCentreY = 0 + _trackParameters.CellSize,
                    CircleRadius = gradientWidth,
                    Colours = colours
                },

                _ => throw new NotImplementedException()
            };
        }

        private CornerGradient CalculateJoinCornerGradient(GradientType gradientType, GradientType gradientTypeJoin, TerrainAdjacency adjacency)
        {
            var gradientWidth = GetGradientWidth(gradientType);
            var colours = GetColoursForGradientType(gradientType).Reverse();
            if (gradientType == GradientType.GrassToWater && gradientTypeJoin == GradientType.SandToWater)
            {
                colours = GetColoursForGradientType(GradientType.SandToWater).Reverse();
                gradientWidth = GradientWidth;
            }
            var gradientOffset = GetGradientOffsetForCornerJoinBetween(gradientType, gradientTypeJoin);

            return adjacency switch
            {
                TerrainAdjacency.UpLeft => new CornerGradient
                {
                    XOffset = 0 + (gradientOffset > 0 ? gradientOffset : 0),
                    YOffset = 0 - (gradientOffset < 0 ? gradientOffset : 0),
                    Width = gradientWidth,
                    Height = gradientWidth,
                    CircleCentreX = gradientWidth + (gradientOffset > 0 ? gradientOffset : 0),
                    CircleCentreY = gradientWidth - (gradientOffset < 0 ? gradientOffset : 0),
                    CircleRadius = gradientWidth,
                    Colours = colours
                },
                TerrainAdjacency.UpRight => new CornerGradient
                {
                    XOffset = _trackParameters.CellSize - gradientWidth + (gradientOffset < 0 ? gradientOffset : 0),
                    YOffset = 0 + (gradientOffset > 0 ? gradientOffset : 0),
                    Width = gradientWidth,
                    Height = gradientWidth,
                    CircleCentreX = _trackParameters.CellSize - gradientWidth + (gradientOffset < 0 ? gradientOffset : 0),
                    CircleCentreY = gradientWidth + (gradientOffset > 0 ? gradientOffset : 0),
                    CircleRadius = gradientWidth,
                    Colours = colours
                },
                TerrainAdjacency.DownRight => new CornerGradient
                {
                    XOffset = _trackParameters.CellSize - gradientWidth - (gradientOffset > 0 ? gradientOffset : 0),
                    YOffset = _trackParameters.CellSize - gradientWidth + (gradientOffset < 0 ? gradientOffset : 0),
                    Width = gradientWidth,
                    Height = gradientWidth,
                    CircleCentreX = _trackParameters.CellSize - gradientWidth - (gradientOffset > 0 ? gradientOffset : 0),
                    CircleCentreY = _trackParameters.CellSize - gradientWidth + (gradientOffset < 0 ? gradientOffset : 0), 
                    CircleRadius = gradientWidth,
                    Colours = colours
                },
                TerrainAdjacency.DownLeft => new CornerGradient
                {
                    XOffset = 0 - (gradientOffset < 0 ? gradientOffset : 0),
                    YOffset = _trackParameters.CellSize - gradientWidth - (gradientOffset > 0 ? gradientOffset : 0),
                    Width = gradientWidth,
                    Height = gradientWidth,
                    CircleCentreX = gradientWidth - (gradientOffset < 0 ? gradientOffset : 0),
                    CircleCentreY =  _trackParameters.CellSize - gradientWidth - (gradientOffset > 0 ? gradientOffset : 0),
                    CircleRadius = gradientWidth,
                    Colours = colours
                },

                _ => throw new NotImplementedException()
            };
        }

        private void RenderEdge(ICanvas canvas, TerrainCellGradient gradient)
        {
            var gradientInfo = CalculateEdgeGradient(gradient.GradientType, gradient.Adjacency);
            (int x, int y) = _pixelMapper.CoordsToViewPortPixels(gradient.Terrain.Column, gradient.Terrain.Row);

            if (gradient.Adjacency == TerrainAdjacency.Left || gradient.Adjacency == TerrainAdjacency.Right)
            {
                canvas.GradientRectLeftRight(
                    x: x + gradientInfo.XOffset,
                    y: y + gradientInfo.YOffset,
                    width: gradientInfo.Width,
                    height: gradientInfo.Height,
                    colours: gradientInfo.Colours);
            }

            if (gradient.Adjacency == TerrainAdjacency.Up || gradient.Adjacency == TerrainAdjacency.Down)
            {
                canvas.GradientRectUpDown(
                    x: x + gradientInfo.XOffset,
                    y: y + gradientInfo.YOffset,
                    width: gradientInfo.Width,
                    height: gradientInfo.Height,
                    colours: gradientInfo.Colours);
            }
        }

        #region oldcode
        //private void DumbShit(TerrainCell terrain, ICanvas canvas, float x, float y)
        //{
        //    //Top Left Corner smoothing gradient
        //    if (upNeighbour.TerrainType == leftNeighbour.TerrainType && GetWeightForTerrain(upNeighbour) > GetWeightForTerrain(terrain))
        //    {
        //        canvas.CircularGradient(
        //            x: x,
        //            y: y,
        //            width: GradientWidth,
        //            height: GradientWidth,
        //            circleCentreX: x + GradientWidth,
        //            circleCentreY: y + GradientWidth,
        //            circleRadius: GetGradientWidthBetweenTerrainTypes(upNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(terrain, upNeighbour)
        //        );
        //    }

        //    //Top Right Corner smoothing gradient
        //    if (upNeighbour.TerrainType == rightNeighbour.TerrainType && GetWeightForTerrain(upNeighbour) > GetWeightForTerrain(terrain))
        //    {
        //        canvas.CircularGradient(
        //            x: x + _trackParameters.CellSize - GradientWidth,
        //            y: y,
        //            width: GradientWidth,
        //            height: GradientWidth,
        //            circleCentreX: x + _trackParameters.CellSize - GradientWidth,
        //            circleCentreY: y + GradientWidth,
        //            circleRadius: GetGradientWidthBetweenTerrainTypes(upNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(terrain, upNeighbour)
        //        );
        //    }

        //    //Bottom Left Corner smoothing gradient
        //    if (downNeighbour.TerrainType == leftNeighbour.TerrainType && GetWeightForTerrain(downNeighbour) > GetWeightForTerrain(terrain))
        //    {
        //        canvas.CircularGradient(
        //            x: x,
        //            y: y + _trackParameters.CellSize - GradientWidth,
        //            width: GradientWidth,
        //            height: GradientWidth,
        //            circleCentreX: x + GradientWidth,
        //            circleCentreY: y + _trackParameters.CellSize - GradientWidth,
        //            circleRadius: GetGradientWidthBetweenTerrainTypes(downNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(terrain, downNeighbour)
        //        );
        //    }

        //    //Bottom Right Corner smoothing gradient
        //    if (downNeighbour.TerrainType == rightNeighbour.TerrainType && GetWeightForTerrain(downNeighbour) > GetWeightForTerrain(terrain))
        //    {
        //        canvas.CircularGradient(
        //            x: x + _trackParameters.CellSize - GradientWidth,
        //            y: y + _trackParameters.CellSize - GradientWidth,
        //            width: GradientWidth,
        //            height: GradientWidth,
        //            circleCentreX: x + _trackParameters.CellSize - GradientWidth,
        //            circleCentreY: y + _trackParameters.CellSize - GradientWidth,
        //            circleRadius: GetGradientWidthBetweenTerrainTypes(downNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(terrain, downNeighbour)
        //        );
        //    }


        //    //Top Left Corner fragment gradient
        //    var upLeftNeighbour = _terrainMap.GetAdjacentTerrain(terrain, TerrainAdjacency.UpLeft);
        //    if (terrain.TerrainType == upNeighbour.TerrainType && upNeighbour.TerrainType == leftNeighbour.TerrainType && GetWeightForTerrain(upLeftNeighbour) > GetWeightForTerrain(terrain))
        //    {
        //        canvas.CircularGradient(
        //            x: x,
        //            y: y,
        //            width: GradientWidth,
        //            height: GradientWidth,
        //            circleCentreX: x,
        //            circleCentreY: y,
        //            circleRadius: GetGradientWidthBetweenTerrainTypes(upLeftNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(upLeftNeighbour, terrain)
        //        );
        //    }

        //    //Top Right Corner fragment gradient
        //    var upRightNeighbour = _terrainMap.GetAdjacentTerrain(terrain, TerrainAdjacency.UpRight);
        //    if (terrain.TerrainType == upNeighbour.TerrainType && upNeighbour.TerrainType == rightNeighbour.TerrainType && GetWeightForTerrain(upRightNeighbour) > GetWeightForTerrain(terrain))
        //    {
        //        canvas.CircularGradient(
        //            x: x + _trackParameters.CellSize - GradientWidth,
        //            y: y,
        //            width: GradientWidth,
        //            height: GradientWidth,
        //            circleCentreX: x + _trackParameters.CellSize,
        //            circleCentreY: y,
        //            circleRadius: GetGradientWidthBetweenTerrainTypes(upLeftNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(upRightNeighbour, terrain)
        //        );
        //    }

        //    //Down Left Corner fragment gradient
        //    var downLeftNeighbour = _terrainMap.GetAdjacentTerrain(terrain, TerrainAdjacency.DownLeft);
        //    if (terrain.TerrainType == downNeighbour.TerrainType && downNeighbour.TerrainType == leftNeighbour.TerrainType && GetWeightForTerrain(downLeftNeighbour) > GetWeightForTerrain(terrain))
        //    {
        //        canvas.CircularGradient(
        //            x: x,
        //            y: y + _trackParameters.CellSize - GradientWidth,
        //            width: GradientWidth,
        //            height: GradientWidth,
        //            circleCentreX: x,
        //            circleCentreY: y + _trackParameters.CellSize,
        //            circleRadius: GetGradientWidthBetweenTerrainTypes(downLeftNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(downLeftNeighbour, terrain)
        //        );
        //    }

        //    //Down Right Corner fragment gradient
        //    var downRightNeighbour = _terrainMap.GetAdjacentTerrain(terrain, TerrainAdjacency.DownRight);
        //    if (terrain.TerrainType == downNeighbour.TerrainType && downNeighbour.TerrainType == rightNeighbour.TerrainType && GetWeightForTerrain(downRightNeighbour) > GetWeightForTerrain(terrain))
        //    {
        //        canvas.CircularGradient(
        //            x: x + _trackParameters.CellSize - GradientWidth,
        //            y: y + _trackParameters.CellSize - GradientWidth,
        //            width: GradientWidth,
        //            height: GradientWidth,
        //            circleCentreX: x + _trackParameters.CellSize,
        //            circleCentreY: y + _trackParameters.CellSize,
        //            circleRadius: GetGradientWidthBetweenTerrainTypes(downRightNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(downRightNeighbour, terrain)
        //        );
        //    }

        //    //Top Left Corner 3 way, orthogonal sand
        //    if (terrain.TerrainType == TerrainType.Water
        //       && upNeighbour.TerrainType == TerrainType.Sand
        //       && leftNeighbour.TerrainType == TerrainType.Sand
        //       && upLeftNeighbour.TerrainType == TerrainType.Grass)
        //    {
        //        canvas.GradientRectTopLeftBottomtRight(
        //            x: x,
        //            y: y,
        //            width: GetGradientWidthBetweenTerrainTypes(upLeftNeighbour.TerrainType, terrain.TerrainType),
        //            height: GetGradientWidthBetweenTerrainTypes(upLeftNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(upLeftNeighbour, terrain)
        //        );
        //    }

        //    //Top Right Corner 3 way, orthogonal sand
        //    if (terrain.TerrainType == TerrainType.Water
        //       && upNeighbour.TerrainType == TerrainType.Sand
        //       && rightNeighbour.TerrainType == TerrainType.Sand
        //       && upRightNeighbour.TerrainType == TerrainType.Grass)
        //    {
        //        canvas.GradientRectTopRightBottomLeft(
        //            x: x + _trackParameters.CellSize - GradientWidth,
        //            y: y,
        //            width: GetGradientWidthBetweenTerrainTypes(upRightNeighbour.TerrainType, terrain.TerrainType),
        //            height: GetGradientWidthBetweenTerrainTypes(upRightNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(upRightNeighbour, terrain)
        //        );
        //    }

        //    //Down left Corner 3 way, orthogonal sand
        //    if (terrain.TerrainType == TerrainType.Water
        //       && downNeighbour.TerrainType == TerrainType.Sand
        //       && leftNeighbour.TerrainType == TerrainType.Sand
        //       && downLeftNeighbour.TerrainType == TerrainType.Grass)
        //    {
        //        canvas.GradientRectTopRightBottomLeft(
        //            x: x,
        //            y: y + _trackParameters.CellSize - GradientWidth,
        //            width: GetGradientWidthBetweenTerrainTypes(downLeftNeighbour.TerrainType, terrain.TerrainType),
        //            height: GetGradientWidthBetweenTerrainTypes(downLeftNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(terrain, downLeftNeighbour)
        //        );
        //    }

        //    //Down right Corner 3 way, orthogonal sand
        //    if (terrain.TerrainType == TerrainType.Water
        //       && downNeighbour.TerrainType == TerrainType.Sand
        //       && rightNeighbour.TerrainType == TerrainType.Sand
        //       && downRightNeighbour.TerrainType == TerrainType.Grass)
        //    {
        //        canvas.GradientRectTopLeftBottomtRight(
        //            x: x + _trackParameters.CellSize - GradientWidth,
        //            y: y + _trackParameters.CellSize - GradientWidth,
        //            width: GetGradientWidthBetweenTerrainTypes(downRightNeighbour.TerrainType, terrain.TerrainType),
        //            height: GetGradientWidthBetweenTerrainTypes(downRightNeighbour.TerrainType, terrain.TerrainType),
        //            colours: GetColoursForGradient(terrain, downRightNeighbour)
        //        );
        //    }

        //    //Top Left Corner, orthogonal different
        //    if (terrain.TerrainType == TerrainType.Water
        //       && upNeighbour.TerrainType == TerrainType.Grass
        //       && leftNeighbour.TerrainType == TerrainType.Sand)
        //    {
        //        canvas.CircularGradient(
        //             x: x,
        //             y: y + GradientWidth,
        //             width: GradientWidth,
        //             height: GradientWidth,
        //             circleCentreX: x + GradientWidth,
        //             circleCentreY: y + GradientWidth * 2,
        //             circleRadius: 2 * GradientWidth,
        //             colours: new Color[] { Colors.LightBlue, Colors.LightYellow }//GetColoursForGradient(terrain, leftNeighbour)
        //         );
        //    }
        //}
        #endregion

        private EdgeGradient CalculateEdgeGradient(GradientType gradientType, TerrainAdjacency adjacency)
        {
            var gradientWidth = GetGradientWidth(gradientType);
            return adjacency switch
            {
                TerrainAdjacency.Up => new EdgeGradient { XOffset = 0, YOffset = 0, Width = _trackParameters.CellSize, Height = gradientWidth, Colours = GetColoursForGradientType(gradientType) },
                TerrainAdjacency.Down => new EdgeGradient { XOffset = 0, YOffset = _trackParameters.CellSize - gradientWidth, Width = _trackParameters.CellSize, Height = gradientWidth, Colours = GetColoursForGradientType(gradientType).Reverse() },
                TerrainAdjacency.Left => new EdgeGradient { XOffset = 0, YOffset = 0, Width = gradientWidth, Height = _trackParameters.CellSize, Colours = GetColoursForGradientType(gradientType) },
                TerrainAdjacency.Right => new EdgeGradient { XOffset = _trackParameters.CellSize - gradientWidth, YOffset = 0, Width = gradientWidth, Height = _trackParameters.CellSize, Colours = GetColoursForGradientType(gradientType).Reverse() },
                _ => throw new NotImplementedException()
            };
        }

        private static float GetGradientWidth(GradientType gradientType)
        {
            return gradientType switch
            {
                GradientType.None => 0.0f,
                GradientType.GrassToWater => 2 * GradientWidth,
                _ => GradientWidth
            };
        }

        private static IEnumerable<Color> GetColoursForGradientType(GradientType gradientType)
        {
            return gradientType switch
            {
                GradientType.None => Array.Empty<Color>(),
                GradientType.SandToWater => new[] { TerrainColourMapper.GetColourForTerrainType(TerrainType.Sand), TerrainColourMapper.GetColourForTerrainType(TerrainType.Water) },
                GradientType.GrassToSand => new[] { TerrainColourMapper.GetColourForTerrainType(TerrainType.Grass), TerrainColourMapper.GetColourForTerrainType(TerrainType.Sand) },
                GradientType.GrassToWater => new[] { TerrainColourMapper.GetColourForTerrainType(TerrainType.Grass), TerrainColourMapper.GetColourForTerrainType(TerrainType.Sand), TerrainColourMapper.GetColourForTerrainType(TerrainType.Water) },
                _ => throw new NotImplementedException()
            };
        }

        private static float GetGradientOffsetForCornerJoinBetween(GradientType gradientType1, GradientType gradientType2)
        {
            if (gradientType1 == GradientType.GrassToWater && gradientType2 == GradientType.SandToWater)
            {
                return GradientWidth;
            }

            if (gradientType2 == GradientType.GrassToWater && gradientType1 == GradientType.SandToWater)
            {
                return -GradientWidth;
            }

            return 0;
        }
    }
}
