using System;
using System.Diagnostics;

namespace Trains.NET.Engine
{
    [DebuggerDisplay("{Direction,nq}")]
    public class Track
    {
        private readonly IGameBoard _gameBoard;

        public Track(IGameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public int Column { get; set; }
        public int Row { get; set; }
        public TrackDirection Direction { get; set; }
        public bool Happy { get; set; }

        public bool CanConnectRight => this.Direction switch
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

        internal TrainPosition Move(float relativeLeft, float relativeTop, float angle, float distance)
        => this.Direction switch
        {
            TrackDirection.LeftUp => TrainMovement.MoveLeftUp(relativeLeft, relativeTop, angle, distance),
            TrackDirection.RightUp => TrainMovement.MoveRightUp(relativeLeft, relativeTop, angle, distance),
            TrackDirection.LeftDown => TrainMovement.MoveLeftDown(relativeLeft, relativeTop, angle, distance),
            TrackDirection.RightDown => TrainMovement.MoveRightDown(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Horizontal => TrainMovement.MoveHorizontal(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Vertical => TrainMovement.MoveVertical(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Cross => MoveCross(relativeLeft, relativeTop, angle, distance),

            TrackDirection.LeftRightDown => MoveLeftRightDown(relativeLeft, relativeTop, angle, distance),
            TrackDirection.LeftUpDown => MoveLeftUpDown(relativeLeft, relativeTop, angle, distance),
            TrackDirection.LeftRightUp => MoveLeftRightUp(relativeLeft, relativeTop, angle, distance),
            TrackDirection.RightUpDown => MoveRightUpDown(relativeLeft, relativeTop, angle, distance),

            _ => throw new System.Exception(null)
        };

        private static TrainPosition MoveLeftRightDown(float relativeLeft, float relativeTop, float angle, float distance)
        {
            // if from left, its a left down track
            if (angle <= 90.0)
            {
                return TrainMovement.MoveLeftDown(relativeLeft, relativeTop, angle, distance);
            }

            return TrainMovement.MoveRightDown(relativeLeft, relativeTop, angle, distance);
        }

        private static TrainPosition MoveLeftUpDown(float relativeLeft, float relativeTop, float angle, float distance)
        {
            // if from left, its a left down track
            if (TrainMovement.BetweenAngles(angle, 89, 181))
            {
                return TrainMovement.MoveLeftUp(relativeLeft, relativeTop, angle, distance);
            }

            return TrainMovement.MoveLeftDown(relativeLeft, relativeTop, angle, distance);
        }

        private static TrainPosition MoveLeftRightUp(float relativeLeft, float relativeTop, float angle, float distance)
        {
            // if from left, its a left down track
            if (TrainMovement.BetweenAngles(angle, 179, 271))
            {
                return TrainMovement.MoveRightUp(relativeLeft, relativeTop, angle, distance);
            }

            return TrainMovement.MoveLeftUp(relativeLeft, relativeTop, angle, distance);
        }

        private static TrainPosition MoveRightUpDown(float relativeLeft, float relativeTop, float angle, float distance)
        {
            // if from left, its a left down track
            if (angle >= 270.0)
            {
                return TrainMovement.MoveRightDown(relativeLeft, relativeTop, angle, distance);
            }

            return TrainMovement.MoveRightUp(relativeLeft, relativeTop, angle, distance);
        }


        private static TrainPosition MoveCross(float relativeLeft, float relativeTop, float angle, float distance)
        {
            if ((angle > 45.0f && angle < 135.0f) ||
                (angle > 225.0f && angle < 315.0f))
            {
                return TrainMovement.MoveVertical(relativeLeft, relativeTop, angle, distance);
            }
            else
            {
                return TrainMovement.MoveHorizontal(relativeLeft, relativeTop, angle, distance);
            }
        }

        public bool CanConnectDown => this.Direction switch
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

        public bool CanConnectLeft => this.Direction switch
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

        public bool CanConnectUp => this.Direction switch
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

            if (this.Direction != newDirection)
            {
                this.Direction = newDirection;
                RefreshNeighbors(false);
            }

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
            Track? left = _gameBoard.GetTrackAt(this.Column - 1, this.Row);
            Track? up = _gameBoard.GetTrackAt(this.Column, this.Row - 1);
            Track? right = _gameBoard.GetTrackAt(this.Column + 1, this.Row);
            Track? down = _gameBoard.GetTrackAt(this.Column, this.Row + 1);

            return new TrackNeighbors(
                left?.CanConnectRight == true ? left : null,
                up?.CanConnectDown == true ? up : null,
                right?.CanConnectLeft == true ? right : null,
                down?.CanConnectUp == true ? down : null
                );
        }

        public TrackNeighbors GetAllNeighbors()
        {
            Track? left = _gameBoard.GetTrackAt(this.Column - 1, this.Row);
            Track? up = _gameBoard.GetTrackAt(this.Column, this.Row - 1);
            Track? right = _gameBoard.GetTrackAt(this.Column + 1, this.Row);
            Track? down = _gameBoard.GetTrackAt(this.Column, this.Row + 1);

            return new TrackNeighbors(left, up, right, down);
        }
    }
}
