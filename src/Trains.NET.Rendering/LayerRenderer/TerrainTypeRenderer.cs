using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Trains.NET.Engine;
using Trains.NET.Engine.Terrain;
using Trains.NET.Rendering.Terrain;

namespace Trains.NET.Rendering
{
    [Order(0)]
    internal class TerrainTypeRenderer : ILayerRenderer
    {
        private readonly ITerrainMap _terrainMap;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _trackParameters;
        private readonly IGradientRenderer _gradientRenderer;

        public TerrainTypeRenderer(ITerrainMap terrainMap, IPixelMapper pixelMapper, ITrackParameters trackParameters, IGradientRenderer gradientRenderer)
        {
            _terrainMap = terrainMap;
            _pixelMapper = pixelMapper;
            _trackParameters = trackParameters;
            _gradientRenderer = gradientRenderer;
        }

        public bool Enabled { get; set; } = true;
        public string Name => "TerrainType";

        public void Render(ICanvas canvas, int width, int height)
        {
            DrawAllTerrainCells(canvas);
        }

        private void DrawAllTerrainCells(ICanvas canvas)
        {
            foreach (var terrain in _terrainMap)
            {
                DrawTerrainCell(canvas, terrain);
            }
        }

        private void DrawTerrainCell(ICanvas canvas, TerrainCell terrain)
        {
            var orthogonalGradients = new Dictionary<TerrainAdjacency, TerrainCellGradient>();
            // Find all edge gradients
            foreach(var adjacency in AdjacencyTypes.OrthogonalAdjacents)
            {
                var gradient = GenerateEdgeGradient(terrain, adjacency);
                orthogonalGradients[adjacency] = gradient;
            }

            var cornerGradients = new Dictionary<TerrainAdjacency, TerrainCellGradient>();
            // Use edge transitions to generate corner gradients
            foreach (var adjacency in AdjacencyTypes.CornerAdjacents)
            {
                var gradient = GenerateGradientForCornerCell(terrain, adjacency, orthogonalGradients);
                if (gradient.GradientType == GradientType.None) continue;
                cornerGradients[adjacency] = gradient;
            }

            RenderBackground(terrain, canvas);

            // Render edge gradients, highest weight last
            foreach (var gradient in orthogonalGradients.Values.Where(g => g.GradientType != GradientType.None).OrderBy(g => GetWeightForGradientType(g.GradientType))) {
                _gradientRenderer.Render(canvas, gradient);
            }

            // Render corner gradients, order does not matter so much because they have no overlap
            foreach(var gradient in cornerGradients.Values)
            {
                _gradientRenderer.Render(canvas, gradient);
            }
        }



        private void RenderBackground(TerrainCell terrain, ICanvas canvas)
        {
            var paintBrush = new PaintBrush
            {
                Color = TerrainColourMapper.GetColourForTerrainType(terrain.TerrainType),
                Style = PaintStyle.Fill,
                IsAntialias = true
            };

            (int x, int y) = _pixelMapper.CoordsToViewPortPixels(terrain.Column, terrain.Row);

            canvas.DrawRect(x, y, _trackParameters.CellSize, _trackParameters.CellSize, paintBrush);
        }

        private TerrainCellGradient GenerateGradientForCornerCell(TerrainCell terrain, TerrainAdjacency cornerAdjacency, Dictionary<TerrainAdjacency, TerrainCellGradient> gradients)
        {

            var sharedAdjacencies = GetSharedAdjacencies(cornerAdjacency);
            var sharedGradients = sharedAdjacencies.Select(adj => gradients[adj].GradientType);
            var cornerAdjacentTerrain = _terrainMap.GetAdjacentTerrain(terrain, cornerAdjacency);

            var gradientTypes = GenerateGradientTypesForCornerCell(terrain, cornerAdjacentTerrain, sharedGradients);

            return new TerrainCellGradient
            {
                Terrain = terrain,
                Adjacency = cornerAdjacency,
                GradientType = gradientTypes.First(),
                GradientTypeJoin = gradientTypes.Count() > 1 ? gradientTypes.Skip(1).First() : (GradientType?) null
            };
        }

        private IEnumerable<GradientType> GenerateGradientTypesForCornerCell(TerrainCell terrain, TerrainCell cornerAdjacentTerrain, IEnumerable<GradientType> sharedGradients)
        {
            // No gradient no either edge means we have an inverse gradient
            if (sharedGradients.First() == GradientType.None
               && sharedGradients.Last() == GradientType.None)
            {
                return new[] { GetGradientTypeBetweenTerrain(terrain, cornerAdjacentTerrain) };
            }

            // Gradients in both, join
            if (sharedGradients.First() != GradientType.None && sharedGradients.Last() != GradientType.None)
            {
                return sharedGradients.ToArray();
            }

            // This just leaves the case of one of them having a gradient, in which case we don't need a corner
            return new [] { GradientType.None};
        }

        private TerrainAdjacency[] GetSharedAdjacencies(TerrainAdjacency cornerAdjacency)
        {
            // The RHS of this should have the elements clockwise, it makes things easier
            return cornerAdjacency switch
            {
                TerrainAdjacency.UpLeft => new[] {TerrainAdjacency.Left, TerrainAdjacency.Up },
                TerrainAdjacency.UpRight => new[] { TerrainAdjacency.Up, TerrainAdjacency.Right },
                TerrainAdjacency.DownRight => new[] { TerrainAdjacency.Right, TerrainAdjacency.Down },
                TerrainAdjacency.DownLeft => new[] { TerrainAdjacency.Down, TerrainAdjacency.Left },
                _ => throw new NotImplementedException()
            };
        }

        private TerrainCellGradient GenerateEdgeGradient(TerrainCell terrain, TerrainAdjacency adjacency)
        {
            var adjacentTerrain = _terrainMap.GetAdjacentTerrain(terrain, adjacency);
            var gradientType = GetGradientTypeBetweenTerrain(terrain, adjacentTerrain);

            return new TerrainCellGradient
            {
                Terrain = terrain,
                Adjacency = adjacency,
                GradientType = gradientType
            };
        }

        private GradientType GetGradientTypeBetweenTerrain(TerrainCell sourceTerrain, TerrainCell adjacentTerrain)
        {
            if (adjacentTerrain.TerrainType == TerrainType.Empty
               || GetWeightForTerrain(adjacentTerrain) <= GetWeightForTerrain(sourceTerrain))
            {
                return GradientType.None;
            }

            return ResolveGradientTypeForTerrainTypes(adjacentTerrain.TerrainType, sourceTerrain.TerrainType);
        }
     
        private GradientType ResolveGradientTypeForTerrainTypes(TerrainType terrainTypeFrom, TerrainType terrainTypeTo)
        {
            if (terrainTypeFrom == TerrainType.Grass && terrainTypeTo == TerrainType.Sand) return GradientType.GrassToSand;
            if (terrainTypeFrom == TerrainType.Grass && terrainTypeTo == TerrainType.Water) return GradientType.GrassToWater;
            if (terrainTypeFrom == TerrainType.Sand && terrainTypeTo == TerrainType.Water) return GradientType.SandToWater;

            return GradientType.None;
        }

        private static int GetWeightForTerrain(TerrainCell terrain)
        {
            return terrain.TerrainType switch
            {
                TerrainType.Grass => 3,
                TerrainType.Sand => 2,
                TerrainType.Water => 1,
                TerrainType.Empty => 0,
                _ => throw new NotImplementedException()
            };
        }

        private static int GetWeightForGradientType(GradientType gradientType)
        {
            return gradientType switch
            {
                GradientType.GrassToWater => 1,
                _ => 0
            };
        }
    }
}
