using Trains.NET.Engine;
using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests;

public class CornerTests : TestBase
{
    public CornerTests(ITestOutputHelper output) : base(output)
    { }

    [Theory]
    [InlineData(TrainAngleHelper.TrainFacingLeft, 2, 2, 1, 1)]
    [InlineData(TrainAngleHelper.TrainFacingDown, 1, 1, 2, 2)]
    public void RightUp_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));

        AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
    }

    [Theory]
    [InlineData(TrainAngleHelper.TrainFacingRight, 1, 2, 2, 1)]
    [InlineData(TrainAngleHelper.TrainFacingDown, 2, 1, 1, 2)]
    public void LeftUp_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
    {
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));
        TrackTool.Execute(1, 2, new ExecuteInfo(2, 2));

        AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
    }

    [Theory]
    [InlineData(TrainAngleHelper.TrainFacingUp, 1, 2, 2, 1)]
    [InlineData(TrainAngleHelper.TrainFacingLeft, 2, 1, 1, 2)]
    public void RightDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
    {
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 1, new ExecuteInfo(2, 1));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));

        AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
    }

    [Theory]
    [InlineData(TrainAngleHelper.TrainFacingRight, 1, 1, 2, 2)]
    [InlineData(TrainAngleHelper.TrainFacingUp, 2, 2, 1, 1)]
    public void LeftDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(1, 1));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));

        AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
    }
}
