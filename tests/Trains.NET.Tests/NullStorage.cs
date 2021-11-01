using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class NullStorage : IGameStorage
{
    public IEnumerable<IEntity> ReadEntities()
    {
        return Enumerable.Empty<IEntity>();
    }

    public IEnumerable<Terrain> ReadTerrain()
    {
        return Enumerable.Empty<Terrain>();
    }

    public void WriteEntities(IEnumerable<IEntity> entities)
    {
    }

    public void WriteTerrain(IEnumerable<Terrain> terrainList)
    {
    }
}
