using System.Diagnostics;

namespace Trains.NET.Engine;

[DebuggerDisplay("{Direction,nq}")]
public class SingleTrack : Track
{
    public override string Identifier => Direction.ToString();

    public SingleTrackDirection Direction { get; set; }

    public override void Move(TrainPosition position)
    {
        switch (Direction)
        {
            case SingleTrackDirection.Horizontal: TrainMovement.MoveHorizontal(position); break;
            case SingleTrackDirection.Vertical: TrainMovement.MoveVertical(position); break;
            case SingleTrackDirection.LeftUp: TrainMovement.MoveLeftUp(position); break;
            case SingleTrackDirection.RightUp: TrainMovement.MoveRightUp(position); break;
            case SingleTrackDirection.RightDown: TrainMovement.MoveRightDown(position); break;
            case SingleTrackDirection.LeftDown: TrainMovement.MoveLeftDown(position); break;
            default: throw new InvalidOperationException("I don't know what that track is!");
        }
    }

    public override bool IsConnectedRight()
        => Direction switch
        {
            SingleTrackDirection.RightDown => true,
            SingleTrackDirection.RightUp => true,
            SingleTrackDirection.Horizontal => true,
            _ => false
        };

    public override bool IsConnectedDown()
        => Direction switch
        {
            SingleTrackDirection.RightDown => true,
            SingleTrackDirection.LeftDown => true,
            SingleTrackDirection.Vertical => true,
            _ => false
        };

    public override bool IsConnectedLeft()
        => Direction switch
        {
            SingleTrackDirection.LeftDown => true,
            SingleTrackDirection.LeftUp => true,
            SingleTrackDirection.Horizontal => true,
            _ => false
        };

    public override bool IsConnectedUp()
        => Direction switch
        {
            SingleTrackDirection.LeftUp => true,
            SingleTrackDirection.RightUp => true,
            SingleTrackDirection.Vertical => true,
            _ => false
        };

    public void SetBestTrackDirection(bool ignoreHappyness)
    {
        var newDirection = GetBestTrackDirection(ignoreHappyness);

        if (Direction != newDirection)
        {
            Direction = newDirection;
            RefreshNeighbors(false);
        }

        ReevaluateHappiness();
    }

    public virtual SingleTrackDirection GetBestTrackDirection(bool ignoreHappyness)
    {
        var neighbors = GetPotentialNeighbors();
        var newDirection = Direction;

        if (neighbors.Count > 2)
        {
            return newDirection;
        }

        // Default direction
        if (neighbors.Count == 0)
        {
            newDirection = SingleTrackDirection.Horizontal;
        }
        else if (!Happy || ignoreHappyness)
        {
            // 2-way connections
            if (neighbors.Up != null && neighbors.Left != null)
            {
                newDirection = SingleTrackDirection.LeftUp;
            }
            else if (neighbors.Up != null && neighbors.Right != null)
            {
                newDirection = SingleTrackDirection.RightUp;
            }
            else if (neighbors.Down != null && neighbors.Left != null)
            {
                newDirection = SingleTrackDirection.LeftDown;
            }
            else if (neighbors.Down != null && neighbors.Right != null)
            {
                newDirection = SingleTrackDirection.RightDown;
            }
            // 1-way connection
            else if (neighbors.Up != null || neighbors.Down != null)
            {
                newDirection = SingleTrackDirection.Vertical;
            }
            else
            {
                newDirection = SingleTrackDirection.Horizontal;
            }
        }

        return newDirection;
    }

    public void RefreshNeighbors(bool ignoreHappyness)
    {
        var neighbors = GetAllNeighbors();
        (neighbors.Up as SingleTrack)?.SetBestTrackDirection(ignoreHappyness);
        (neighbors.Down as SingleTrack)?.SetBestTrackDirection(ignoreHappyness);
        (neighbors.Right as SingleTrack)?.SetBestTrackDirection(ignoreHappyness);
        (neighbors.Left as SingleTrack)?.SetBestTrackDirection(ignoreHappyness);
    }

    private TrackNeighbors GetPotentialNeighbors()
    {
        _ = TrackLayout ?? throw new InvalidOperationException("Game board can't be null");

        return new TrackNeighbors(
            TrackLayout.TryGet(Column - 1, Row, out Track? left) && left.CanConnectRight() ? left : null,
            TrackLayout.TryGet(Column, Row - 1, out Track? up) && up.CanConnectDown() ? up : null,
            TrackLayout.TryGet(Column + 1, Row, out Track? right) && right.CanConnectLeft() ? right : null,
            TrackLayout.TryGet(Column, Row + 1, out Track? down) && down.CanConnectUp() ? down : null
            );
    }

    public override void Created()
    {
        SetBestTrackDirection(false);
    }

    public override void Updated()
    {
        SetBestTrackDirection(true);
    }

    public override void Removed()
    {
        // We need to assume that we've already been removed from our parent, but before we go,
        // tell the neighbours we won't be back in the morning
        if (TrackLayout != null)
        {
            RefreshNeighbors(true);
        }
    }
}
