using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    [Order(3)]
    public class CrossTrackFactory : IStaticEntityFactory<Track>
    {
        public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
        {
            var neighbours = track.GetAllNeighbors();
            if (neighbours.Count == 4)
            {
                yield return new Track() { Direction = TrackDirection.Cross };
            }
        }

        public bool TryCreateEntity(int column, int row, [NotNullWhen(true)] out Track? entity)
        {
            // TODO: This should work
            entity = null;
            return false;
        }
    }
}
