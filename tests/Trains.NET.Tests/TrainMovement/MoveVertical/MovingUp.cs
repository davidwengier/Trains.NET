using Trains.NET.Engine;

namespace Trains.NET.Tests.TrainMovementTests.MoveVertical;

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
        var position = new TrainPosition(0.5f, 1.0f, angle, 0.5f);
        var expectedPos = new TrainPosition(0.5f, 0.5f, 270, 0.0f);

        TrainMovement.MoveVertical(position);

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
    public void MoveVertical_MovingUp_WithinCell_SnappingToMiddle(float relativeLeft)
    {
        var position = new TrainPosition(relativeLeft, 1.0f, 270, 0.5f);
        var expectedPos = new TrainPosition(0.5f, 0.5f, 270, 0.0f);

        TrainMovement.MoveVertical(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(1.0f, 0.1f, 0.9f)]
    [InlineData(0.9f, 0.2f, 0.7f)]
    [InlineData(0.9f, 0.5f, 0.4f)]
    [InlineData(0.5f, 0.4f, 0.1f)]
    public void MoveVertical_MovingUp_WithinCell_VariedDistance(float initalTop, float distance, float expectedTop)
    {
        var position = new TrainPosition(0.5f, initalTop, 270, distance);
        var expectedPos = new TrainPosition(0.5f, expectedTop, 270, 0.0f);

        TrainMovement.MoveVertical(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }

    [Theory]
    [InlineData(0.5f, 0.5f, 0.0f)]
    [InlineData(0.5f, 0.6f, 0.1f)]
    [InlineData(0.5f, 1.0f, 0.5f)]
    [InlineData(0.5f, 2.0f, 1.5f)]
    [InlineData(1.0f, 1.0f, 0.0f)]
    public void MoveVertical_MovingUp_BeyondCell(float initalTop, float distance, float expectedDistance)
    {
        var position = new TrainPosition(0.5f, initalTop, 270, distance);
        var expectedPos = new TrainPosition(0.5f, -0.1f, 270, expectedDistance);

        TrainMovement.MoveVertical(position);

        Assert.Equal(expectedPos.RelativeLeft, position.RelativeLeft, 3f);
        Assert.Equal(expectedPos.RelativeTop, position.RelativeTop, 3f);
        Assert.Equal(expectedPos.Angle, position.Angle, 3f);
        Assert.Equal(expectedPos.Distance, position.Distance, 3f);
    }
}
