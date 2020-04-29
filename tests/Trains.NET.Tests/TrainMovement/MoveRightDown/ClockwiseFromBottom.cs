using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests.TrainMovementTests.MoveRightDown
{
    public class ClockwiseFromBottom : TrainMovementTestsHelper
    {
        [Theory]
        [InlineData(255.0f)] // Extreme
        [InlineData(269.0f)]
        [InlineData(270.0f)]
        [InlineData(271.0f)]
        [InlineData(285.0f)] // Extreme
        public void MoveRightDown_ClockwiseFromBottom_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(0.5f, 1.0f, angle, MovementDistanceOf45Degrees);
            TrainPosition expectedPos = new TrainPosition(1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315, 0.0f);

            TrainMovement.MoveRightDown(position);

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
        public void MoveRightDown_ClockwiseFromBottom_WithinCell_SnappingToCenter(float relativeLeft)
        {
            TrainPosition position = new TrainPosition(relativeLeft, 1.0f, 270, MovementDistanceOf45Degrees);
            TrainPosition expectedPos = new TrainPosition(1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315, 0.0f);

            TrainMovement.MoveRightDown(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.5f, 1.0f, 270.0f, MovementDistanceOf45Degrees, 1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315.0f)]
        [InlineData(0.5f, 1.0f, 270.0f, MovementDistanceOf30Degrees, 1.0f - Cos30ByRadius, 1.0f - Sin30ByRadius, 300.0f)]
        [InlineData(0.5f, 1.0f, 270.0f, MovementDistanceOf30Degrees * 2, 1.0f - Cos60ByRadius, 1.0f - Sin60ByRadius, 330.0f)]
        [InlineData(1.0f - Cos30ByRadius, 1.0f - Sin30ByRadius, 315.0f, MovementDistanceOf30Degrees, 1.0f - Cos60ByRadius, 1.0f - Sin60ByRadius, 330.0f)]
        public void MoveRightDown_ClockwiseFromBottom_WithinCell_VariedDistance(float initalLeft, float initalTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
        {
            TrainPosition position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
            TrainPosition expectedPos = new TrainPosition(expectedLeft, expectedTop, expectedAngle, 0.0f);

            TrainMovement.MoveRightDown(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.5f, 1.0f, 270.0f, MovementDistanceOf45Degrees * 3, MovementDistanceOf45Degrees)]
        [InlineData(1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315.0f, MovementDistanceOf45Degrees * 2, MovementDistanceOf45Degrees)]
        [InlineData(1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315.0f, 1.0f + MovementDistanceOf45Degrees, 1.0f)]
        public void MoveRightDown_ClockwiseFromBottom_BeyondCell(float initalLeft, float initalTop, float initialAngle, float distance, float expectedDistance)
        {
            TrainPosition position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
            TrainPosition expectedPos = new TrainPosition(1.1f, 0.5f, 0, expectedDistance);

            TrainMovement.MoveRightDown(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }
    }
}
