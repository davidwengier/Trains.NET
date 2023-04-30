using Trains.NET.Engine;
using static Trains.NET.Tests.TrainMovementTestsHelper;

namespace Trains.NET.Tests.TrainMovementTests.MoveLeftUp;

public class CounterClockwiseFromLeft
{
    [Theory]
    [InlineData(345.0f)] // Extreme
    [InlineData(359.0f)]
    [InlineData(360.0f)]
    [InlineData(0.0f)]
    [InlineData(1.0f)]
    [InlineData(15.0f)] // Extreme
    public void MoveLeftUp_CounterClockwiseFromLeft_WithinCell_VariedInitialAngles(float angle)
    {
        var position = new TrainPosition(0.0f, 0.5f, angle, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(Cos45ByRadius, Sin45ByRadius, 315.0f, 0.0f);

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
    public void MoveLeftUp_CounterClockwiseFromLeft_WithinCell_SnappingToCenter(float relativeTop)
    {
        var position = new TrainPosition(0.0f, relativeTop, 0.0f, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(Cos45ByRadius, Sin45ByRadius, 315.0f, 0.0f);

        TrainMovement.MoveLeftUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(0.0f, 0.5f, 0.0f, HalfCornerTrackDistance, Cos45ByRadius, Sin45ByRadius, 315.0f)]
    [InlineData(0.0f, 0.5f, 0.0f, ThirdCornerTrackDistance, Cos60ByRadius, Sin60ByRadius, 330.0f)]
    [InlineData(0.0f, 0.5f, 0.0f, ThirdCornerTrackDistance * 2, Cos30ByRadius, Sin30ByRadius, 300.0f)]
    [InlineData(Cos60ByRadius, Sin60ByRadius, 330.0f, ThirdCornerTrackDistance, Cos30ByRadius, Sin30ByRadius, 300.0f)]
    public void MoveLeftUp_CounterClockwiseFromLeft_WithinCell_VariedDistance(float initalLeft, float initalTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
    {
        var position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
        var expectedPos = new TrainPosition(expectedLeft, expectedTop, expectedAngle, 0.0f);

        TrainMovement.MoveLeftUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(0.0f, 0.5f, 0.0f, HalfCornerTrackDistance * 3, HalfCornerTrackDistance)]
    [InlineData(Cos45ByRadius, Sin45ByRadius, 315.0f, HalfCornerTrackDistance * 2, HalfCornerTrackDistance)]
    [InlineData(Cos45ByRadius, Sin45ByRadius, 315.0f, 1.0f + HalfCornerTrackDistance, 1.0f)]
    public void MoveLeftUp_CounterClockwiseFromLeft_BeyondCell(float initalLeft, float initalTop, float initialAngle, float distance, float expectedDistance)
    {
        var position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
        var expectedPos = new TrainPosition(0.5f, -0.1f, 270.0f, expectedDistance);

        TrainMovement.MoveLeftUp(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }
}
