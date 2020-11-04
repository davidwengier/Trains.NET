using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public class TrackNeighbors
    {
        public Track? Left { get; }
        public Track? Up { get; }
        public Track? Right { get; }
        public Track? Down { get; }

        public IEnumerable<Track> All
        {
            get
            {
                if (this.Up is not null) yield return this.Up;
                if (this.Left is not null) yield return this.Left;
                if (this.Right is not null) yield return this.Right;
                if (this.Down is not null) yield return this.Down;
            }
        }

        public int Count => (this.Up == null ? 0 : 1) +
                (this.Down == null ? 0 : 1) +
                (this.Right == null ? 0 : 1) +
                (this.Left == null ? 0 : 1);

        public bool Contains(Track otherTrack)
        {
            return otherTrack != null &&
                (this.Left == otherTrack ||
                this.Right == otherTrack ||
                this.Up == otherTrack ||
                this.Down == otherTrack);
        }

        public TrackNeighbors(Track? left, Track? up, Track? right, Track? down)
        {
            this.Left = left;
            this.Up = up;
            this.Right = right;
            this.Down = down;
        }
    }
}
