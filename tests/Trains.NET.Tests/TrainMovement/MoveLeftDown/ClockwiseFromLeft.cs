using Trains.NET.Engine;
using static Trains.NET.Tests.TrainMovementTestsHelper;

namespace Trains.NET.Tests.TrainMovementTests.MoveLeftDown;

public class ClockwiseFromLeft
{
    [Theory]
    [InlineData(345.0f)] // Extreme
    [InlineData(359.0f)]
    [InlineData(360.0f)]
    [InlineData(0.0f)]
    [InlineData(1.0f)]
    [InlineData(15.0f)] // Extreme
    public void MoveLeftDown_ClockwiseFromLeft_WithinCell_VariedInitialAngles(float angle)
    {
        var position = new TrainPosition(0.0f, 0.5f, angle, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(Cos45ByRadius, 1.0f - Sin45ByRadius, 45, 0.0f);

        TrainMovement.MoveLeftDown(position);

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
    public void MoveLeftDown_ClockwiseFromLeft_WithinCell_SnappingToCenter(float relativeTop)
    {
        var position = new TrainPosition(0.0f, relativeTop, 0, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(Cos45ByRadius, 1.0f - Sin45ByRadius, 45, 0.0f);

        TrainMovement.MoveLeftDown(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(0.0f, 0.5f, 0.0f, HalfCornerTrackDistance, Cos45ByRadius, 1.0f - Sin45ByRadius, 45.0f)]
    [InlineData(0.0f, 0.5f, 0.0f, ThirdCornerTrackDistance, Cos60ByRadius, 1.0f - Sin60ByRadius, 30.0f)]
    [InlineData(0.0f, 0.5f, 0.0f, ThirdCornerTrackDistance * 2, Cos30ByRadius, 1.0f - Sin30ByRadius, 60.0f)]
    [InlineData(Cos60ByRadius, 1.0f - Sin60ByRadius, 30, ThirdCornerTrackDistance, Cos30ByRadius, 1.0f - Sin30ByRadius, 60.0f)]
    public void MoveLeftDown_ClockwiseFromLeft_WithinCell_VariedDistance(float initialLeft, float initialTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
    {
        var position = new TrainPosition(initialLeft, initialTop, initialAngle, distance);
        var expectedPos = new TrainPosition(expectedLeft, expectedTop, expectedAngle, 0.0f);

        TrainMovement.MoveLeftDown(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(0.0f, 0.5f, 0.0f, HalfCornerTrackDistance * 3, HalfCornerTrackDistance)]
    [InlineData(Cos45ByRadius, 1.0f - Sin45ByRadius, 45.0f, HalfCornerTrackDistance * 2, HalfCornerTrackDistance)]
    [InlineData(Cos45ByRadius, 1.0f - Sin45ByRadius, 45.0f, 1.0f + HalfCornerTrackDistance, 1.0f)]
    public void MoveLeftDown_ClockwiseFromLeft_BeyondCell(float initialLeft, float initialTop, float initialAngle, float distance, float expectedDistance)
    {
        var position = new TrainPosition(initialLeft, initialTop, initialAngle, distance);
        var expectedPos = new TrainPosition(0.5f, 1.1f, 90.0f, expectedDistance);

        TrainMovement.MoveLeftDown(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }
}
