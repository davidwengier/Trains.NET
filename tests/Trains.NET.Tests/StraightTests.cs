using Trains.NET.Engine;
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
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(1, 3);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingRight, 1, 1, 3, 1)]
        [InlineData(TrainAngleHelper.TrainFacingLeft, 3, 1, 1, 1)]
        public void Horizontal_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(3, 1);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingRight, 1, 2, 3, 2)]
        [InlineData(TrainAngleHelper.TrainFacingLeft, 3, 2, 1, 2)]
        [InlineData(TrainAngleHelper.TrainFacingUp, 2, 3, 2, 1)]
        [InlineData(TrainAngleHelper.TrainFacingDown, 2, 1, 2, 3)]
        public void Cross_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(3, 2);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }
    }
}
