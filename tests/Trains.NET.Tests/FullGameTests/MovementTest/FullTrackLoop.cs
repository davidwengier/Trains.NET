using System.Linq;
using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;
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

        [Theory]
        [InlineData(0.0f)]
        [InlineData(180.0f)]
        public void MovementTest_FullTrackLoop_3x3Square(float initialTrainAngle)
        {
            var trackLayout = new TrackLayout();
            var board = new GameBoard(trackLayout, null, null);

            trackLayout.Add(1, 1, new Track(trackLayout));
            trackLayout.Add(2, 1, new Track(trackLayout));
            trackLayout.Add(3, 1, new Track(trackLayout));
            trackLayout.Add(3, 2, new Track(trackLayout));
            trackLayout.Add(3, 3, new Track(trackLayout));
            trackLayout.Add(2, 3, new Track(trackLayout));
            trackLayout.Add(1, 3, new Track(trackLayout));
            trackLayout.Add(1, 2, new Track(trackLayout));

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

        [Theory]
        [InlineData(0.0f)]
        [InlineData(180.0f)]
        public void MovementTest_FullTrackLoop_FourLoopCorners(float initialTrainAngle)
        {
            var trackLayout = new TrackLayout();
            var board = new GameBoard(trackLayout, null, null);

            trackLayout.Add(3, 2, new Track(trackLayout));
            trackLayout.Add(4, 2, new Track(trackLayout));
            trackLayout.Add(5, 2, new Track(trackLayout));
            trackLayout.Add(5, 1, new Track(trackLayout));
            trackLayout.Add(4, 1, new Track(trackLayout));
            trackLayout.Add(4, 3, new Track(trackLayout));
            trackLayout.Add(4, 4, new Track(trackLayout));
            trackLayout.Add(4, 5, new Track(trackLayout));
            trackLayout.Add(5, 5, new Track(trackLayout));
            trackLayout.Add(5, 4, new Track(trackLayout));
            trackLayout.Add(3, 4, new Track(trackLayout));
            trackLayout.Add(2, 4, new Track(trackLayout));
            trackLayout.Add(1, 4, new Track(trackLayout));
            trackLayout.Add(1, 5, new Track(trackLayout));
            trackLayout.Add(2, 5, new Track(trackLayout));
            trackLayout.Add(2, 3, new Track(trackLayout));
            // Skip until end!
            trackLayout.Add(2, 1, new Track(trackLayout));
            trackLayout.Add(1, 1, new Track(trackLayout));
            trackLayout.Add(1, 2, new Track(trackLayout));
            // Finish it off
            trackLayout.Add(2, 2, new Track(trackLayout));

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
