using System;

namespace Trains.NET.Engine
{
    public static class TrainMovement
    {
        private const double Radius = 0.5;

        public static double RadToDegree(double angle) => angle * 180.0 / Math.PI;

        public static double DegreeToRad(double angle) => angle / (180.0 / Math.PI);

        public static float KeepWithin0and360(float angle)
        {
            while (angle < 0) angle += 360;
            while (angle > 360) angle -= 360;
            return angle;
        }
        public static double PointsToAngle(float x, float y)
        {
            // Atan2 allows us to find the angle between 0,0 and a point
            // Atan2 is special, as it takes into account where on the circle the point is
            // BUT BE CAREFUL!!! Atan takes Y as the FRIST paramater, tricky math!

            return Math.Atan2(y, x);
        }
        public static (float x, float y) AngleToPoints(double angle, float radius)
        {
            // Sin for Y, Cos for X, multiply them by the radius and we are done!
            float y = (float)(radius * Math.Sin(angle));
            float x = (float)(radius * Math.Cos(angle));

            return (x, y);
        }

        public static bool BetweenAngles(float angle, float limit1, float limit2)
        {
            return (limit1 < limit2 && (angle > limit1 && angle < limit2)) ||
                (limit2 < limit1 && (angle > limit1 || angle < limit2));
        }

        /// <summary>
        /// Moves a train around a 90 degree arc, in either direction
        /// </summary>
        /// <param name="position">The trains position</param>
        /// <param name="quadrantPositionX">The distance between the center of the circle, and the left hand side of the cell</param>
        /// <param name="quadrantPositionY">The distance between the center of the circle, and the top of the cell</param>
        /// <param name="midpointAngle">The angle at the middle of the arc you want to move</param>
        public static void MoveAroundCorner(TrainPosition position, int quadrantPositionX, int quadrantPositionY, int minTrainAngleCCW, int maxTrainAngleCCW, int minimumAngle, int maximumAngle)
        {
            // Find the angle within the tracks circle using the current position
            // This *should* be perpendicular to angle
            double currentAngle = TrainMovement.PointsToAngle(position.RelativeLeft - Math.Abs(quadrantPositionX), position.RelativeTop - Math.Abs(quadrantPositionY));

            float distance;
            // In order to figure out if we are moving clockwise or counter-clockwise, look at the angle of the train
            if (BetweenAngles(position.Angle, minTrainAngleCCW, maxTrainAngleCCW))
            {
                // We are facing left/up, so we move counter clockwise, with a minimum angle of 90
                (currentAngle, distance) = MoveCounterClockwise(currentAngle, position.Distance, DegreeToRad(minimumAngle));

                position.Angle = (float)TrainMovement.RadToDegree(currentAngle) - 90.0f;
            }
            else
            {
                // We are NOT facing left/up, so we move clockwise, with a maximum angle of 180, Math.PI
                (currentAngle, distance) = MoveClockwise(currentAngle, position.Distance, DegreeToRad(maximumAngle));

                position.Angle = (float)TrainMovement.RadToDegree(currentAngle) + 90.0f;
            }

            position.Distance = distance;

            // Double check to keep our angle in range, this makes our angle checks easier!:
            position.Angle = TrainMovement.KeepWithin0and360(position.Angle);

            // Find our new position on the track
            (position.RelativeLeft, position.RelativeTop) = TrainMovement.AngleToPoints(currentAngle, 0.5f);

            position.RelativeLeft += Math.Abs(quadrantPositionX);
            position.RelativeTop += Math.Abs(quadrantPositionY);

            DoAnEdgeSnap(position);
        }
        public static void DoAnEdgeSnap(TrainPosition position)
        {
            // Do some fun snapping
            if (position.RelativeLeft < 0)
            {
                position.RelativeLeft = -0.1f;
                position.RelativeTop = 0.5f;
                position.Angle = 180.0f;
            }
            else if (position.RelativeLeft > 1f)
            {
                position.RelativeLeft = 1.1f;
                position.RelativeTop = 0.5f;
                position.Angle = 0.0f;
            }

            if (position.RelativeTop < 0)
            {
                position.RelativeTop = -0.1f;
                position.RelativeLeft = 0.5f;
                position.Angle = 270.0f;
            }
            else if (position.RelativeTop > 1f)
            {
                position.RelativeTop = 1.1f;
                position.RelativeLeft = 0.5f;
                position.Angle = 90.0f;
            }
        }

        public static void MoveLeftDown(TrainPosition position) => TrainMovement.MoveAroundCorner(position, 0, -1, 135, 315, 270, 360);

        public static void MoveRightDown(TrainPosition position) => TrainMovement.MoveAroundCorner(position, -1, -1, 45, 225, 180, 270);

        public static void MoveRightUp(TrainPosition position) => TrainMovement.MoveAroundCorner(position, -1, 0, -45, 135, 90, 180);

        public static void MoveLeftUp(TrainPosition position) => TrainMovement.MoveAroundCorner(position, 0, 0, 225, 45, 0, 90);

        public static (double currentAngle, float distance) MoveCounterClockwise(double currentAngle, float distance, double minimumNewAngle)
        {
            if (currentAngle <= 0) currentAngle += Math.PI * 2.0;

            double angleToMove = distance / Radius;

            // If the angle to move is outside our limits, then only move as much as we can

            double angleOver = currentAngle - angleToMove - minimumNewAngle;

            if (angleOver < -0.00001)
            {
                // Set our angle to the limit, and a bit over
                currentAngle = minimumNewAngle - 0.1f;

                // Calculate how far we could move
                distance = -(float)(angleOver * 0.5f);
            }
            else
            {
                currentAngle -= angleToMove;
                currentAngle = Math.Max(currentAngle, minimumNewAngle);
                distance = 0;
            }

            return (currentAngle, distance);
        }

        public static (double currentAngle, float distance) MoveClockwise(double currentAngle, float distance, double maximumNewAngle)
        {
            if (currentAngle < 0) currentAngle += Math.PI * 2.0;

            double angleToMove = distance / Radius;

            double angleOver = currentAngle + angleToMove - maximumNewAngle;

            if (angleOver > 0.00001)
            {
                currentAngle = maximumNewAngle + 0.1f;

                if (currentAngle > Math.PI * 2.0) currentAngle -= Math.PI * 2.0;

                distance = (float)(angleOver * Radius);
            }
            else
            {
                currentAngle += angleToMove;
                currentAngle = Math.Min(currentAngle, maximumNewAngle);
                distance = 0;
            }

            return (currentAngle, distance);
        }

        public static void MoveVertical(TrainPosition position)
        {
            // Snap left
            position.RelativeLeft = 0.5f;

            // Snap angle
            if (position.Angle < 180f)
            {
                position.Angle = 90f;
                float toGo = 1.0f - position.RelativeTop;

                if (position.Distance < toGo)
                {
                    position.RelativeTop += position.Distance;
                    position.Distance = 0;
                }
                else
                {
                    position.Distance -= toGo;
                    position.RelativeTop = 1.1f;
                }
            }
            else
            {
                position.Angle = 270f;
                float toGo = position.RelativeTop;

                if (position.Distance < toGo)
                {
                    position.RelativeTop -= position.Distance;
                    position.Distance = 0;
                }
                else
                {
                    position.Distance -= toGo;
                    position.RelativeTop = -0.1f;
                }
            }
        }

        public static void MoveHorizontal(TrainPosition position)
        {
            // Snap top
            position.RelativeTop = 0.5f;

            // Snap angle
            if (position.Angle < 90f || position.Angle > 270f)
            {
                position.Angle = 0f;
                float toGo = 1.0f - position.RelativeLeft;

                if (position.Distance < toGo)
                {
                    position.RelativeLeft += position.Distance;
                    position.Distance = 0;
                }
                else
                {
                    position.Distance -= toGo;
                    position.RelativeLeft = 1.1f;
                }
            }
            else
            {
                position.Angle = 180f;
                float toGo = position.RelativeLeft;

                if (position.Distance < toGo)
                {
                    position.RelativeLeft -= position.Distance;
                    position.Distance = 0;
                }
                else
                {
                    position.Distance -= toGo;
                    position.RelativeLeft = -0.1f;
                }
            }
        }

    }
}
