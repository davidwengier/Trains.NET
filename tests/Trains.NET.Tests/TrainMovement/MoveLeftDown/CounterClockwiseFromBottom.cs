using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests.TrainMovementTests.MoveLeftDown
{
    public class CounterClockwiseFromBottom : TrainMovementTestsHelper
    {
        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingUp - 15.0f)] // Extreme
        [InlineData(TrainAngleHelper.TrainFacingUp - 1.0f)]
        [InlineData(TrainAngleHelper.TrainFacingUp)]
        [InlineData(TrainAngleHelper.TrainFacingUp + 1.0f)]
        [InlineData(TrainAngleHelper.TrainFacingUp + 15.0f)] // Extreme
        public void MoveLeftDown_CounterClockwiseFromBottom_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(0.5f, 1.0f, angle, (float)HalfCornerTrackDistance);
            TrainPosition expectedPos = new TrainPosition(Cos45ByRadius, 1.0f - Sin45ByRadius, 225, 0.0f);

            TrainMovement.MoveLeftDown(position);

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
        public void MoveLeftDown_CounterClockwiseFromBottom_WithinCell_SnappingToCenter(float relativeLeft)
        {
            TrainPosition position = new TrainPosition(relativeLeft, 1.0f, 270, (float)HalfCornerTrackDistance);
            TrainPosition expectedPos = new TrainPosition(Cos45ByRadius, 1.0f - Sin45ByRadius, 225, 0.0f);

            TrainMovement.MoveLeftDown(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.5f, 1.0f, 270.0f, HalfCornerTrackDistance, Cos45ByRadius, 1.0f - Sin45ByRadius, 225.0f)]
        [InlineData(0.5f, 1.0f, 270.0f, ThirdCornerTrackDistance, Cos30ByRadius, 1.0f - Sin30ByRadius, 240.0f)]
        [InlineData(0.5f, 1.0f, 270.0f, ThirdCornerTrackDistance * 2, Cos60ByRadius, 1.0f - Sin60ByRadius, 210.0f)]
        [InlineData(Cos30ByRadius, 1.0f - Sin30ByRadius, 240.0f, ThirdCornerTrackDistance, Cos60ByRadius, 1.0f - Sin60ByRadius, 210.0f)]
        public void MoveLeftDown_CounterClockwiseFromBottom_WithinCell_VariedDistance(float initalLeft, float initalTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
        {
            TrainPosition position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
            TrainPosition expectedPos = new TrainPosition(expectedLeft, expectedTop, expectedAngle, 0.0f);

            TrainMovement.MoveLeftDown(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.5f, 1.0f, 270.0f, HalfCornerTrackDistance * 3, HalfCornerTrackDistance)]
        [InlineData(Cos45ByRadius, 1.0f - Sin45ByRadius, 225, HalfCornerTrackDistance * 2, HalfCornerTrackDistance)]
        [InlineData(Cos45ByRadius, 1.0f - Sin45ByRadius, 225, 1.0f + HalfCornerTrackDistance, 1.0f)]
        public void MoveLeftDown_CounterClockwiseFromBottom_BeyondCell(float initalLeft, float initalTop, float initialAngle, float distance, float expectedDistance)
        {
            TrainPosition position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
            TrainPosition expectedPos = new TrainPosition(-0.1f, 0.5f, 180, expectedDistance);

            TrainMovement.MoveLeftDown(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }
    }
}
