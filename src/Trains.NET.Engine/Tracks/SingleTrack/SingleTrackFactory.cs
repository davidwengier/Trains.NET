using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    [Order(3)]
    public class SingleTrackFactory : IStaticEntityFactory<Track>
    {
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout<Track> _trackLayout;

        public SingleTrackFactory(ITerrainMap terrainMap, ILayout<Track> trackLayout)
        {
            _terrainMap = terrainMap;
            _trackLayout = trackLayout;
        }

        public bool TryCreateEntity(int column, int row, bool isPartOfDrag, int fromColumn, int fromRow, [NotNullWhen(returnValue: true)] out Track? entity)
        {
            entity = null;

            if (_terrainMap.Get(column, row).IsWater)
            {
                return false;
            }

            // this factory is never used to override
            if (_trackLayout.TryGet(column, row, out _))
            {
                return false;
            }

            entity = new SingleTrack();
            return true;
        }

        public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
        {
            if (_terrainMap.Get(column, row).IsWater)
            {
                yield break;
            }

            yield return new SingleTrack() { Direction = SingleTrackDirection.Horizontal };
            var neighbours = track.GetAllNeighbors();
            if (neighbours.Up is not null || neighbours.Down is not null)
            {
                yield return new SingleTrack() { Direction = SingleTrackDirection.Vertical };
            }
            if (neighbours.Up is not null && neighbours.Left is not null)
            {
                yield return new SingleTrack() { Direction = SingleTrackDirection.LeftUp };
            }
            if (neighbours.Up is not null && neighbours.Right is not null)
            {
                yield return new SingleTrack() { Direction = SingleTrackDirection.RightUp };
            }
            if (neighbours.Down is not null && neighbours.Left is not null)
            {
                yield return new SingleTrack() { Direction = SingleTrackDirection.LeftDown };
            }
            if (neighbours.Down is not null && neighbours.Right is not null)
            {
                yield return new SingleTrack() { Direction = SingleTrackDirection.RightDown };
            }
        }
    }
}
