using Trains.NET.Engine;
using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests
{
    public class TIntersectionTests : TestBase
    {
        public TIntersectionTests(ITestOutputHelper output) : base(output)
        { }

        [Theory]
        [InlineData(TrainAngle.TrainFacingDown, 1, 1, 2, 2)]
        [InlineData(TrainAngle.TrainFacingUp, 1, 3, 2, 2)]
        [InlineData(TrainAngle.TrainFacingLeft, 2, 2, 1, 1)]
        public void RightUpDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(1, 3);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(1, 2);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngle.TrainFacingDown, 2, 1, 1, 2)]
        [InlineData(TrainAngle.TrainFacingUp, 2, 3, 1, 2)]
        [InlineData(TrainAngle.TrainFacingRight, 1, 2, 2, 3)]
        public void LeftUpDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 2);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngle.TrainFacingRight, 1, 1, 2, 2)]
        [InlineData(TrainAngle.TrainFacingLeft, 3, 1, 2, 2)]
        [InlineData(270, 2, 2, 3, 1)]
        public void LeftRightDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(3, 1);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(2, 1);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngle.TrainFacingRight, 1, 2, 2, 1)]
        [InlineData(TrainAngle.TrainFacingLeft, 3, 2, 2, 1)]
        [InlineData(TrainAngle.TrainFacingDown, 2, 1, 1, 2)]
        public void LeftRightUp_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }
    }
}
