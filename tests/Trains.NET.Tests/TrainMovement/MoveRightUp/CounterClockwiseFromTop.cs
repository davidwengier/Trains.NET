using Trains.NET.Engine;
using static Trains.NET.Tests.TrainMovementTestsHelper;

namespace Trains.NET.Tests.TrainMovementTests.MoveRightUp;

public class CounterClockwiseFromTop
{
    [Theory]
    [InlineData(85.0f)] // Extreme
    [InlineData(89.0f)]
    [InlineData(90.0f)]
    [InlineData(91.0f)]
    [InlineData(105.0f)] // Extreme
    public void MoveRightUp_CounterClockwiseFromTop_WithinCell_VariedInitialAngles(float angle)
    {
        var position = new TrainPosition(0.5f, 0.0f, angle, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(1.0f - Cos45ByRadius, Sin45ByRadius, 45.0f, 0.0f);

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
    public void MoveRightUp_CounterClockwiseFromTop_WithinCell_SnappingToCenter(float relativeLeft)
    {
        var position = new TrainPosition(relativeLeft, 0.0f, 90.0f, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(1.0f - Cos45ByRadius, Sin45ByRadius, 45.0f, 0.0f);

        TrainMovement.MoveRightUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
        Assert.Equal(expectedPos.Angle, position.Angle, 3);
        Assert.Equal(expectedPos.Distance, position.Distance, 3);
    }

    [Theory]
    [InlineData(0.5f, 0.0f, 90.0f, HalfCornerTrackDistance, 1.0f - Cos45ByRadius, Sin45ByRadius, 45.0f)]
    [InlineData(0.5f, 0.0f, 90.0f, ThirdCornerTrackDistance, 1.0f - Cos30ByRadius, Sin30ByRadius, 60.0f)]
    [InlineData(0.5f, 0.0f, 90.0f, ThirdCornerTrackDistance * 2, 1.0f - Cos60ByRadius, Sin60ByRadius, 30.0f)]
    [InlineData(1.0f - Cos30ByRadius, Sin30ByRadius, 60.0f, ThirdCornerTrackDistance, 1.0f - Cos60ByRadius, Sin60ByRadius, 30.0f)]
    public void MoveRightUp_CounterClockwiseFromTop_WithinCell_VariedDistance(float initalLeft, float initalTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
    {
        var position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
        var expectedPos = new TrainPosition(expectedLeft, expectedTop, expectedAngle, 0.0f);

        TrainMovement.MoveRightUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
        Assert.Equal(expectedPos.Angle, position.Angle, 3);
        Assert.Equal(expectedPos.Distance, position.Distance, 3);
    }

    [Theory]
    [InlineData(0.5f, 0.0f, 90.0f, HalfCornerTrackDistance * 3, HalfCornerTrackDistance)]
    [InlineData(1.0f - Cos45ByRadius, Sin45ByRadius, 45.0f, HalfCornerTrackDistance * 2, HalfCornerTrackDistance)]
    [InlineData(1.0f - Cos45ByRadius, Sin45ByRadius, 45.0f, 1.0f + HalfCornerTrackDistance, 1.0f)]
    public void MoveRightUp_CounterClockwiseFromTop_BeyondCell(float initalLeft, float initalTop, float initialAngle, float distance, float expectedDistance)
    {
        var position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
        var expectedPos = new TrainPosition(1.1f, 0.5f, 0.0f, expectedDistance);

        TrainMovement.MoveRightUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
        Assert.Equal(expectedPos.Angle, position.Angle, 3);
        Assert.Equal(expectedPos.Distance, position.Distance, 3);
    }
}
