using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Trains.NET.Engine;

public class TerrainMap : ITerrainMap
{
    private ImmutableDictionary<(int, int), Terrain> _terrainMap = ImmutableDictionary<(int, int), Terrain>.Empty;

    public event EventHandler? CollectionChanged;

    public void Reset(int seed, int columns, int rows)
    {
        Dictionary<(int x, int y), float>? noiseMap = NoiseGenerator.GenerateNoiseMap(columns, rows, 4, seed);

        ImmutableDictionary<(int, int), Terrain>.Builder builder = ImmutableDictionary.CreateBuilder<(int, int), Terrain>();
        foreach ((int x, int y) coord in noiseMap.Keys)
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

    public void Set(IEnumerable<Terrain> terrainList)
    {
        _terrainMap = terrainList.ToImmutableDictionary(t => (t.Column, t.Row));
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerator<Terrain> GetEnumerator()
    {
        return _terrainMap.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Terrain Get(int column, int row)
        => _terrainMap[(column, row)];
}
