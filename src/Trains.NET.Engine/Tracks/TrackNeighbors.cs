namespace Trains.NET.Engine;

public class TrackNeighbors(Track? left, Track? up, Track? right, Track? down)
{
    public Track? Left { get; } = left;
    public Track? Up { get; } = up;
    public Track? Right { get; } = right;
    public Track? Down { get; } = down;

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

    public static TrackNeighbors GetConnectedNeighbours(ILayout trackLayout, int column, int row, bool emptyIsConsideredConnected = false, bool ignoreCurrent = false)
    {
        trackLayout.TryGet(column, row, out Track? current);

        bool isConnectedLeft;
        bool isConnectedUp;
        bool isConnectedRight;
        bool isConnectedDown;
        if (ignoreCurrent || current is null)
        {
            isConnectedLeft = emptyIsConsideredConnected;
            isConnectedUp = emptyIsConsideredConnected;
            isConnectedRight = emptyIsConsideredConnected;
            isConnectedDown = emptyIsConsideredConnected;
        }
        else
        {
            isConnectedLeft = current.IsConnectedLeft();
            isConnectedUp = current.IsConnectedUp();
            isConnectedRight = current.IsConnectedRight();
            isConnectedDown = current.IsConnectedDown();
        }

        return new TrackNeighbors(
            trackLayout.TryGet(column - 1, row, out Track? left) && isConnectedLeft && left.IsConnectedRight() ? left : null,
            trackLayout.TryGet(column, row - 1, out Track? up) && isConnectedUp && up.IsConnectedDown() ? up : null,
            trackLayout.TryGet(column + 1, row, out Track? right) && isConnectedRight && right.IsConnectedLeft() ? right : null,
            trackLayout.TryGet(column, row + 1, out Track? down) && isConnectedDown && down.IsConnectedUp() ? down : null
            );
    }

    public static TrackNeighbors GetAllNeighbours(ILayout trackLayout, int column, int row)
    {
        return new TrackNeighbors(
            trackLayout.TryGet(column - 1, row, out Track? left) ? left : null,
            trackLayout.TryGet(column, row - 1, out Track? up) ? up : null,
            trackLayout.TryGet(column + 1, row, out Track? right) ? right : null,
            trackLayout.TryGet(column, row + 1, out Track? down) ? down : null
            );
    }
}
