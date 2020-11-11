using System;
using System.Diagnostics;

namespace Trains.NET.Engine
{
    [DebuggerDisplay("{Direction,nq}")]
    public class Track : IStaticEntity
    {
        private ILayout? _trackLayout;

        public Track()
        {
        }

        /// <summary>
        /// Gets a string that represents the current tracks state
        /// </summary>
        public virtual string Identifier
            => $"{this.Direction}.{this.AlternateState}";

        public int Column { get; set; }
        public int Row { get; set; }
        public TrackDirection Direction { get; set; }
        public bool Happy { get; set; }
        public bool AlternateState { get; set; }
        public virtual bool HasMultipleStates
            => this.Direction switch
            {
                TrackDirection.RightUp_RightDown => true,
                TrackDirection.RightDown_LeftDown => true,
                TrackDirection.LeftDown_LeftUp => true,
                TrackDirection.LeftUp_RightUp => true,
                _ => false
            };

        internal virtual void Move(TrainPosition position)
        {
            switch (this.Direction)
            {
                case TrackDirection.Horizontal: TrainMovement.MoveHorizontal(position); break;
                case TrackDirection.Vertical: TrainMovement.MoveVertical(position); break;
                case TrackDirection.LeftUp: TrainMovement.MoveLeftUp(position); break;
                case TrackDirection.RightUp: TrainMovement.MoveRightUp(position); break;
                case TrackDirection.RightDown: TrainMovement.MoveRightDown(position); break;
                case TrackDirection.LeftDown: TrainMovement.MoveLeftDown(position); break;
                case TrackDirection.RightUp_RightDown: MoveRightUpDown(position); break;
                case TrackDirection.RightDown_LeftDown: MoveLeftRightDown(position); break;
                case TrackDirection.LeftDown_LeftUp: MoveLeftUpDown(position); break;
                case TrackDirection.LeftUp_RightUp: MoveLeftRightUp(position); break;
                default: throw new InvalidOperationException("I don't know what that track is!");
            }
        }

        public virtual void NextState()
        {
            if (this.HasMultipleStates)
            {
                this.AlternateState = !this.AlternateState;

                OnChanged();
            }
        }

        protected void OnChanged()
        {
            _trackLayout?.RaiseCollectionChanged();
        }

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

        public virtual bool IsBlocked() => false;

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

        private bool CanConnectRight()
            => !this.Happy || IsConnectedRight();
        private bool CanConnectDown()
            => !this.Happy || IsConnectedDown();
        private bool CanConnectLeft()
            => !this.Happy || IsConnectedLeft();
        private bool CanConnectUp()
            => !this.Happy || IsConnectedUp();

        public virtual bool IsConnectedRight()
            => this.Direction switch
            {
                TrackDirection.RightDown => true,
                TrackDirection.RightUp => true,
                TrackDirection.Horizontal => true,
                TrackDirection.RightDown_LeftDown => true,
                TrackDirection.LeftUp_RightUp => true,
                TrackDirection.RightUp_RightDown => true,
                _ => false
            };

        public virtual bool IsConnectedDown()
            => this.Direction switch
            {
                TrackDirection.RightDown => true,
                TrackDirection.LeftDown => true,
                TrackDirection.Vertical => true,
                TrackDirection.RightDown_LeftDown => true,
                TrackDirection.LeftDown_LeftUp => true,
                TrackDirection.RightUp_RightDown => true,
                _ => false
            };

        public virtual bool IsConnectedLeft()
            => this.Direction switch
            {
                TrackDirection.LeftDown => true,
                TrackDirection.LeftUp => true,
                TrackDirection.Horizontal => true,
                TrackDirection.RightDown_LeftDown => true,
                TrackDirection.LeftUp_RightUp => true,
                TrackDirection.LeftDown_LeftUp => true,
                _ => false
            };

        public virtual bool IsConnectedUp()
            => this.Direction switch
            {
                TrackDirection.LeftUp => true,
                TrackDirection.RightUp => true,
                TrackDirection.Vertical => true,
                TrackDirection.LeftDown_LeftUp => true,
                TrackDirection.LeftUp_RightUp => true,
                TrackDirection.RightUp_RightDown => true,
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

        public virtual TrackDirection GetBestTrackDirection(bool ignoreHappyness)
        {
            TrackNeighbors neighbors = GetNeighbors();
            TrackDirection newDirection = this.Direction;

            // Default direction
            if (neighbors.Count == 0)
            {
                newDirection = TrackDirection.Horizontal;
            }
            else if (!this.Happy || ignoreHappyness)
            {
                // 3-way connections
                if (neighbors.Up != null && neighbors.Left != null && neighbors.Down != null)
                {
                    newDirection = TrackDirection.LeftDown_LeftUp;
                }
                else if (neighbors.Up != null && neighbors.Right != null && neighbors.Down != null)
                {
                    newDirection = TrackDirection.RightUp_RightDown;
                }
                else if (neighbors.Up != null && neighbors.Left != null && neighbors.Right != null)
                {
                    newDirection = TrackDirection.LeftUp_RightUp;
                }
                else if (neighbors.Down != null && neighbors.Left != null && neighbors.Right != null)
                {
                    newDirection = TrackDirection.RightDown_LeftDown;
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
            this.Happy = GetConnectedNeighbors().Count > 1;
        }

        public void RefreshNeighbors(bool ignoreHappyness)
        {
            TrackNeighbors neighbors = GetAllNeighbors();
            neighbors.Up?.SetBestTrackDirection(ignoreHappyness);
            neighbors.Down?.SetBestTrackDirection(ignoreHappyness);
            neighbors.Right?.SetBestTrackDirection(ignoreHappyness);
            neighbors.Left?.SetBestTrackDirection(ignoreHappyness);
        }

        public TrackNeighbors GetConnectedNeighbors()
        {
            _ = _trackLayout ?? throw new InvalidOperationException("Game board can't be null");

            return TrackNeighbors.GetConnectedNeighbours(_trackLayout, this.Column, this.Row);
        }

        public TrackNeighbors GetNeighbors()
        {
            _ = _trackLayout ?? throw new InvalidOperationException("Game board can't be null");

            return new TrackNeighbors(
                _trackLayout.TryGet(this.Column - 1, this.Row, out Track? left) && left.CanConnectRight() ? left : null,
                _trackLayout.TryGet(this.Column, this.Row - 1, out Track? up) && up.CanConnectDown() ? up : null,
                _trackLayout.TryGet(this.Column + 1, this.Row, out Track? right) && right.CanConnectLeft() ? right : null,
                _trackLayout.TryGet(this.Column, this.Row + 1, out Track? down) && down.CanConnectUp() ? down : null
                );
        }

        public TrackNeighbors GetAllNeighbors()
        {
            _ = _trackLayout ?? throw new InvalidOperationException("Game board can't be null");

            return new TrackNeighbors(
                _trackLayout.TryGet(this.Column - 1, this.Row, out Track? left) ? left : null,
                _trackLayout.TryGet(this.Column, this.Row - 1, out Track? up) ? up : null,
                _trackLayout.TryGet(this.Column + 1, this.Row, out Track? right) ? right : null,
                _trackLayout.TryGet(this.Column, this.Row + 1, out Track? down) ? down : null
                );
        }

        public void Stored(ILayout? collection)
        {
            _trackLayout = collection;
        }

        public void Created()
        {
            SetBestTrackDirection(false);
        }

        public void Updated()
        {
            SetBestTrackDirection(true);
        }

        public void Replaced()
        {
            ReevaluateHappiness();
            var neighbours = GetAllNeighbors();
            foreach (var n in neighbours.All)
            {
                n.ReevaluateHappiness();
            }
        }

        public void Removed()
        {
            // We need to assume that we've already been removed from our parent, but before we go,
            // tell the neighbours we won't be back in the morning
            if (_trackLayout != null)
            {
                RefreshNeighbors(true);
            }
        }
    }
}
