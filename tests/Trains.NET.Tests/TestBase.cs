using System;
using System.Linq;
using Trains.NET.Engine;
using Xunit;

namespace Trains.NET.Tests
{
    public class TestBase : IDisposable
    {
        internal readonly IGameStorage Storage;
        internal readonly TestTimer Timer;
        internal readonly GameBoard GameBoard;

        protected TestBase()
        {
            Storage = new NullStorage();
            Timer = new TestTimer();
            GameBoard = new GameBoard(Storage, Timer)
            {
                SpeedAdjustmentFactor = 100
            };
        }

        protected void AssertTrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            GameBoard.AddTrain(startColumn, startRow);
            IMovable? train = GameBoard.GetMovables().First();
            train.SetAngle(startAngle);

            Timer.Tick();
            Timer.Tick();
            Timer.Tick();

            Assert.Equal(endColumn, train.Column);
            Assert.Equal(endRow, train.Row);
        }

        public void Dispose()
        {
            Timer.Dispose();
            GameBoard.Dispose();
        }
    }
}
