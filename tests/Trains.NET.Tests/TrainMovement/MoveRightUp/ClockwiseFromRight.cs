using Trains.NET.Engine;
using static Trains.NET.Tests.TrainMovementTestsHelper;

namespace Trains.NET.Tests.TrainMovementTests.MoveRightUp;

public class ClockwiseFromRight
{
    [Theory]
    [InlineData(165.0f)] // Extreme
    [InlineData(179.0f)]
    [InlineData(180.0f)]
    [InlineData(181.0f)]
    [InlineData(195.0f)] // Extreme
    public void MoveRightUp_ClockwiseFromRight_WithinCell_VariedInitialAngles(float angle)
    {
        var position = new TrainPosition(1.0f, 0.5f, angle, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(1.0f - Cos45ByRadius, Sin45ByRadius, 225, 0.0f);

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
        var position = new TrainPosition(1.0f, relativeTop, 180, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(1.0f - Cos45ByRadius, Sin45ByRadius, 225, 0.0f);

        TrainMovement.MoveRightUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
        Assert.Equal(expectedPos.Angle, position.Angle, 3);
        Assert.Equal(expectedPos.Distance, position.Distance, 3);
    }

    [Theory]
    [InlineData(1.0f, 0.5f, 180.0f, HalfCornerTrackDistance, 1.0f - Cos45ByRadius, Sin45ByRadius, 225.0f)]
    [InlineData(1.0f, 0.5f, 180.0f, ThirdCornerTrackDistance, 1.0f - Cos60ByRadius, Sin60ByRadius, 210.0f)]
    [InlineData(1.0f, 0.5f, 180.0f, ThirdCornerTrackDistance * 2, 1.0f - Cos30ByRadius, Sin30ByRadius, 240.0f)]
    [InlineData(1.0f - Cos60ByRadius, Sin60ByRadius, 210.0f, ThirdCornerTrackDistance, 1.0f - Cos30ByRadius, Sin30ByRadius, 240.0f)]
    public void MoveRightUp_ClockwiseFromRight_WithinCell_VariedDistance(float initalLeft, float initalTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
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
    [InlineData(1.0f, 0.5f, 180.0f, HalfCornerTrackDistance * 3, HalfCornerTrackDistance)]
    [InlineData(1.0f - Cos45ByRadius, Sin45ByRadius, 225.0f, HalfCornerTrackDistance * 2, HalfCornerTrackDistance)]
    [InlineData(1.0f - Cos45ByRadius, Sin45ByRadius, 225.0f, 1.0f + HalfCornerTrackDistance, 1.0f)]
    public void MoveRightUp_ClockwiseFromRight_BeyondCell(float initalLeft, float initalTop, float initialAngle, float distance, float expectedDistance)
    {
        var position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
        var expectedPos = new TrainPosition(0.5f, -0.1f, 270, expectedDistance);

        TrainMovement.MoveRightUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3);
        Assert.Equal(expectedPos.Angle, position.Angle, 3);
        Assert.Equal(expectedPos.Distance, position.Distance, 3);
    }
}
