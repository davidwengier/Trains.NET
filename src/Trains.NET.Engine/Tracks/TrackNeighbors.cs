namespace Trains.NET.Engine
{
    public class TrackNeighbors
    {
        public Track? Left { get; }
        public Track? Up { get; }
        public Track? Right { get; }
        public Track? Down { get; }

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
