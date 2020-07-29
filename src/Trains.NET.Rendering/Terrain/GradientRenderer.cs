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
