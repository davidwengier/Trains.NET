using System;

#nullable disable

namespace Trains.NET.Tests
{
    public class TrainMovementTestsHelper
    {
        internal const float Radius = 0.5f;

        internal const float MovementDistanceOf45Degrees = 0.39269908169872415480783042290994f;
        internal const float Cos45ByRadius = 0.35355339059327376220042218105242f;
        internal const float Sin45ByRadius = 0.35355339059327376220042218105242f;

        internal const float MovementDistanceOf30Degrees = 0.26179938779914943653855361527329f;
        internal const float Cos30ByRadius = 0.43301270189221932338186158537647f;
        internal const float Sin30ByRadius = 0.25f;

        internal const float Cos60ByRadius = Sin30ByRadius; // Gotta love the unit circle!
        internal const float Sin60ByRadius = Cos30ByRadius;

        internal const double Angle45InRads = Math.PI / 4.0;
        internal const double Angle135InRads = Math.PI / 2.0 + Math.PI / 4.0;
        internal const double Angle225InRads = Math.PI + Math.PI / 4.0;
        internal const double Angle315InRads = 2.0 * Math.PI - Math.PI / 4.0;
        internal const double Angle90InRads = Math.PI / 2.0;
        internal const double Angle180InRads = Math.PI;
        internal const double Angle270InRads = 3.0 * Math.PI / 2.0;
        internal const double Angle360InRads = 2.0 * Math.PI;
    }
}
