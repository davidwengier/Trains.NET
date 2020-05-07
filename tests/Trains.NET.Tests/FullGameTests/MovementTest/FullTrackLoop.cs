using System.Linq;
using Trains.NET.Engine;
using Xunit;

#nullable disable

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
    public abstract class FullTrackLoop : TrainMovementTestsHelper
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
            var board = new GameBoard(null, null);

            board.AddTrack(1, 1);
            board.AddTrack(2, 1);
            board.AddTrack(3, 1);
            board.AddTrack(3, 2);
            board.AddTrack(3, 3);
            board.AddTrack(2, 3);
            board.AddTrack(1, 3);
            board.AddTrack(1, 2);

            board.AddTrain(2, 1);

            Train train = (Train)board.GetMovables().Single();

            float distance = (float)(4 * StraightTrackDistance +
                                     4 * CornerTrackDistance);

            // Train speed & angle
            train.Speed = distance / _movementSteps / GameBoard.SpeedScaleModifier;
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
            var board = new GameBoard(null, null);

            board.AddTrack(3, 2);
            board.AddTrack(4, 2);
            board.AddTrack(5, 2);
            board.AddTrack(5, 1);
            board.AddTrack(4, 1);
            board.AddTrack(4, 3);
            board.AddTrack(4, 4);
            board.AddTrack(4, 5);
            board.AddTrack(5, 5);
            board.AddTrack(5, 4);
            board.AddTrack(3, 4);
            board.AddTrack(2, 4);
            board.AddTrack(1, 4);
            board.AddTrack(1, 5);
            board.AddTrack(2, 5);
            board.AddTrack(2, 3);
            // Skip until end!
            board.AddTrack(2, 1);
            board.AddTrack(1, 1);
            board.AddTrack(1, 2);
            // Finish it off
            board.AddTrack(2, 2);

            board.AddTrain(3, 2);

            Train train = (Train)board.GetMovables().Single();

            float distance = (float)(12 * StraightTrackDistance +
                                     12 * CornerTrackDistance);

            train.Angle = initialTrainAngle;
            train.Speed = distance / _movementSteps / GameBoard.SpeedScaleModifier;

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
