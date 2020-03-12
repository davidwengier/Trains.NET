using System;
using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests
{
    public class TrainMovementTests
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

        #region MoveVertical_MovingDown
        [Theory]
        [InlineData(75.0f)] // Extreme
        [InlineData(89.0f)]
        [InlineData(90.0f)]
        [InlineData(91.0f)]
        [InlineData(105.0f)] // Extreme
        public void MoveVertical_MovingDown_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(0.5f, 0.1f, angle, 0.4f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 90, 0.0f);

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
        public void MoveVertical_MovingDown_WithinCell_SnappingToMiddle(float relativeLeft)
        {
            TrainPosition position = new TrainPosition(relativeLeft, 0.1f, 90, 0.4f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 90, 0.0f);

            TrainMovement.MoveVertical(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.1f, 0.1f, 0.2f)]
        [InlineData(0.1f, 0.5f, 0.6f)]
        [InlineData(0.5f, 0.4f, 0.9f)]
        public void MoveVertical_MovingDown_WithinCell_VariedDistance(float initalTop, float distance, float expectedTop)
        {
            TrainPosition position = new TrainPosition(0.5f, initalTop, 90, distance);
            TrainPosition expectedPos = new TrainPosition(0.5f, expectedTop, 90, 0.0f);

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
        public void MoveVertical_MovingDown_BeyondCell(float initalTop, float distance, float expectedDistance)
        {
            TrainPosition position = new TrainPosition(0.5f, initalTop, 90, distance);
            TrainPosition expectedPos = new TrainPosition(0.5f, 1.1f, 90, expectedDistance);

            TrainMovement.MoveVertical(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }
        #endregion MoveVertical_MovingDown

        #region MoveVertical_MovingUp
        [Theory]
        [InlineData(255.0f)] // Extreme
        [InlineData(269.0f)]
        [InlineData(270.0f)]
        [InlineData(271.0f)]
        [InlineData(285.0f)] // Extreme
        public void MoveVertical_MovingUp_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(0.5f, 0.9f, angle, 0.4f);
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
            TrainPosition position = new TrainPosition(relativeLeft, 0.9f, 270, 0.4f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 270, 0.0f);

            TrainMovement.MoveVertical(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.9f, 0.1f, 0.8f)]
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
        #endregion MoveVertical_MovingUp

        #region MoveHorizontal_MovingRight
        [Theory]
        [InlineData(345.0f)] // Extreme
        [InlineData(-15.0f)] // Extreme
        [InlineData(359.0f)]
        [InlineData(-1.0f)]
        [InlineData(360.0f)]
        [InlineData(0.0f)]
        [InlineData(1.0f)]
        [InlineData(-359.0f)]
        [InlineData(15.0f)] // Extreme
        [InlineData(-345.0f)] // Extreme
        public void MoveHorizontal_MovingRight_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(0.1f, 0.5f, angle, 0.4f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 0, 0.0f);

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
        public void MoveHorizontal_MovingRight_WithinCell_SnappingToCenter(float relativeTop)
        {
            TrainPosition position = new TrainPosition(0.1f, relativeTop, 0, 0.4f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 0, 0.0f);

            TrainMovement.MoveHorizontal(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.1f, 0.1f, 0.2f)]
        [InlineData(0.1f, 0.5f, 0.6f)]
        [InlineData(0.5f, 0.4f, 0.9f)]
        public void MoveHorizontal_MovingRight_WithinCell_VariedDistance(float initalLeft, float distance, float expectedLeft)
        {
            TrainPosition position = new TrainPosition(initalLeft, 0.5f, 0, distance);
            TrainPosition expectedPos = new TrainPosition(expectedLeft, 0.5f, 0, 0.0f);

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
        public void MoveHorizontal_MovingRight_BeyondCell(float initalLeft, float distance, float expectedDistance)
        {
            TrainPosition position = new TrainPosition(initalLeft, 0.5f, 0, distance);
            TrainPosition expectedPos = new TrainPosition(1.1f, 0.5f, 0, expectedDistance);

            TrainMovement.MoveHorizontal(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }
        #endregion MoveHorizontal_MovingRight

        #region MoveHorizontal_MovingLeft
        [Theory]
        [InlineData(165.0f)] // Extreme
        [InlineData(179.0f)]
        [InlineData(180.0f)]
        [InlineData(181.0f)]
        [InlineData(195.0f)] // Extreme
        public void MoveHorizontal_MovingLeft_WithinCell_VariedInitialAngles(float angle)
        {
            TrainPosition position = new TrainPosition(0.9f, 0.5f, angle, 0.4f);
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
            TrainPosition position = new TrainPosition(0.9f, relativeTop, 180, 0.4f);
            TrainPosition expectedPos = new TrainPosition(0.5f, 0.5f, 180, 0.0f);

            TrainMovement.MoveHorizontal(position);

            Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
            Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
            Assert.Equal(expectedPos.Angle, position.Angle, 3);
            Assert.Equal(expectedPos.Distance, position.Distance, 3);
        }

        [Theory]
        [InlineData(0.9f, 0.1f, 0.8f)]
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
        #endregion MoveHorizontal_MovingLeft
    }
}
