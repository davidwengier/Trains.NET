using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Trains.NET.Engine;

public class TerrainMap : ITerrainMap, IGameState
{
    private const string TerrainSeedConfigName = "TerrainSeed";

    private ImmutableDictionary<(int, int), Terrain> _terrainMap = ImmutableDictionary<(int, int), Terrain>.Empty;
    private readonly Random _newSeedRandom = new();

    public int Seed { get; private set; }

    public event EventHandler? CollectionChanged;

    public void Reset(int columns, int rows)
        => ResetToSeed(_newSeedRandom.Next(), columns, rows);

    public void ResetToSeed(int seed, int columns, int rows)
    {
        this.Seed = seed;

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

    public bool Load(IEnumerable<IEntity> entities, int columns, int rows)
    {
        var terrainSeed = entities.OfType<ConfigEntity>()
                                    .FirstOrDefault(x => x.Name == TerrainSeedConfigName);

        if (terrainSeed == null) return false;

        ResetToSeed(terrainSeed.Value, columns, rows);

        return true;
    }

    public IEnumerable<IEntity> Save()
    {
        yield return new ConfigEntity(TerrainSeedConfigName, this.Seed);
    }
}
