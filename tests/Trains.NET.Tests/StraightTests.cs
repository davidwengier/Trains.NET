using Trains.NET.Engine;

namespace Trains.NET.Tests;

public class StraightTests(ITestOutputHelper output) : TestBase(output)
{
    [Theory]
    [InlineData(TrainAngleHelper.TrainFacingUp, 1, 3, 1, 1)]
    [InlineData(TrainAngleHelper.TrainFacingDown, 1, 1, 1, 3)]
    public void Vertical_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));
        TrackTool.Execute(1, 3, new ExecuteInfo(1, 3));

        AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
    }

    [Theory]
    [InlineData(TrainAngleHelper.TrainFacingRight, 1, 1, 3, 1)]
    [InlineData(TrainAngleHelper.TrainFacingLeft, 3, 1, 1, 1)]
    public void Horizontal_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(1, 1));
        TrackTool.Execute(3, 1, new ExecuteInfo(2, 1));

        AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
    }

    [Theory]
    [InlineData(TrainAngleHelper.TrainFacingRight, 1, 2, 3, 2)]
    [InlineData(TrainAngleHelper.TrainFacingLeft, 3, 2, 1, 2)]
    [InlineData(TrainAngleHelper.TrainFacingUp, 2, 3, 2, 1)]
    [InlineData(TrainAngleHelper.TrainFacingDown, 2, 1, 2, 3)]
    public void Cross_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
    {
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));
        TrackTool.Execute(1, 2, new ExecuteInfo(2, 3));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));

        AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
    }
}
