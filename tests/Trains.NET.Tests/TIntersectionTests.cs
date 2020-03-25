using System.Linq;
using Trains.NET.Engine;
using Xunit;

namespace Trains.NET.Tests
{
    public class TIntersectionTests
    {
        private readonly IGameStorage _storage = new NullStorage();

        [Theory]
        [InlineData(90, 1, 1, 2, 2)]
        [InlineData(270, 1, 3, 2, 2)]
        [InlineData(180, 2, 2, 1, 1)]
        public void RightUpDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            var timer = new TestTimer();
            var gameBoard = new GameBoard(_storage, timer);
            gameBoard.SpeedAdjustmentFactor = 100;

            gameBoard.AddTrack(1, 1);
            gameBoard.AddTrack(1, 2);
            gameBoard.AddTrack(1, 3);
            gameBoard.AddTrack(2, 2);
            gameBoard.AddTrack(1, 2);

            gameBoard.AddTrain(startColumn, startRow);
            IMovable? train = gameBoard.GetMovables().First();
            train.SetAngle(startAngle);

            timer.Tick();
            timer.Tick();
            timer.Tick();

            Assert.Equal(endColumn, train.Column);
            Assert.Equal(endRow, train.Row);
        }

        [Theory]
        [InlineData(90, 2, 1, 1, 2)]
        [InlineData(270, 2, 3, 1, 2)]
        [InlineData(0, 1, 2, 2, 3)]
        public void LeftUpDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            var timer = new TestTimer();
            var gameBoard = new GameBoard(_storage, timer);
            gameBoard.SpeedAdjustmentFactor = 100;

            gameBoard.AddTrack(2, 1);
            gameBoard.AddTrack(2, 2);
            gameBoard.AddTrack(2, 3);
            gameBoard.AddTrack(1, 2);
            gameBoard.AddTrack(2, 2);

            gameBoard.AddTrain(startColumn, startRow);
            IMovable? train = gameBoard.GetMovables().First();
            train.SetAngle(startAngle);

            timer.Tick();
            timer.Tick();
            timer.Tick();

            Assert.Equal(endColumn, train.Column);
            Assert.Equal(endRow, train.Row);
        }
    }
}
