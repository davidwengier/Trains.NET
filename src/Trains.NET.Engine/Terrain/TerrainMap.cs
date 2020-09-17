using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Trains.NET.Engine
{
    public class TerrainMap : ITerrainMap
    {
        private ImmutableDictionary<(int, int), Terrain> _terrainMap = ImmutableDictionary<(int, int), Terrain>.Empty;

        public event EventHandler? CollectionChanged;

        public void Reset(int seed, int columns, int rows)
        {
            Dictionary<(int x, int y), float>? noiseMap = NoiseGenerator.GenerateNoiseMap(columns, rows, 4, seed);

            var builder = ImmutableDictionary.CreateBuilder<(int, int), Terrain>();
            foreach (var coord in noiseMap.Keys)
            {
                builder.Add(coord, new Terrain
                {
                    Column = coord.x,
                    Row = coord.y,
                    Height = (int)(noiseMap[coord] * Terrain.MaxHeight)
                });
            }
            _terrainMap = builder.ToImmutable();

            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetTerrainHeight(int column, int row, int height)
        {
            GetOrAdd(column, row).Height = height;
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Set(IEnumerable<Terrain> terrainList)
        {
            _terrainMap = terrainList.ToImmutableDictionary(t => (t.Column, t.Row));
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private Terrain GetOrAdd(int column, int row)
        {
            return ImmutableInterlocked.GetOrAdd(ref _terrainMap, (column, row), key => new Terrain { Column = key.Item1, Row = key.Item2 });
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

        public Terrain GetTerrainOrDefault(int column, int row)
        {
            if (_terrainMap.TryGetValue((column, row), out Terrain? adjacentTerrain))
            {
                return adjacentTerrain;
            }

            return new Terrain
            {
                Row = row,
                Column = column,
                Height = 0,
            };
        }

        public bool IsEmpty() => _terrainMap.IsEmpty;
    }
}
