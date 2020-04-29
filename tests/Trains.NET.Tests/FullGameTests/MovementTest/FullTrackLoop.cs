using System;
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
    public abstract class FullTrackLoop
    {
        private const int MovementPrecision = 3;
        private const float Left45Degrees = 0.35355339059327376220042218105242f;
        private const float Top45Degrees = 0.35355339059327376220042218105242f;
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

            // Set the train's direction
            train.SetAngle(initialTrainAngle);

            Assert.Equal(2, train.Column);
            Assert.Equal(1, train.Row);
            Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
            Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
            Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);

            // 4 Tracks + 1 whole circle
            float radius = 0.5f;
            float circleCircumference = 2.0f * (float)Math.PI * radius;
            float distance = 4.0f + circleCircumference;

            // Move it!
            for (int i = 0; i < _movementSteps; i++)
                board.GameLoopStep(distance / _movementSteps);

            Assert.Equal(2, train.Column);
            Assert.Equal(1, train.Row);
            Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
            Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
            Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);
        }

        [Theory]
        [InlineData(135.0f)]
        [InlineData(315.0f)]
        public void MovementTest_FullTrackLoop_ScottsAllCurves(float initialTrainAngle)
        {
            var board = new GameBoard(null, null);

            board.AddTrack(3, 1);
            board.AddTrack(2, 1);
            board.AddTrack(2, 2);
            board.AddTrack(1, 2);
            board.AddTrack(1, 3);
            board.AddTrack(2, 3);
            board.AddTrack(2, 4);
            board.AddTrack(3, 4);
            board.AddTrack(3, 3);
            board.AddTrack(4, 3);
            board.AddTrack(4, 2);
            board.AddTrack(3, 2);

            board.AddTrain(2, 1);

            Train train = (Train)board.GetMovables().Single();

            float radius = 0.5f;
            float circleCircumference = 2.0f * (float)Math.PI * radius;
            float distance = 3 * circleCircumference;

            float initialLeft = 1.0f - Left45Degrees; // We are on a down right, so flip it!
            float initialTop = 1.0f - Top45Degrees; // We are on a down right, so flip it!

            train.RelativeLeft = initialLeft;
            train.RelativeTop = initialTop;
            train.Angle = initialTrainAngle;

            Assert.Equal(2, train.Column);
            Assert.Equal(1, train.Row);
            Assert.Equal(initialLeft, train.RelativeLeft, MovementPrecision);
            Assert.Equal(initialTop, train.RelativeTop, MovementPrecision);
            Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);

            // Move it!
            for (int i = 0; i < _movementSteps; i++)
                board.GameLoopStep(distance / _movementSteps);

            Assert.Equal(2, train.Column);
            Assert.Equal(1, train.Row);
            Assert.Equal(initialLeft, train.RelativeLeft, MovementPrecision);
            Assert.Equal(initialTop, train.RelativeTop, MovementPrecision);
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

            float radius = 0.5f;
            float circleCircumference = 2.0f * (float)Math.PI * radius;
            float distance = 12 + 3 * circleCircumference;

            train.Angle = initialTrainAngle;

            Assert.Equal(3, train.Column);
            Assert.Equal(2, train.Row);
            Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
            Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
            Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);

            // Move it!
            for (int i = 0; i < _movementSteps; i++)
                board.GameLoopStep(distance / _movementSteps);

            Assert.Equal(3, train.Column);
            Assert.Equal(2, train.Row);
            Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
            Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
            Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);
        }
    }
}
