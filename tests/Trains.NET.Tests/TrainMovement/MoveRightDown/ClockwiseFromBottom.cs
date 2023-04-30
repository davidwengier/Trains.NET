using Trains.NET.Engine;
using static Trains.NET.Tests.TrainMovementTestsHelper;

namespace Trains.NET.Tests.TrainMovementTests.MoveRightDown;

public class ClockwiseFromBottom
{
    [Theory]
    [InlineData(255.0f)] // Extreme
    [InlineData(269.0f)]
    [InlineData(270.0f)]
    [InlineData(271.0f)]
    [InlineData(285.0f)] // Extreme
    public void MoveRightDown_ClockwiseFromBottom_WithinCell_VariedInitialAngles(float angle)
    {
        var position = new TrainPosition(0.5f, 1.0f, angle, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315, 0.0f);

        TrainMovement.MoveRightDown(position);

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
    public void MoveRightDown_ClockwiseFromBottom_WithinCell_SnappingToCenter(float relativeLeft)
    {
        var position = new TrainPosition(relativeLeft, 1.0f, 270, (float)HalfCornerTrackDistance);
        var expectedPos = new TrainPosition(1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315, 0.0f);

        TrainMovement.MoveRightDown(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(0.5f, 1.0f, 270.0f, HalfCornerTrackDistance, 1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315.0f)]
    [InlineData(0.5f, 1.0f, 270.0f, ThirdCornerTrackDistance, 1.0f - Cos30ByRadius, 1.0f - Sin30ByRadius, 300.0f)]
    [InlineData(0.5f, 1.0f, 270.0f, ThirdCornerTrackDistance * 2, 1.0f - Cos60ByRadius, 1.0f - Sin60ByRadius, 330.0f)]
    [InlineData(1.0f - Cos30ByRadius, 1.0f - Sin30ByRadius, 315.0f, ThirdCornerTrackDistance, 1.0f - Cos60ByRadius, 1.0f - Sin60ByRadius, 330.0f)]
    public void MoveRightDown_ClockwiseFromBottom_WithinCell_VariedDistance(float initalLeft, float initalTop, float initialAngle, float distance, float expectedLeft, float expectedTop, float expectedAngle)
    {
        var position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
        var expectedPos = new TrainPosition(expectedLeft, expectedTop, expectedAngle, 0.0f);

        TrainMovement.MoveRightDown(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(0.5f, 1.0f, 270.0f, HalfCornerTrackDistance * 3, HalfCornerTrackDistance)]
    [InlineData(1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315.0f, HalfCornerTrackDistance * 2, HalfCornerTrackDistance)]
    [InlineData(1.0f - Cos45ByRadius, 1.0f - Sin45ByRadius, 315.0f, 1.0f + HalfCornerTrackDistance, 1.0f)]
    public void MoveRightDown_ClockwiseFromBottom_BeyondCell(float initalLeft, float initalTop, float initialAngle, float distance, float expectedDistance)
    {
        var position = new TrainPosition(initalLeft, initalTop, initialAngle, distance);
        var expectedPos = new TrainPosition(1.1f, 0.5f, 0, expectedDistance);

        TrainMovement.MoveRightDown(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }
}
