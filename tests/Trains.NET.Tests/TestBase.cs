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
            Train? train = (Train)GameBoard.GetMovables().First();
            
            train.SetAngle(startAngle);
            // Run until we get to the very end of the track
            train.FrontEdgeDistance = 0.01f;

            Timer.Tick();
            Timer.Tick();
            Timer.Tick();
            Timer.Tick();
            Timer.Tick();
            Timer.Tick();
            Timer.Tick();
            Timer.Tick();
            Timer.Tick();
            Timer.Tick();
            // BOOM!

            Assert.Equal((endColumn, endRow), (train.Column, train.Row));
        }

        public void Dispose()
        {
            Timer.Dispose();
            GameBoard.Dispose();
        }
    }
}
