using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests.TrainMovementTests.MoveLeftUp
{
    public class CounterClockwiseFromLeft : TrainMovementTestsHelper
    {
        [Theory]
        [InlineData(345.0f)] // Extreme
        [InlineData(359.0f)]
        [InlineData(360.0f)]
        [InlineData(0.0f)]
        [InlineData(1.0f)]
        [InlineData(15.0f)] // Extreme
        public void MoveLeftUp_CounterClockwiseFromLeft_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(0.0f, 0.5f, angle, MovementDistanceOf45Degrees);
            TrainPosition expectedPos = new TrainPosition(Cos45ByRadius, Sin45ByRadius, 315.0f, 0.0f);

            TrainMovement.MoveLeftUp(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.1f)] // Extreme
        [InlineData(0.4f)]
        [InlineData(0.5f)]
        [InlineData(0.6f)]
        [InlineData(0.9f)] // Extreme
        public void MoveLeftUp_CounterClockwiseFromLeft_WithinCell_SnappingToCenter(float relativeTop)
        {
            TrainPosition position = new TrainPosition(0.0f, relativeTop, 0.0f, MovementDistanceOf45Degrees);
            TrainPosition expectedPos = new TrainPosition(Cos45ByRadius, Sin45ByRadius, 315.0f, 0.0f);

            TrainMovement.MoveLeftUp(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.0f, 0.5f, 0.0f, MovementDistanceOf45Degrees, Cos45ByRadius, Sin45ByRadius, 315.0f)]
        [InlineData(0.0f, 0.5f, 0.0f, MovementDistanceOf30Degrees, Cos60ByRadius, Sin60ByRadius, 330.0f)]
        [InlineData(0.0f, 0.5f, 0.0f, MovementDistanceOf30Degrees * 2, Cos30ByRadius, Sin30ByRadius, 300.0f)]
        [InlineData(Cos60ByRadius, Sin60ByRadius, 330.0f, MovementDistanceOf30Degrees, Cos30ByRadius, Sin30ByRadius, 300.0f)]
        public void MoveLeftUp_CounterClockwiseFromLeft_WithinCell_VariedDistance(float initalLeft, float initalTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
        {
            TrainPosition position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
            TrainPosition expectedPos = new TrainPosition(expectedLeft, expectedTop, expectedAngle, 0.0f);

            TrainMovement.MoveLeftUp(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.0f, 0.5f, 0.0f, MovementDistanceOf45Degrees * 3, MovementDistanceOf45Degrees)]
        [InlineData(Cos45ByRadius, Sin45ByRadius, 315.0f, MovementDistanceOf45Degrees * 2, MovementDistanceOf45Degrees)]
        [InlineData(Cos45ByRadius, Sin45ByRadius, 315.0f, 1.0f + MovementDistanceOf45Degrees, 1.0f)]
        public void MoveLeftUp_CounterClockwiseFromLeft_BeyondCell(float initalLeft, float initalTop, float initialAngle, float distance, float expectedDistance)
        {
            TrainPosition position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
            TrainPosition expectedPos = new TrainPosition(0.5f, -0.1f, 270.0f, expectedDistance);

            TrainMovement.MoveLeftUp(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }
    }
}
