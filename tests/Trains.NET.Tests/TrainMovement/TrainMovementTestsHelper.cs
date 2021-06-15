using System;

namespace Trains.NET.Tests
{
    public static class TrainMovementTestsHelper
    {
        internal const float CornerRadius = 0.5f;

        // 0.5 * cos(45) =
        internal const float Cos45ByRadius = 0.35355339059327376220042218105242f;

        // 0.5 * sin(45) =
        internal const float Sin45ByRadius = 0.35355339059327376220042218105242f;

        // 0.5 * cos(30) =
        internal const float Cos30ByRadius = 0.43301270189221932338186158537647f;

        // 0.5 * sin(30) =
        internal const float Sin30ByRadius = 0.25f;

        // 0.5 * cos(60) =
        internal const float Cos60ByRadius = Sin30ByRadius; // Gotta love the unit circle!

        // 0.5 * sin(60) =
        internal const float Sin60ByRadius = Cos30ByRadius;

        internal const double Angle45InRads = Math.PI / 4.0;
        internal const double Angle135InRads = Math.PI / 2.0 + Math.PI / 4.0;
        internal const double Angle225InRads = Math.PI + Math.PI / 4.0;
        internal const double Angle315InRads = 2.0 * Math.PI - Math.PI / 4.0;
        internal const double Angle90InRads = Math.PI / 2.0;
        internal const double Angle180InRads = Math.PI;
        internal const double Angle270InRads = 3.0 * Math.PI / 2.0;
        internal const double Angle360InRads = 2.0 * Math.PI;

        internal const double StraightTrackDistance = 1.0;
        internal const double HalfStraightTrackDistance = StraightTrackDistance / 2.0;

        internal const double CornerTrackDistance = (Math.PI / 2.0) * CornerRadius;
        // Half corner is equivalent to traveling 45 degrees 
        internal const double HalfCornerTrackDistance = CornerTrackDistance / 2.0;
        // Third corner is equivalent to traveling 30 degrees
        internal const double ThirdCornerTrackDistance = CornerTrackDistance / 3.0;
    }
}
