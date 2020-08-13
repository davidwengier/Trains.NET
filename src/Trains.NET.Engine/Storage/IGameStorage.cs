using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface IGameStorage
    {
        IEnumerable<IStaticEntity> ReadStaticEntities();

        void WriteStaticEntities(IEnumerable<IStaticEntity> entities);

        IEnumerable<Terrain> ReadTerrain();

        void WriteTerrain(IEnumerable<Terrain> terrainList);
    }
}
