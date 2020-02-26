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

        internal (float RelativeLeft, float RelativeTop, float Angle, float Distance) Move(float relativeLeft, float relativeTop, float angle, float distance)
        => this.Direction switch
        {
            TrackDirection.LeftUp => MoveLeftUp(relativeLeft, relativeTop, angle, distance),
            TrackDirection.RightUp => throw null,
            TrackDirection.Horizontal => MoveHorizontal(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Vertical => MoveVertical(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Cross => throw null,
            TrackDirection.LeftUpDown => throw null,
            TrackDirection.LeftRightUp => throw null,
            TrackDirection.RightUpDown => throw null,
            _ => throw new System.Exception(null)
        };

        private static (float RelativeLeft, float RelativeTop, float Angle, float Distance) MoveLeftUp(float relativeLeft, float relativeTop, float angle, float distance)
        {
            // To travel 2PIr, we need to move 360
            // To travel x we need to move x/2PIr * 360
            // To travel x rad we need to move x/2PIr * 2PI
            // To travel x rad we need to move x/r

            float radius = 0.5f;
            double relAngle = Math.Atan2((double)relativeTop, (double)relativeLeft);
            double angleToMove = distance / radius;
            if (angle < 45.0f || angle > 225.0f)
            {
                
                if(relAngle - angleToMove < 0)
                {
                    double angleOver = angleToMove - relAngle;
                    relAngle = -0.001f;
                    distance = (float)(distance - angleOver * radius);
                }
                else
                {
                    relAngle -= angleToMove;
                    distance = 0;
                }
                angle = (float)(360 * relAngle / (2 * Math.PI)) - 90.0f;
                if (angle < 360) angle += 360;
            }
            else
            {
                if (relAngle + angleToMove > Math.PI / 2)
                {
                    double angleOver = (angleToMove + relAngle) - Math.PI / 2;
                    relAngle = (Math.PI / 2) + 0.001f;
                    distance = (float)(distance - angleOver * radius);
                }
                else
                {
                    relAngle += angleToMove;
                    distance = 0;
                }
                angle = (float)(360 * relAngle / (2 * Math.PI)) + 90.0f;
                if (angle > 360) angle -= 360;
            }

            relativeTop = (float)(radius * Math.Sin(relAngle));
            relativeLeft = (float)(radius * Math.Cos(relAngle));

            return (relativeLeft, relativeTop, angle, distance);
        }

        private static (float RelativeLeft, float RelativeTop, float Angle, float Distance) MoveVertical(float relativeLeft, float relativeTop, float angle, float distance)
        {
            // Snap left
            relativeLeft = 0.5f;

            // Snap angle
            if (angle < 180f)
            {
                angle = 90f;
                float toGo = 1.0f - relativeTop;

                if (distance < toGo)
                {
                    relativeTop += distance;
                    distance = 0;
                }
                else
                {
                    distance -= toGo;
                    relativeTop = 1.1f;
                }
            }
            else
            {
                angle = 270f;
                float toGo = relativeTop;

                if (distance < toGo)
                {
                    relativeTop -= distance;
                    distance = 0;
                }
                else
                {
                    distance -= toGo;
                    relativeTop = -0.1f;
                }
            }

            return (relativeLeft, relativeTop, angle, distance);
        }

        private static (float RelativeLeft, float RelativeTop, float Angle, float Distance) MoveHorizontal(float relativeLeft, float relativeTop, float angle, float distance)
        {
            // Snap top
            relativeTop = 0.5f;

            // Snap angle
            if (angle < 90f || angle > 270f)
            {
                angle = 0f;
                float toGo = 1.0f - relativeLeft;

                if (distance < toGo)
                {
                    relativeLeft += distance;
                    distance = 0;
                }
                else
                {
                    distance -= toGo;
                    relativeLeft = 1.1f;
                }
            }
            else
            {
                angle = 180f;
                float toGo = relativeLeft;

                if (distance < toGo)
                {
                    relativeLeft -= distance;
                    distance = 0;
                }
                else
                {
                    distance -= toGo;
                    relativeLeft = -0.1f;
                }
            }

            return (relativeLeft, relativeTop, angle, distance);
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
