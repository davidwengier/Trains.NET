using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    [Order(2)]
    public class TIntersectionFactory : IStaticEntityFactory<Track>
    {
        public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
        {
            var neighbours = track.GetAllNeighbors();
            if (neighbours.Count < 3)
            {
                yield break;
            }

            if (AreAllPresent(neighbours.Up, neighbours.Left, neighbours.Right))
            {
                yield return new Track() { Direction = TrackDirection.LeftUp_RightUp };
                yield return new Track() { Direction = TrackDirection.LeftUp_RightUp, AlternateState = true };
            }
            if (AreAllPresent(neighbours.Up, neighbours.Left, neighbours.Down))
            {
                yield return new Track() { Direction = TrackDirection.LeftDown_LeftUp };
                yield return new Track() { Direction = TrackDirection.LeftDown_LeftUp, AlternateState = true };
            }
            if (AreAllPresent(neighbours.Up, neighbours.Right, neighbours.Down))
            {
                yield return new Track() { Direction = TrackDirection.RightUp_RightDown };
                yield return new Track() { Direction = TrackDirection.RightUp_RightDown, AlternateState = true };
            }
            if (AreAllPresent(neighbours.Down, neighbours.Left, neighbours.Right))
            {
                yield return new Track() { Direction = TrackDirection.RightDown_LeftDown };
                yield return new Track() { Direction = TrackDirection.RightDown_LeftDown, AlternateState = true };
            }
        }

        private static bool AreAllPresent(Track? track1, Track? track2, Track? track3)
            => track1 is not null
            && track2 is not null
            && track3 is not null;

        public bool TryCreateEntity(int column, int row, [NotNullWhen(true)] out Track? entity)
        {
            // TODO: This should work
            entity = null;
            return false;
        }
    }
}
