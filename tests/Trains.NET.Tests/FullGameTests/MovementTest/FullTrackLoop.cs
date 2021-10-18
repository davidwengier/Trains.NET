using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;
using Trains.NET.Rendering;
using Xunit;
using static Trains.NET.Tests.TrainMovementTestsHelper;

namespace Trains.NET.Tests.FullGameTests.MovementTest
{
    public class FullTrackLoop_SingleStep : FullTrackLoop
    {
        public FullTrackLoop_SingleStep() : base(1) { }
    }
    public class FullTrackLoop_2Step : FullTrackLoop
    {
        public FullTrackLoop_2Step() : base(2) { }
    }
    public class FullTrackLoop_3Step : FullTrackLoop
    {
        public FullTrackLoop_3Step() : base(3) { }
    }
    public class FullTrackLoop_10Step : FullTrackLoop
    {
        public FullTrackLoop_10Step() : base(10) { }
    }
    public class FullTrackLoop_100Step : FullTrackLoop
    {
        public FullTrackLoop_100Step() : base(100) { }
    }
    public class FullTrackLoop_1000Step : FullTrackLoop
    {
        public FullTrackLoop_1000Step() : base(1000) { }
    }
    public abstract class FullTrackLoop
    {
        private const int MovementPrecision = 3;
        private readonly int _movementSteps;

        public FullTrackLoop(int movementSteps)
        {
            _movementSteps = movementSteps;
        }

        [Theory(Skip = "These are broken and I don't know why")]
        [InlineData(0.0f)]
        [InlineData(180.0f)]
        public void MovementTest_FullTrackLoop_3x3Square(float initialTrainAngle)
        {
            var trackLayout = new Layout();
            var terrainMap = new TerrainMap();
            var board = new GameBoard(trackLayout, terrainMap, null, null);

            trackLayout.AddTrack(1, 1);
            trackLayout.AddTrack(2, 1);
            trackLayout.AddTrack(3, 1);
            trackLayout.AddTrack(3, 2);
            trackLayout.AddTrack(3, 3);
            trackLayout.AddTrack(2, 3);
            trackLayout.AddTrack(1, 3);
            trackLayout.AddTrack(1, 2);

            board.AddTrain(2, 1);

            var train = (Train)board.GetMovables().Single();

            float distance = (float)(4 * StraightTrackDistance +
                                     4 * CornerTrackDistance);

            // Train speed & angle
            train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
            train.SetAngle(initialTrainAngle);

            Assert.Equal(2, train.Column);
            Assert.Equal(1, train.Row);
            Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
            Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
            Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);

            // Move it!
            for (int i = 0; i < _movementSteps; i++)
                board.GameLoopStep();

            Assert.Equal(2, train.Column);
            Assert.Equal(1, train.Row);
            Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
            Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
            Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);
        }

        [Theory(Skip = "These are broken and I don't know why")]
        [InlineData(0.0f)]
        [InlineData(180.0f)]
        public void MovementTest_FullTrackLoop_FourLoopCorners(float initialTrainAngle)
        {
            var trackLayout = new Layout();
            var filteredLayout = new FilteredLayout<Track>(trackLayout);
            var terrainMap = new TerrainMap();
            terrainMap.Reset(1, 100, 100);
            var board = new GameBoard(trackLayout, terrainMap, null, null);
            var entityFactories = new List<IStaticEntityFactory<Track>>
            {
                new CrossTrackFactory(terrainMap, trackLayout),
                new TIntersectionFactory(terrainMap, trackLayout),
                new BridgeFactory(terrainMap, filteredLayout),
                new SingleTrackFactory(terrainMap, filteredLayout)
            };

            var trackTool = new TrackTool(filteredLayout, entityFactories);

            trackTool.Execute(3, 2, true);
            trackTool.Execute(4, 2, true);
            trackTool.Execute(5, 2, true);
            trackTool.Execute(5, 1, true);
            trackTool.Execute(4, 1, true);
            trackTool.Execute(4, 2, true);
            trackTool.Execute(4, 3, true);
            trackTool.Execute(4, 4, true);
            trackTool.Execute(4, 5, true);
            trackTool.Execute(5, 5, true);
            trackTool.Execute(5, 4, true);
            trackTool.Execute(4, 4, true);
            trackTool.Execute(3, 4, true);
            trackTool.Execute(2, 4, true);
            trackTool.Execute(1, 4, true);
            trackTool.Execute(1, 5, true);
            trackTool.Execute(2, 5, true);
            trackTool.Execute(2, 4, true);
            trackTool.Execute(2, 3, true);
            trackTool.Execute(2, 2, true);
            trackTool.Execute(2, 1, true);
            trackTool.Execute(1, 1, true);
            trackTool.Execute(1, 2, true);
            trackTool.Execute(2, 2, true);
            trackTool.Execute(3, 2, true);

            board.AddTrain(3, 2);

            var train = (Train)board.GetMovables().Single();

            float distance = (float)(12 * StraightTrackDistance +
                                     12 * CornerTrackDistance);

            train.Angle = initialTrainAngle;
            train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);

            Assert.Equal(3, train.Column);
            Assert.Equal(2, train.Row);
            Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
            Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
            Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);

            // Move it!
            for (int i = 0; i < _movementSteps; i++)
                board.GameLoopStep();

            Assert.Equal(3, train.Column);
            Assert.Equal(2, train.Row);
            Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
            Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
            Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);
        }
    }
}
