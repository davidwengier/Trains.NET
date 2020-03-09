using System.Diagnostics;

namespace Trains.NET.Engine
{
    public static class CornerAngles
    {
        public static readonly float LeftUp = 45.0f;
        public static readonly float RightUp = 135.0f;
        public static readonly float LeftDown = 315.0f;
        public static readonly float RightDown = 225.0f;
    }

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
            TrackDirection.LeftUp => MoveCorner(relativeLeft, relativeTop, angle, distance, CornerAngles.LeftUp),
            TrackDirection.RightUp => MoveCorner(relativeLeft, relativeTop, angle, distance, CornerAngles.RightUp),
            TrackDirection.LeftDown => MoveCorner(relativeLeft, relativeTop, angle, distance, CornerAngles.LeftDown),
            TrackDirection.RightDown => MoveCorner(relativeLeft, relativeTop, angle, distance, CornerAngles.RightDown),
            TrackDirection.Horizontal => MoveHorizontal(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Vertical => MoveVertical(relativeLeft, relativeTop, angle, distance),
            TrackDirection.Cross => MoveCross(relativeLeft, relativeTop, angle, distance),
            _ => throw new System.Exception(null)
        };

        private static (float RelativeLeft, float RelativeTop, float Angle, float Distance) MoveCorner(float relativeLeft, float relativeTop, float trainAngle, float distance, float cornerAngle)
        {
            // As the train is perpendicular to the corner angle, we can use the corner angle as a maximum to find clockwise vs counterclockwise
            //  minus 180 to get the minimum limit, but keep it in the 0-360 range (wrap it!)
            float minimumCornerAngle = MathHelpers.KeepWithin0and360(cornerAngle - 180.0f);

            // Use the angle to figure out which quadrent the corner is in
            (float scaleLeft, float scaleTop) = CalculateQuadrantScale(cornerAngle);

            // Find the angle of the train relative to the center of the circle, shifted so the middle is always in the top left
            double currentAngle = MathHelpers.PointsToAngle(relativeLeft - scaleLeft, relativeTop - scaleTop);

            // Find the radians that we need to move
            double angleToMove = distance / RelativeCellRadius;

            // Using the angle of the train, find clockwise vs counterclockwise
            float directionScale = MathHelpers.BetweenAngles(trainAngle, minimumCornerAngle, cornerAngle) ? -1.0f : 1.0f;

            // Using the corner angle, we can now find the limit angle, aka, maxiumum angle in our given direction before we are in the next cell
            //  Note: we use 44.999... here as jumping can happen when exactly 90 degrees
            double limitAngle = MathHelpers.KeepWithinNegPIandPIRads(MathHelpers.DegreeToRad(cornerAngle + directionScale * 44.999f));

            // Find our new angle, and if we have any distance left over
            (currentAngle, distance) = TravelAlongArc(currentAngle, angleToMove, distance, limitAngle, directionScale);

            // Our train will be prependicular (at 90 degrees) to the angle returned
            trainAngle = (float)MathHelpers.RadToDegree(currentAngle) + directionScale * 90.0f;

            // Keep things in the circle
            trainAngle = MathHelpers.KeepWithin0and360(trainAngle);

            // Find the new point on the track
            (relativeLeft, relativeTop) = MathHelpers.AngleToPoints(currentAngle, RelativeCellRadius);

            // Return everything, shifting back
            return (relativeLeft + scaleLeft, relativeTop + scaleTop, trainAngle, distance);
        }

        private static (float scaleLeft, float scaleTop) CalculateQuadrantScale(float cornerAngle)
        {
            float scaleLeft = 0.0f;
            if (cornerAngle > 90.0f && cornerAngle < 270.0f) scaleLeft = 1.0f;

            float scaleTop = 0.0f;
            if (cornerAngle > 180.0f) scaleTop = 1.0f;

            return (scaleLeft, scaleTop);
        }

        private static (double currentAngle, float distance) TravelAlongArc(double currentAngle, double angleToMove, float distance, double limit, float scale)
        {
            double angleOver = scale * (limit - currentAngle) - angleToMove;
            if (angleOver < 0)
            {
                currentAngle = limit + scale * OutsideCellAnglePadding;
                distance = (float)(distance + angleOver * RelativeCellRadius);
            }
            else
            {
                currentAngle += scale * angleToMove;
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
