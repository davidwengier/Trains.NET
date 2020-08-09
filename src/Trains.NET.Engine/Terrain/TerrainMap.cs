using System;
using System.Collections;
using System.Collections.Generic;

namespace Trains.NET.Engine.Tracks
{
    internal class TerrainMap : ITerrainMap
    {
        private readonly Dictionary<(int, int), Terrain> _terrainMap = new();

        public void SetTerrainHeight(int column, int row, int height)
        {
            Func<Terrain, Terrain> transform = terrain => {
                terrain.Height = height;
                return terrain;
            };

            AddOrOverwrite(column, row, transform);
        }

        public void SetTerrainType(int column, int row, TerrainType type)
        {
            Func<Terrain, Terrain> transform = terrain => {
                terrain.TerrainType = type;
                return terrain;
            };

            AddOrOverwrite(column, row, transform);
        }

        private void AddOrOverwrite(int column, int row, Func<Terrain, Terrain> transform)
        {
            (int, int) key = (column, row);
            Terrain terrain = _terrainMap.ContainsKey(key)
                ? _terrainMap[key]
                : new Terrain { Column = column, Row = row };

            _terrainMap[key] = transform(terrain);
        }

        public IEnumerator<Terrain> GetEnumerator()
        {
            return _terrainMap.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Terrain GetAdjacentTerrainUp(Terrain terrain)
        {
            return GetTerrainOrDefault(terrain.Column, terrain.Row - 1);
        }

         public Terrain GetAdjacentTerrainDown(Terrain terrain)
        {
            return GetTerrainOrDefault(terrain.Column, terrain.Row + 1);
        }

        public Terrain GetAdjacentTerrainLeft(Terrain terrain)
        {
            return GetTerrainOrDefault(terrain.Column - 1, terrain.Row);
        }

        public Terrain GetAdjacentTerrainRight(Terrain terrain)
        {
            return GetTerrainOrDefault(terrain.Column + 1, terrain.Row);
        }

        private Terrain GetTerrainOrDefault(int column, int row)
        {
            if (_terrainMap.TryGetValue((column, row), out Terrain? adjacentTerrain))
            {
                return adjacentTerrain;
            }

            return new Terrain
            {
                Row = row,
                Column = column,
                Height = 0
            };
        }
    }
}
