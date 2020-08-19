using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Tests
{
    internal class NullStorage : IGameStorage
    {
        public IEnumerable<IStaticEntity> ReadStaticEntities()
        {
            return Enumerable.Empty<IStaticEntity>();
        }

        public IEnumerable<Terrain> ReadTerrain()
        {
            return Enumerable.Empty<Terrain>();
        }

        public void WriteStaticEntities(IEnumerable<IStaticEntity> entities)
        {
        }

        public void WriteTerrain(IEnumerable<Terrain> terrainList)
        {
        }
    }
}
