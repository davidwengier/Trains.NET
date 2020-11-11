using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    [Order(1)]
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

        public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
        {
            if (_terrainMap.Get(column, row).IsWater)
            {
                yield break;
            }

            yield return new Track() { Direction = TrackDirection.Horizontal };
            var neighbours = track.GetAllNeighbors();
            if (neighbours.Up is not null || neighbours.Down is not null)
            {
                yield return new Track() { Direction = TrackDirection.Vertical };
            }
            if (neighbours.Up is not null && neighbours.Left is not null)
            {
                yield return new Track() { Direction = TrackDirection.LeftUp };
            }
            if (neighbours.Up is not null && neighbours.Right is not null)
            {
                yield return new Track() { Direction = TrackDirection.RightUp };
            }
            if (neighbours.Down is not null && neighbours.Left is not null)
            {
                yield return new Track() { Direction = TrackDirection.LeftDown };
            }
            if (neighbours.Down is not null && neighbours.Right is not null)
            {
                yield return new Track() { Direction = TrackDirection.RightDown };
            }
        }
    }
}
