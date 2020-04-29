using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests.TrainMovementTests.MoveRightUp
{
    public class ClockwiseFromRight : TrainMovementTestsHelper
    {
        [Theory]
        [InlineData(165.0f)] // Extreme
        [InlineData(179.0f)]
        [InlineData(180.0f)]
        [InlineData(181.0f)]
        [InlineData(195.0f)] // Extreme
        public void MoveRightUp_ClockwiseFromRight_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(1.0f, 0.5f, angle, MovementDistanceOf45Degrees);
            TrainPosition expectedPos = new TrainPosition(1.0f - Cos45ByRadius, Sin45ByRadius, 225, 0.0f);

            TrainMovement.MoveRightUp(position);

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
        public void MoveRightUp_ClockwiseFromRight_WithinCell_SnappingToCenter(float relativeTop)
        {
            TrainPosition position = new TrainPosition(1.0f, relativeTop, 180, MovementDistanceOf45Degrees);
            TrainPosition expectedPos = new TrainPosition(1.0f - Cos45ByRadius, Sin45ByRadius, 225, 0.0f);

            TrainMovement.MoveRightUp(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(1.0f, 0.5f, 180.0f, MovementDistanceOf45Degrees, 1.0f - Cos45ByRadius, Sin45ByRadius, 225.0f)]
        [InlineData(1.0f, 0.5f, 180.0f, MovementDistanceOf30Degrees, 1.0f - Cos60ByRadius, Sin60ByRadius, 210.0f)]
        [InlineData(1.0f, 0.5f, 180.0f, MovementDistanceOf30Degrees * 2, 1.0f - Cos30ByRadius, Sin30ByRadius, 240.0f)]
        [InlineData(1.0f - Cos60ByRadius, Sin60ByRadius, 210.0f, MovementDistanceOf30Degrees, 1.0f - Cos30ByRadius, Sin30ByRadius, 240.0f)]
        public void MoveRightUp_ClockwiseFromRight_WithinCell_VariedDistance(float initalLeft, float initalTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
        {
            TrainPosition position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
            TrainPosition expectedPos = new TrainPosition(expectedLeft, expectedTop, expectedAngle, 0.0f);

            TrainMovement.MoveRightUp(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(1.0f, 0.5f, 180.0f, MovementDistanceOf45Degrees * 3, MovementDistanceOf45Degrees)]
        [InlineData(1.0f - Cos45ByRadius, Sin45ByRadius, 225.0f, MovementDistanceOf45Degrees * 2, MovementDistanceOf45Degrees)]
        [InlineData(1.0f - Cos45ByRadius, Sin45ByRadius, 225.0f, 1.0f + MovementDistanceOf45Degrees, 1.0f)]
        public void MoveRightUp_ClockwiseFromRight_BeyondCell(float initalLeft, float initalTop, float initialAngle, float distance, float expectedDistance)
        {
            TrainPosition position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
            TrainPosition expectedPos = new TrainPosition(0.5f, -0.1f, 270, expectedDistance);

            TrainMovement.MoveRightUp(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }
    }
}
