using System;
using System.Diagnostics;

namespace Trains.NET.Engine
{
    [DebuggerDisplay("{Direction,nq}")]
    public class Track
    {
        private const float RelativeCellRadius = 0.5f;
        private const float OutsideCellAnglePadding = 0.001f;
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
            TrackDirection.RightUp => MoveRightUp(relativeLeft, relativeTop, angle, distance),
            TrackDirection.LeftDown => MoveLeftDown(relativeLeft, relativeTop, angle, distance),
            TrackDirection.RightDown => MoveRightDown(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Horizontal => MoveHorizontal(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Vertical => MoveVertical(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Cross => MoveCross(relativeLeft, relativeTop, angle, distance),
            _ => throw new System.Exception(null)
        };

        private static (float RelativeLeft, float RelativeTop, float Angle, float Distance) MoveLeftDown(float relativeLeft, float relativeTop, float trainAngle, float distance)
        {
            // Find the angle within the tracks circle using the current position
            // This *should* be perpendicular to angle
            double currentAngle = MathHelpers.PointsToAngle(relativeLeft , relativeTop- 1.0f);

            // To travel 2PIr, we need to move 360
            // To travel x we need to move x/2PIr * 360
            // To travel x rad we need to move x/2PIr * 2PI
            // To travel x rad we need to move x/r
            double angleToMove = distance / RelativeCellRadius;

            // In order to figure out if we are moving clockwise or counter-clockwise, look at the angle of the train
            if (trainAngle > 135.0f && trainAngle < 315.0f)
            {
                // We are facing left/up, so we move counter clockwise, with a minimum angle of 90
                (currentAngle, distance) = MoveCounterClockwise(currentAngle, angleToMove, distance, -Math.PI / 2);

                trainAngle = (float)MathHelpers.RadToDegree(currentAngle) - 90.0f;
            }
            else
            {
                // We are NOT facing left/up, so we move clockwise, with a maximum angle of 180, Math.PI
                (currentAngle, distance) = MoveClockwise(currentAngle, angleToMove, distance, 0);

                trainAngle = (float)MathHelpers.RadToDegree(currentAngle) + 90.0f;
            }

            // Double check to keep our angle in range, this makes our angle checks easier!:
            trainAngle = MathHelpers.KeepWithin0and360(trainAngle);

            // Find our new position on the track
            (relativeLeft, relativeTop) = MathHelpers.AngleToPoints(currentAngle, RelativeCellRadius);

            return (relativeLeft, relativeTop + 1.0f, trainAngle, distance);
        }

        private static (float RelativeLeft, float RelativeTop, float Angle, float Distance) MoveRightDown(float relativeLeft, float relativeTop, float trainAngle, float distance)
        {
            // Find the angle within the tracks circle using the current position
            // This *should* be perpendicular to angle
            double currentAngle = MathHelpers.PointsToAngle(relativeLeft - 1.0f, relativeTop - 1.0f);

            // To travel 2PIr, we need to move 360
            // To travel x we need to move x/2PIr * 360
            // To travel x rad we need to move x/2PIr * 2PI
            // To travel x rad we need to move x/r
            double angleToMove = distance / RelativeCellRadius;

            // In order to figure out if we are moving clockwise or counter-clockwise, look at the angle of the train
            if (trainAngle > 45.0f && trainAngle < 220.0f)
            {
                // We are facing left/up, so we move counter clockwise, with a minimum angle of 90
                (currentAngle, distance) = MoveCounterClockwise(currentAngle, angleToMove, distance, -Math.PI);

                trainAngle = (float)MathHelpers.RadToDegree(currentAngle) - 90.0f;
            }
            else
            {
                // We are NOT facing left/up, so we move clockwise, with a maximum angle of 180, Math.PI
                (currentAngle, distance) = MoveClockwise(currentAngle, angleToMove, distance, 2 * Math.PI);

                trainAngle = (float)MathHelpers.RadToDegree(currentAngle) + 90.0f;
            }

            // Double check to keep our angle in range, this makes our angle checks easier!:
            trainAngle = MathHelpers.KeepWithin0and360(trainAngle);

            // Find our new position on the track
            (relativeLeft, relativeTop) = MathHelpers.AngleToPoints(currentAngle, RelativeCellRadius);

            return (relativeLeft + 1.0f, relativeTop + 1.0f, trainAngle, distance);
        }

        private static (float RelativeLeft, float RelativeTop, float Angle, float Distance) MoveRightUp(float relativeLeft, float relativeTop, float trainAngle, float distance)
        {
            // Find the angle within the tracks circle using the current position
            // This *should* be perpendicular to angle
            double currentAngle = MathHelpers.PointsToAngle(relativeLeft - 1.0f, relativeTop);

            // To travel 2PIr, we need to move 360
            // To travel x we need to move x/2PIr * 360
            // To travel x rad we need to move x/2PIr * 2PI
            // To travel x rad we need to move x/r
            double angleToMove = distance / RelativeCellRadius;

            // In order to figure out if we are moving clockwise or counter-clockwise, look at the angle of the train
            if (trainAngle > -45.0f && trainAngle < 135.0f)
            {
                // We are facing left/up, so we move counter clockwise, with a minimum angle of 90
                (currentAngle, distance) = MoveCounterClockwise(currentAngle, angleToMove, distance, Math.PI / 2);

                trainAngle = (float)MathHelpers.RadToDegree(currentAngle) - 90.0f;
            }
            else
            {
                // We are NOT facing left/up, so we move clockwise, with a maximum angle of 180, Math.PI
                (currentAngle, distance) = MoveClockwise(currentAngle, angleToMove, distance, Math.PI);

                trainAngle = (float)MathHelpers.RadToDegree(currentAngle) + 90.0f;
            }

            // Double check to keep our angle in range, this makes our angle checks easier!:
            trainAngle = MathHelpers.KeepWithin0and360(trainAngle);

            // Find our new position on the track
            (relativeLeft, relativeTop) = MathHelpers.AngleToPoints(currentAngle, RelativeCellRadius);

            return (relativeLeft + 1.0f, relativeTop, trainAngle, distance);
        }

        private static (float RelativeLeft, float RelativeTop, float Angle, float Distance) MoveLeftUp(float relativeLeft, float relativeTop, float trainAngle, float distance)
        {
            // Find the angle within the tracks circle using the current position
            // This *should* be perpendicular to angle
            double currentAngle = MathHelpers.PointsToAngle(relativeLeft, relativeTop);

            // To travel 2PIr, we need to move 360
            // To travel x we need to move x/2PIr * 360
            // To travel x rad we need to move x/2PIr * 2PI
            // To travel x rad we need to move x/r
            double angleToMove = distance / RelativeCellRadius;

            // In order to figure out if we are moving clockwise or counter-clockwise, look at the angle of the train
            if (trainAngle < 45.0f || trainAngle > 225.0f)
            {
                // We are facing right/up, so we move counter clockwise, with a minimum angle of 0
                (currentAngle, distance) = MoveCounterClockwise(currentAngle, angleToMove, distance, 0);

                trainAngle = (float)MathHelpers.RadToDegree(currentAngle) - 90.0f;
            }
            else
            {
                // We are NOT facing right/up, so we move clockwise, with a maximum angle of 90 aka PI/2
                (currentAngle, distance) = MoveClockwise(currentAngle, angleToMove, distance, Math.PI / 2);

                trainAngle = (float)MathHelpers.RadToDegree(currentAngle) + 90.0f;
            }

            // Double check to keep our angle in range, this makes our angle checks easier!:
            trainAngle = MathHelpers.KeepWithin0and360(trainAngle);

            // Find our new position on the track
            (relativeLeft, relativeTop) = MathHelpers.AngleToPoints(currentAngle, RelativeCellRadius);

            return (relativeLeft, relativeTop, trainAngle, distance);
        }

        private static (double currentAngle, float distance) MoveCounterClockwise(double currentAngle, double angleToMove, float distance, double minimumNewAngle)
        {
            // If the angle to move is outside our limits, then only move as much as we can
            if (currentAngle - angleToMove < minimumNewAngle)
            {
                // Calculate how far over we are
                double angleOver = (angleToMove - currentAngle) + minimumNewAngle;

                // Set our angle to the limit, and a bit over
                currentAngle = minimumNewAngle - OutsideCellAnglePadding;

                // Calculate how far we could move
                distance = (float)(distance - angleOver * RelativeCellRadius);
            }
            else
            {
                currentAngle -= angleToMove;
                distance = 0;
            }

            return (currentAngle, distance);
        }

        private static (double currentAngle, float distance) MoveClockwise(double currentAngle, double angleToMove, float distance, double maximumNewAngle)
        {
            if (currentAngle + angleToMove > maximumNewAngle)
            {
                double angleOver = (angleToMove + currentAngle) - maximumNewAngle;
                currentAngle = maximumNewAngle + OutsideCellAnglePadding;
                distance = (float)(distance - angleOver * RelativeCellRadius);
            }
            else
            {
                currentAngle += angleToMove;
                distance = 0;
            }

            return (currentAngle, distance);
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

        private static (float RelativeLeft, float RelativeTop, float Angle, float Distance) MoveCross(float relativeLeft, float relativeTop, float angle, float distance)
        {
            if ((angle > 45.0f && angle < 135.0f) ||
                (angle > 225.0f && angle < 315.0f))
            {
                return MoveVertical(relativeLeft, relativeTop, angle, distance);
            }
            else
            {
                return MoveHorizontal(relativeLeft, relativeTop, angle, distance);
            }
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
