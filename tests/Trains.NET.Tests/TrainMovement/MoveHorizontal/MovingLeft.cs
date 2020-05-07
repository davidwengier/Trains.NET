using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests.TrainMovementTests.MoveHorizontal
{
    public class MovingLeft : TrainMovementTestsHelper
    {
        [Theory]
        [InlineData(165.0f)] // Extreme
        [InlineData(179.0f)]
        [InlineData(180.0f)]
        [InlineData(181.0f)]
        [InlineData(195.0f)] // Extreme
        public void MoveHorizontal_MovingLeft_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(1.0f, 0.5f, angle, 0.5f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 180, 0.0f);

            TrainMovement.MoveHorizontal(position);

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
        public void MoveHorizontal_MovingLeft_WithinCell_SnappingToCenter(float relativeTop)
        {
            TrainPosition position = new TrainPosition(1.0f, relativeTop, 180, 0.5f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 180, 0.0f);

            TrainMovement.MoveHorizontal(position);

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
        public void MoveHorizontal_MovingLeft_WithinCell_VariedDistance(float initalLeft, float distance, float expectedLeft)
        {
            TrainPosition position = new TrainPosition(initalLeft, 0.5f, 180, distance);
            TrainPosition expectedPos = new TrainPosition(expectedLeft, 0.5f, 180, 0.0f);

            TrainMovement.MoveHorizontal(position);

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
        public void MoveHorizontal_MovingLeft_BeyondCell(float initalLeft, float distance, float expectedDistance)
        {
            TrainPosition position = new TrainPosition(initalLeft, 0.5f, 180, distance);
            TrainPosition expectedPos = new TrainPosition(-0.1f, 0.5f, 180, expectedDistance);

            TrainMovement.MoveHorizontal(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }
    }
}
