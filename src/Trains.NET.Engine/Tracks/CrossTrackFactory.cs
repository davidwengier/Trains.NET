using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    [Order(1)]
    public class CrossTrackFactory : IStaticEntityFactory<Track>
    {
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout<Track> _layout;

        public CrossTrackFactory(ITerrainMap terrainMap, ILayout<Track> layout)
        {
            _terrainMap = terrainMap;
            _layout = layout;
        }

        public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
        {
            if (_terrainMap.Get(column, row).IsWater)
            {
                yield break;
            }

            var neighbours = track.GetAllNeighbors();
            if (neighbours.Count == 4)
            {
                yield return new CrossTrack();
            }
        }

        public bool TryCreateEntity(int column, int row, bool isPartOfDrag, [NotNullWhen(true)] out Track? entity)
        {
            var neighbours = TrackNeighbors.GetConnectedNeighbours(_layout.ToLayout(), column, row, emptyIsConsideredConnected: true);

            // if they click and its the perfect spot for a cross track, just do it
            if (!isPartOfDrag && neighbours.Count == 4)
            {
                entity = new CrossTrack();
                return true;
            }

            if (isPartOfDrag)
            {
                if (neighbours.Count == 4)
                {
                    entity = new CrossTrack();
                    return true;
                }

                // if they're dragging, we're looking for them to complete an intersection
                neighbours = TrackNeighbors.GetConnectedNeighbours(_layout.ToLayout(), column - 1, row);
                if (neighbours.Count == 3 && neighbours.Right is null)
                {
                    entity = new Track() { Direction = TrackDirection.Horizontal };
                    _layout.Set(column - 1, row, new CrossTrack());
                    return true;
                }

                neighbours = TrackNeighbors.GetConnectedNeighbours(_layout.ToLayout(), column, row - 1);
                if (neighbours.Count == 3 && neighbours.Down is null)
                {
                    entity = new Track() { Direction = TrackDirection.Vertical };
                    _layout.Set(column, row - 1, new CrossTrack());
                    return true;
                }

                neighbours = TrackNeighbors.GetConnectedNeighbours(_layout.ToLayout(), column + 1, row);
                if (neighbours.Count == 3 && neighbours.Left is null)
                {
                    entity = new Track() { Direction = TrackDirection.Horizontal };
                    _layout.Set(column + 1, row, new CrossTrack());
                    return true;
                }

                neighbours = TrackNeighbors.GetConnectedNeighbours(_layout.ToLayout(), column, row + 1);
                if (neighbours.Count == 3 && neighbours.Up is null)
                {
                    entity = new Track() { Direction = TrackDirection.Vertical };
                    _layout.Set(column, row + 1, new CrossTrack());
                    return true;
                }
            }

            entity = null;
            return false;
        }
    }
}
