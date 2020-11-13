using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests
{
    public class StraightTests : TestBase
    {
        public StraightTests(ITestOutputHelper output) : base(output)
        { }

        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingUp, 1, 3, 1, 1)]
        [InlineData(TrainAngleHelper.TrainFacingDown, 1, 1, 1, 3)]
        public void Vertical_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(1, 3, true);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingRight, 1, 1, 3, 1)]
        [InlineData(TrainAngleHelper.TrainFacingLeft, 3, 1, 1, 1)]
        public void Horizontal_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(3, 1, true);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingRight, 1, 2, 3, 2)]
        [InlineData(TrainAngleHelper.TrainFacingLeft, 3, 2, 1, 2)]
        [InlineData(TrainAngleHelper.TrainFacingUp, 2, 3, 2, 1)]
        [InlineData(TrainAngleHelper.TrainFacingDown, 2, 1, 2, 3)]
        public void Cross_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 2, true);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }
    }
}
