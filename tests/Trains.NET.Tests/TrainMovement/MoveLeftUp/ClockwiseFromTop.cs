using Trains.NET.Engine;
using static Trains.NET.Tests.TrainMovementTestsHelper;

namespace Trains.NET.Tests.TrainMovementTests.MoveLeftUp;

public class ClockwiseFromTop
{
    [Theory]
    [InlineData(85.0f)] // Extreme
    [InlineData(89.0f)]
    [InlineData(90.0f)]
    [InlineData(91.0f)]
    [InlineData(105.0f)] // Extreme
    public void MoveLeftUp_ClockwiseFromTop_WithinCell_VariedInitialAngles(float angle)
    {
        var position = new TrainPosition(0.5f, 0.0f, angle, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(Cos45ByRadius, Sin45ByRadius, 135, 0.0f);

        TrainMovement.MoveLeftUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(0.1f)] // Extreme
    [InlineData(0.4f)]
    [InlineData(0.5f)]
    [InlineData(0.6f)]
    [InlineData(0.9f)] // Extreme
    public void MoveLeftUp_ClockwiseFromTop_WithinCell_SnappingToCenter(float relativeLeft)
    {
        var position = new TrainPosition(relativeLeft, 0.0f, 90, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(Cos45ByRadius, Sin45ByRadius, 135, 0.0f);

        TrainMovement.MoveLeftUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(0.5f, 0.0f, 90.0f, HalfCornerTrackDistance, Cos45ByRadius, Sin45ByRadius, 135.0f)]
    [InlineData(0.5f, 0.0f, 90.0f, ThirdCornerTrackDistance, Cos30ByRadius, Sin30ByRadius, 120.0f)]
    [InlineData(0.5f, 0.0f, 90.0f, ThirdCornerTrackDistance * 2, Cos60ByRadius, Sin60ByRadius, 150.0f)]
    public void MoveLeftUp_ClockwiseFromTop_WithinCell_VariedDistance(float initialLeft, float initialTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
    {
        var position = new TrainPosition(initialLeft, initialTop, initialAngle, distance);
        var expectedPos = new TrainPosition(expectedLeft, expectedTop, expectedAngle, 0.0f);

        TrainMovement.MoveLeftUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(Cos45ByRadius, Sin45ByRadius, 135.0f, HalfCornerTrackDistance * 2, (float)HalfCornerTrackDistance)]
    [InlineData(Cos45ByRadius, Sin45ByRadius, 135.0f, 1.0f + HalfCornerTrackDistance, 1.0f)]
    public void MoveLeftUp_ClockwiseFromTop_BeyondCell(float initialLeft, float initialTop, float initialAngle, float distance, float expectedDistance)
    {
        var position = new TrainPosition(initialLeft, initialTop, initialAngle, distance);
        var expectedPos = new TrainPosition(-0.1f, 0.5f, 180, expectedDistance);

        TrainMovement.MoveLeftUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }
}
