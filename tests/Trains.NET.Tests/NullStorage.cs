using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class NullStorage : IGameStorage
{
    public string ReadEntities()
    {
        return null;
    }

    public int? ReadTerrainSeed()
    {
        return null;
    }

    public void WriteEntities(string entities)
    {
    }

    public void WriteTerrainSeed(int terrainSeed)
    {
    }
}
