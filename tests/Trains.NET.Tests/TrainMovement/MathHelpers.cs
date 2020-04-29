using System;
using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests.TrainMovementTests
{
    public class MathHelpers
    {
        [Theory]
        [InlineData(0.0, 0.0)]
        [InlineData(Math.PI / 2, 90.0)]
        [InlineData(Math.PI, 180.0)]
        [InlineData(Math.PI * 2, 360.0)]
        public void RadToDegree(double angle, double expected) => Assert.Equal(expected, TrainMovement.RadToDegree(angle), 1);

        [Theory]
        [InlineData(0.0, 0.0)]
        [InlineData(90.0, Math.PI / 2)]
        [InlineData(180.0, Math.PI)]
        [InlineData(360.0, Math.PI * 2)]
        public void DegreeToRad(double angle, double expected) => Assert.Equal(expected, TrainMovement.DegreeToRad(angle), 1);

        [Theory]
        [InlineData(0.0, 0.0)]
        [InlineData(90.0, 90.0)]
        [InlineData(180.0, 180.0)]
        [InlineData(360.0, 360.0)]
        [InlineData(450.0, 90.0)]
        [InlineData(810.0, 90.0)]
        [InlineData(-90.0, 270.0)]
        [InlineData(-360.0, 0.0)]
        [InlineData(-810.0, 270.0)]
        [InlineData(-630.0, 90.0)]
        public void KeepWithin0and360(float angle, float expected) => Assert.Equal(expected, TrainMovement.KeepWithin0and360(angle), 1);

        [Theory]
        [InlineData(45.0f, 0.0f, 90.0f)]
        [InlineData(90.0f, 0.0f, 360.0f)]
        [InlineData(270.0f, 180.0f, 360.0f)]
        [InlineData(360.0f, 270.0f, 90.0f)]
        [InlineData(0.0f, 270.0f, 90.0f)]
        public void BetweenAngles_WithinAngle(float angle, float limit1, float limit2) => Assert.True(TrainMovement.BetweenAngles(angle, limit1, limit2));

        [Theory]
        [InlineData(180.0f, 0.0f, 90.0f)]
        [InlineData(90.0f, 180.0f, 360.0f)]
        [InlineData(180.0f, 270.0f, 90.0f)]
        public void BetweenAngles_OutsideAngle(float angle, float limit1, float limit2) => Assert.False(TrainMovement.BetweenAngles(angle, limit1, limit2));

        [Theory]
        [InlineData(1.0f, 0.0f, 0.0)]
        [InlineData(0.0f, 1.0f, Math.PI / 2)]
        [InlineData(-1.0f, 0.0f, Math.PI)]
        [InlineData(0.0f, -1.0f, -Math.PI / 2)]
        public void PointsToAngle(float x, float y, double expected) => Assert.Equal(expected, TrainMovement.PointsToAngle(x, y), 1);


        [Theory]
        [InlineData(0.0, 1.0f, 1.0f, 0.0f)]
        [InlineData(0.0, 5.0f, 5.0f, 0.0f)]
        [InlineData(Math.PI, 1.0f, -1.0f, 0.0f)]
        [InlineData(Math.PI, 5.0f, -5.0f, 0.0f)]
        [InlineData(Math.PI / 2, 1.0f, 0.0f, 1.0f)]
        [InlineData(Math.PI / 2, 5.0f, 0.0f, 5.0f)]
        [InlineData(Math.PI / 4, 1.0f, 0.707f, 0.707f)]
        public void AngleToPoints(double angle, float radius, float expectedX, float expectedY)
        {
            (float x, float y) = TrainMovement.AngleToPoints(angle, radius);

            Assert.Equal(expectedX, x, 3);
            Assert.Equal(expectedY, y, 3);
        }
    }
}
