using Trains.NET.Engine;

namespace BlazingTrains
{
    public class BlazorGameStorage : IGameStorage
    {
        public IEnumerable<IEntity> ReadEntities()
        {
            yield break;
        }

        public IEnumerable<Terrain> ReadTerrain()
        {
            yield break;
        }

        public void WriteEntities(IEnumerable<IEntity> entities)
        {
            //throw new NotImplementedException();
        }

        public void WriteTerrain(IEnumerable<Terrain> terrainList)
        {
            //throw new NotImplementedException();
        }
    }
}
