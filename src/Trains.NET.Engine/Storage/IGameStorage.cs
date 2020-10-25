using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface IGameStorage
    {
        IEnumerable<IEntity> ReadEntities();

        void WriteEntities(IEnumerable<IEntity> entities);

        IEnumerable<Terrain> ReadTerrain();

        void WriteTerrain(IEnumerable<Terrain> terrainList);
    }
}
