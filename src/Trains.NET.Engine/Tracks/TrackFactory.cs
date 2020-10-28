using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    public class TrackFactory : IStaticEntityFactory<Track>
    {
        private readonly ITerrainMap _terrainMap;

        public TrackFactory(ITerrainMap terrainMap)
        {
            _terrainMap = terrainMap;
        }

        public bool TryCreateEntity(int column, int row, [NotNullWhen(returnValue: true)] out Track? entity)
        {
            if (_terrainMap.Get(column, row).IsWater)
            {
                entity = null;
                return false;
            }

            entity = new Track();
            return true;
        }

        public IEnumerable<Track> GetAllPossibleEntities(int column, int row)
        {
            if (!_terrainMap.Get(column, row).IsWater)
            {
                yield return new Track();
            }
        }
    }
}
