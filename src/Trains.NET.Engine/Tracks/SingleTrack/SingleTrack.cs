using System;
using System.Diagnostics;

namespace Trains.NET.Engine;

[DebuggerDisplay("{Direction,nq}")]
public class SingleTrack : Track
{
    public override string Identifier => this.Direction.ToString();

    public SingleTrackDirection Direction { get; set; }

    public override void Move(TrainPosition position)
    {
        switch (this.Direction)
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
        => this.Direction switch
        {
            SingleTrackDirection.RightDown => true,
            SingleTrackDirection.RightUp => true,
            SingleTrackDirection.Horizontal => true,
            _ => false
        };

    public override bool IsConnectedDown()
        => this.Direction switch
        {
            SingleTrackDirection.RightDown => true,
            SingleTrackDirection.LeftDown => true,
            SingleTrackDirection.Vertical => true,
            _ => false
        };

    public override bool IsConnectedLeft()
        => this.Direction switch
        {
            SingleTrackDirection.LeftDown => true,
            SingleTrackDirection.LeftUp => true,
            SingleTrackDirection.Horizontal => true,
            _ => false
        };

    public override bool IsConnectedUp()
        => this.Direction switch
        {
            SingleTrackDirection.LeftUp => true,
            SingleTrackDirection.RightUp => true,
            SingleTrackDirection.Vertical => true,
            _ => false
        };

    public void SetBestTrackDirection(bool ignoreHappyness)
    {
        SingleTrackDirection newDirection = GetBestTrackDirection(ignoreHappyness);

        if (this.Direction != newDirection)
        {
            this.Direction = newDirection;
            RefreshNeighbors(false);
        }

        ReevaluateHappiness();
    }

    public virtual SingleTrackDirection GetBestTrackDirection(bool ignoreHappyness)
    {
        TrackNeighbors neighbors = GetPotentialNeighbors();
        SingleTrackDirection newDirection = this.Direction;

        if (neighbors.Count > 2)
        {
            return newDirection;
        }

        // Default direction
        if (neighbors.Count == 0)
        {
            newDirection = SingleTrackDirection.Horizontal;
        }
        else if (!this.Happy || ignoreHappyness)
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
        TrackNeighbors neighbors = GetAllNeighbors();
        (neighbors.Up as SingleTrack)?.SetBestTrackDirection(ignoreHappyness);
        (neighbors.Down as SingleTrack)?.SetBestTrackDirection(ignoreHappyness);
        (neighbors.Right as SingleTrack)?.SetBestTrackDirection(ignoreHappyness);
        (neighbors.Left as SingleTrack)?.SetBestTrackDirection(ignoreHappyness);
    }

    private TrackNeighbors GetPotentialNeighbors()
    {
        _ = this.TrackLayout ?? throw new InvalidOperationException("Game board can't be null");

        return new TrackNeighbors(
            this.TrackLayout.TryGet(this.Column - 1, this.Row, out Track? left) && left.CanConnectRight() ? left : null,
            this.TrackLayout.TryGet(this.Column, this.Row - 1, out Track? up) && up.CanConnectDown() ? up : null,
            this.TrackLayout.TryGet(this.Column + 1, this.Row, out Track? right) && right.CanConnectLeft() ? right : null,
            this.TrackLayout.TryGet(this.Column, this.Row + 1, out Track? down) && down.CanConnectUp() ? down : null
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
        if (this.TrackLayout != null)
        {
            RefreshNeighbors(true);
        }
    }
}
