using System.Collections;
using System.Collections.Immutable;

namespace Trains.NET.Engine;

public class TerrainMap : ITerrainMap, IInitializeAsync, IGameState
{
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

    public bool Load(IGameStorage storage)
    {
        var terrainSeed = storage.Read(nameof(TerrainMap));

        if (int.TryParse(terrainSeed, out var seed))
        {
            Reset(seed);
            return true;
        }

        return false;
    }

    public void Save(IGameStorage storage)
    {
        storage.Write(nameof(TerrainMap), _seed.ToString());
    }

    public Task InitializeAsync(int columns, int rows)
    {
        _columns = columns;
        _rows = rows;

        return Task.CompletedTask;
    }
}
