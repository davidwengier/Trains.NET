using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests.TrainMovementTests.MoveVertical
{
    public class MovingUp
    {
        [Theory]
        [InlineData(255.0f)] // Extreme
        [InlineData(269.0f)]
        [InlineData(270.0f)]
        [InlineData(271.0f)]
        [InlineData(285.0f)] // Extreme
        public void MoveVertical_MovingUp_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(0.5f, 1.0f, angle, 0.5f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 270, 0.0f);

            TrainMovement.MoveVertical(position);

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
        public void MoveVertical_MovingUp_WithinCell_SnappingToMiddle(float relativeLeft)
        {
            TrainPosition position = new TrainPosition(relativeLeft, 1.0f, 270, 0.5f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 270, 0.0f);

            TrainMovement.MoveVertical(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(1.0f, 0.1f, 0.9f)]
        [InlineData(0.9f, 0.2f, 0.7f)]
        [InlineData(0.9f, 0.5f, 0.4f)]
        [InlineData(0.5f, 0.4f, 0.1f)]
        public void MoveVertical_MovingUp_WithinCell_VariedDistance(float initalTop, float distance, float expectedTop)
        {
            TrainPosition position = new TrainPosition(0.5f, initalTop, 270, distance);
            TrainPosition expectedPos = new TrainPosition(0.5f, expectedTop, 270, 0.0f);

            TrainMovement.MoveVertical(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.5f, 0.5f, 0.0f)]
        [InlineData(0.5f, 0.6f, 0.1f)]
        [InlineData(0.5f, 1.0f, 0.5f)]
        [InlineData(0.5f, 2.0f, 1.5f)]
        [InlineData(1.0f, 1.0f, 0.0f)]
        public void MoveVertical_MovingUp_BeyondCell(float initalTop, float distance, float expectedDistance)
        {
            TrainPosition position = new TrainPosition(0.5f, initalTop, 270, distance);
            TrainPosition expectedPos = new TrainPosition(0.5f, -0.1f, 270, expectedDistance);

            TrainMovement.MoveVertical(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }
    }
}
