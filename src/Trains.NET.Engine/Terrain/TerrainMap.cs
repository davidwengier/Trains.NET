using System;
using System.Collections;
using System.Collections.Generic;

namespace Trains.NET.Engine.Terrain
{
    internal class TerrainMap : ITerrainMap
    {
        private readonly Dictionary<(int, int), TerrainCell> _terrainMap = new Dictionary<(int, int), TerrainCell>();

        public void SetTerrainHeight(int column, int row, int height)
        {
            Func<TerrainCell, TerrainCell> transform = terrain => {
                terrain.Height = height;
                return terrain;
            };

            AddOrOverwrite(column, row, transform);
        }

        public void SetTerrainType(int column, int row, TerrainType type)
        {
            Func<TerrainCell, TerrainCell> transform = terrain => {
                terrain.TerrainType = type;
                return terrain;
            };

            AddOrOverwrite(column, row, transform);
        }

        private void AddOrOverwrite(int column, int row, Func<TerrainCell, TerrainCell> transform)
        {
            var key = (column, row);
            var terrain = _terrainMap.ContainsKey(key)
                ? _terrainMap[key]
                : new TerrainCell { Column = column, Row = row };

            _terrainMap[key] = transform(terrain);
        }

        public IEnumerator<TerrainCell> GetEnumerator()
        {
            return _terrainMap.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public TerrainCell GetAdjacentTerrain(TerrainCell terrain, TerrainAdjacency adjacency)
        {
            return adjacency switch
            {
                TerrainAdjacency.Up => GetTerrainOrDefault(terrain.Column, terrain.Row - 1),
                TerrainAdjacency.Down => GetTerrainOrDefault(terrain.Column, terrain.Row + 1),
                TerrainAdjacency.Left => GetTerrainOrDefault(terrain.Column - 1, terrain.Row),
                TerrainAdjacency.Right => GetTerrainOrDefault(terrain.Column + 1, terrain.Row),
                TerrainAdjacency.UpLeft => GetTerrainOrDefault(terrain.Column - 1, terrain.Row - 1),
                TerrainAdjacency.UpRight => GetTerrainOrDefault(terrain.Column + 1, terrain.Row - 1),
                TerrainAdjacency.DownLeft => GetTerrainOrDefault(terrain.Column - 1, terrain.Row + 1),
                TerrainAdjacency.DownRight => GetTerrainOrDefault(terrain.Column + 1, terrain.Row + 1),
                _ => throw new NotImplementedException()
            };
        }

        private TerrainCell GetTerrainOrDefault(int column, int row)
        {
            if (_terrainMap.TryGetValue((column, row), out var adjacentTerrain))
            {
                return adjacentTerrain;
            }

            return new TerrainCell
            {
                Row = row,
                Column = column,
                Height = 0
            };
        }

    }
}
