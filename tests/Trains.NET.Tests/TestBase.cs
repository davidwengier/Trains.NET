using System;
using Trains.NET.Engine;
using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests
{
    public class TestBase : IDisposable
    {
        private readonly ITestOutputHelper _output;
        internal readonly IGameStorage Storage;
        internal readonly TestTimer Timer;
        internal readonly GameBoard GameBoard;
        internal readonly ILayout TrackLayout;
        internal readonly ITerrainMap TerrainMap;

        protected TestBase(ITestOutputHelper output)
        {
            Storage = new NullStorage();
            Timer = new TestTimer();
            TrackLayout = new Layout();
            TerrainMap = new TerrainMap();
            GameBoard = new GameBoard(TrackLayout, TerrainMap, Storage, Timer);
            _output = output;
        }

        protected void AssertTrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            var train = GameBoard.AddTrain(startColumn, startRow) as Train;

            train!.LookaheadDistance = 0.1f;
            train.SetAngle(startAngle);

            _output.WriteLine("Initial: " + train);

            for (int i = 0; i < 100; i++)
            {
                Timer.Tick();
                _output.WriteLine($"Tick {i}: {train}");
            }

            Assert.Equal((endColumn, endRow), (train.Column, train.Row));
        }

        public void Dispose()
        {
            Timer.Dispose();
            GameBoard.Dispose();
        }
    }
}
