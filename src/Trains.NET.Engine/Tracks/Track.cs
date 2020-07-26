using System;
using System.Diagnostics;
using Trains.NET.Engine.Tracks;

namespace Trains.NET.Engine
{
    [DebuggerDisplay("{Direction,nq}")]
    public class Track
    {
        private readonly ITrackLayout? _trackLayout;

        public Track(ITrackLayout? trackLayout)
        {
            _trackLayout = trackLayout;
        }

        public int Column { get; set; }
        public int Row { get; set; }
        public TrackDirection Direction { get; set; }
        public bool Happy { get; set; }
        public bool AlternateState { get; set; }

        internal void Move(TrainPosition position)
        {
            switch (this.Direction)
            {
                case TrackDirection.Horizontal: TrainMovement.MoveHorizontal(position); break;
                case TrackDirection.Vertical: TrainMovement.MoveVertical(position); break;
                case TrackDirection.LeftUp: TrainMovement.MoveLeftUp(position); break;
                case TrackDirection.RightUp: TrainMovement.MoveRightUp(position); break;
                case TrackDirection.RightDown: TrainMovement.MoveRightDown(position); break;
                case TrackDirection.LeftDown: TrainMovement.MoveLeftDown(position); break;
                case TrackDirection.RightUpDown: MoveRightUpDown(position); break;
                case TrackDirection.LeftRightDown: MoveLeftRightDown(position); break;
                case TrackDirection.LeftUpDown: MoveLeftUpDown(position); break;
                case TrackDirection.LeftRightUp: MoveLeftRightUp(position); break;
                case TrackDirection.Cross: MoveCross(position); break;
                default: throw new InvalidOperationException("I don't know what that track is!");
            }
        }

        public bool HasAlternateState()
            => this.Direction switch
            {
                TrackDirection.RightUpDown => true,
                TrackDirection.LeftRightDown => true,
                TrackDirection.LeftUpDown => true,
                TrackDirection.LeftRightUp => true,
                _ => false
            };

        private void MoveLeftRightDown(TrainPosition position)
        {
            // Check single track extremes, as there are 2 places where the
            //  train angle could be at 0 degrees
            if (position.RelativeLeft < 0.4f)
            {
                TrainMovement.MoveLeftDown(position);
            }
            else if (position.RelativeLeft > 0.6f)
            {
                TrainMovement.MoveRightDown(position);
            }
            else if (position.Angle <= 90.0)
            {
                TrainMovement.MoveLeftDown(position);
            }
            else
            {
                if (this.AlternateState)
                {
                    TrainMovement.MoveLeftDown(position);
                }
                else
                {
                    TrainMovement.MoveRightDown(position);
                }
            }
        }

        private void MoveLeftUpDown(TrainPosition position)
        {
            if (position.RelativeTop < 0.4f)
            {
                TrainMovement.MoveLeftUp(position);
            }
            else if (position.RelativeTop > 0.6f)
            {
                TrainMovement.MoveLeftDown(position);
            }
            else if (TrainMovement.BetweenAngles(position.Angle, 89, 181))
            {
                TrainMovement.MoveLeftUp(position);
            }
            else
            {
                if (this.AlternateState)
                {
                    TrainMovement.MoveLeftUp(position);
                }
                else
                {
                    TrainMovement.MoveLeftDown(position);
                }
            }
        }

        private void MoveLeftRightUp(TrainPosition position)
        {
            if (position.RelativeLeft < 0.4f)
            {
                TrainMovement.MoveLeftUp(position);
            }
            else if (position.RelativeLeft > 0.6f)
            {
                TrainMovement.MoveRightUp(position);
            }
            else if (TrainMovement.BetweenAngles(position.Angle, 179, 271))
            {
                TrainMovement.MoveRightUp(position);
            }
            else
            {
                if (this.AlternateState)
                {
                    TrainMovement.MoveRightUp(position);
                }
                else
                {
                    TrainMovement.MoveLeftUp(position);
                }
            }
        }

        private void MoveRightUpDown(TrainPosition position)
        {
            // Right -> Up, Enters 180, Leaves 270
            // Up -> Right, Enters 90, Leaves 0
            // Down -> Right, Enters 270, Leaves 0
            if (position.RelativeTop < 0.4f)
            {
                TrainMovement.MoveRightUp(position);
            }
            else if (position.RelativeTop > 0.6f)
            {
                TrainMovement.MoveRightDown(position);
            }
            else if (position.Angle >= 270.0)
            {
                TrainMovement.MoveRightDown(position);
            }
            else
            {
                if (this.AlternateState)
                {
                    TrainMovement.MoveRightDown(position);
                }
                else
                {
                    TrainMovement.MoveRightUp(position);
                }
            }
        }

        private static void MoveCross(TrainPosition position)
        {
            if ((position.Angle > 45.0f && position.Angle < 135.0f) ||
                (position.Angle > 225.0f && position.Angle < 315.0f))
            {
                TrainMovement.MoveVertical(position);
            }
            else
            {
                TrainMovement.MoveHorizontal(position);
            }
        }
        private bool CanConnectRight() => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.RightDown => true,
            TrackDirection.RightUp => true,
            TrackDirection.Horizontal => true,
            TrackDirection.Cross => true,
            TrackDirection.LeftRightDown => true,
            TrackDirection.LeftRightUp => true,
            TrackDirection.RightUpDown => true,
            _ => false
        };

        private bool CanConnectDown() => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.RightDown => true,
            TrackDirection.LeftDown => true,
            TrackDirection.Vertical => true,
            TrackDirection.Cross => true,
            TrackDirection.LeftRightDown => true,
            TrackDirection.LeftUpDown => true,
            TrackDirection.RightUpDown => true,
            _ => false
        };

        private bool CanConnectLeft() => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.LeftDown => true,
            TrackDirection.LeftUp => true,
            TrackDirection.Horizontal => true,
            TrackDirection.Cross => true,
            TrackDirection.LeftRightDown => true,
            TrackDirection.LeftRightUp => true,
            TrackDirection.LeftUpDown => true,
            _ => false
        };

        private bool CanConnectUp() => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.LeftUp => true,
            TrackDirection.RightUp => true,
            TrackDirection.Vertical => true,
            TrackDirection.Cross => true,
            TrackDirection.LeftUpDown => true,
            TrackDirection.LeftRightUp => true,
            TrackDirection.RightUpDown => true,
            _ => false
        };

        public void SetBestTrackDirection(bool ignoreHappyness)
        {
            TrackDirection newDirection = GetBestTrackDirection(ignoreHappyness);

            if (this.Direction != newDirection)
            {
                this.Direction = newDirection;
                this.AlternateState = false;
                RefreshNeighbors(false);
            }

            ReevaluateHappiness();
        }

        public TrackDirection GetBestTrackDirection(bool ignoreHappyness)
        {
            TrackNeighbors neighbors = GetNeighbors();
            TrackDirection newDirection = this.Direction;

            // Default direction
            if (neighbors.Count == 0)
            {
                newDirection = TrackDirection.Horizontal;
            }
            else if (neighbors.Count == 4)
            {
                newDirection = TrackDirection.Cross;
            }
            else if (!this.Happy || ignoreHappyness)
            {
                // 3-way connections
                if (neighbors.Up != null && neighbors.Left != null && neighbors.Down != null)
                {
                    newDirection = TrackDirection.LeftUpDown;
                }
                else if (neighbors.Up != null && neighbors.Right != null && neighbors.Down != null)
                {
                    newDirection = TrackDirection.RightUpDown;
                }
                else if (neighbors.Up != null && neighbors.Left != null && neighbors.Right != null)
                {
                    newDirection = TrackDirection.LeftRightUp;
                }
                else if (neighbors.Down != null && neighbors.Left != null && neighbors.Right != null)
                {
                    newDirection = TrackDirection.LeftRightDown;
                }
                // 2-way connections
                else if (neighbors.Up != null && neighbors.Left != null)
                {
                    newDirection = TrackDirection.LeftUp;
                }
                else if (neighbors.Up != null && neighbors.Right != null)
                {
                    newDirection = TrackDirection.RightUp;
                }
                else if (neighbors.Down != null && neighbors.Left != null)
                {
                    newDirection = TrackDirection.LeftDown;
                }
                else if (neighbors.Down != null && neighbors.Right != null)
                {
                    newDirection = TrackDirection.RightDown;
                }
                // 1-way connection
                else if (neighbors.Up != null || neighbors.Down != null)
                {
                    newDirection = TrackDirection.Vertical;
                }
                else
                {
                    newDirection = TrackDirection.Horizontal;
                }
            }

            return newDirection;
        }

        public void ReevaluateHappiness()
        {
            this.Happy = GetNeighbors().Count > 1;
        }

        public void RefreshNeighbors(bool ignoreHappyness)
        {
            TrackNeighbors neighbors = GetAllNeighbors();
            neighbors.Up?.SetBestTrackDirection(ignoreHappyness);
            neighbors.Down?.SetBestTrackDirection(ignoreHappyness);
            neighbors.Right?.SetBestTrackDirection(ignoreHappyness);
            neighbors.Left?.SetBestTrackDirection(ignoreHappyness);
        }

        public TrackNeighbors GetNeighbors()
        {
            _ = _trackLayout ?? throw new InvalidOperationException("Game board can't be null");

            return new TrackNeighbors(
                _trackLayout.TryGet(this.Column - 1, this.Row, out Track? left) && left.CanConnectRight() ? left : null,
                _trackLayout.TryGet(this.Column, this.Row - 1, out Track? up) && up.CanConnectDown() == true ? up : null,
                _trackLayout.TryGet(this.Column + 1, this.Row, out Track? right) && right.CanConnectLeft() == true ? right : null,
                _trackLayout.TryGet(this.Column, this.Row + 1, out Track? down) && down.CanConnectUp() == true ? down : null
                );
        }

        private TrackNeighbors GetAllNeighbors()
        {
            _ = _trackLayout ?? throw new InvalidOperationException("Game board can't be null");

            return new TrackNeighbors(
                _trackLayout.TryGet(this.Column - 1, this.Row, out Track? left) ? left : null,
                _trackLayout.TryGet(this.Column, this.Row - 1, out Track? up) ? up : null,
                _trackLayout.TryGet(this.Column + 1, this.Row, out Track? right) ? right : null,
                _trackLayout.TryGet(this.Column, this.Row + 1, out Track? down) ? down : null
                );
        }
    }
}
