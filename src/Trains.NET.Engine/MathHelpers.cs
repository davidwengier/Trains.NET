using System;

namespace Trains.NET.Engine
{
    public static class MathHelpers
    {
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

        public static bool BetweenAngles(float angle, float limit1, float limit2) =>
            (limit1 < limit2 && (angle > limit1 && angle < limit2)) ||
                (limit2 < limit1 && (angle > limit1 || angle < limit2));
    }
}
