using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Trains.NET.Engine;

public class TerrainMap : ITerrainMap, IInitializeAsync, IGameState
{
    private const string TerrainSeedConfigName = "TerrainSeed";

    private ImmutableDictionary<(int, int), Terrain> _terrainMap = ImmutableDictionary<(int, int), Terrain>.Empty;
    private int _columns;
    private int _rows;
    private readonly Random _newSeedRandom = new();

    private int _seed;

    public event EventHandler? CollectionChanged;

    void IGameState.Reset()
        => Reset(null);

    public void Reset(int? seed)
    {
        _seed = seed ?? _newSeedRandom.Next();

        Dictionary<(int x, int y), float>? noiseMap = NoiseGenerator.GenerateNoiseMap(_columns, _rows, 4, _seed);

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

    public bool Load(IEnumerable<IEntity> entities)
    {
        var terrainSeed = entities.OfType<ConfigEntity>()
                                    .FirstOrDefault(x => x.Name == TerrainSeedConfigName);

        if (terrainSeed == null) return false;

        Reset(terrainSeed.Value);

        return true;
    }

    public IEnumerable<IEntity> Save()
    {
        yield return new ConfigEntity(TerrainSeedConfigName, _seed);
    }

    public Task InitializeAsync(int columns, int rows)
    {
        _columns = columns;
        _rows = rows;

        return Task.CompletedTask;
    }
}
