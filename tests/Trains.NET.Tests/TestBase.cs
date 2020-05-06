using System;
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
            GameBoard = new GameBoard(Storage, Timer);
        }

        protected void AssertTrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            var train = GameBoard.AddTrain(startColumn, startRow) as Train;

            train!.LookaheadDistance = 0.1f;
            train.SetAngle(startAngle);

            for (int i = 0; i < 500; i++)
            {
                Timer.Tick();
            }

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
