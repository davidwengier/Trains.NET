using Trains.NET.Engine;
using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests
{
    public class CornerTests : TestBase
    {
        public CornerTests(ITestOutputHelper output) : base(output)
        { }

        [Theory]
        [InlineData(TrainAngle.TrainFacingLeft, 2, 2, 1, 1)]
        [InlineData(TrainAngle.TrainFacingDown, 1, 1, 2, 2)]
        public void RightUp_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 2);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngle.TrainFacingRight, 1, 2, 2, 1)]
        [InlineData(TrainAngle.TrainFacingDown, 2, 1, 1, 2)]
        public void LeftUp_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(1, 2);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngle.TrainFacingUp, 1, 2, 2, 1)]
        [InlineData(TrainAngle.TrainFacingLeft, 2, 1, 1, 2)]
        public void RightDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngle.TrainFacingRight, 1, 1, 2, 2)]
        [InlineData(TrainAngle.TrainFacingUp, 2, 2, 1, 1)]
        public void LeftDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }
    }
}
