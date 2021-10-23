using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests
{
    public class TIntersectionTests : TestBase
    {
        public TIntersectionTests(ITestOutputHelper output) : base(output)
        { }

        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingDown, 1, 1, 2, 2)]
        [InlineData(TrainAngleHelper.TrainFacingUp, 1, 3, 2, 2)]
        [InlineData(TrainAngleHelper.TrainFacingLeft, 2, 2, 1, 1)]
        public void RightUpDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            TrackLayout.AddTrack(1, 1, Engine.SingleTrackDirection.Vertical);
            TrackLayout.AddTIntersectionTrack(1, 2, Engine.TIntersectionDirection.RightUp_RightDown, Engine.TIntersectionStyle.CornerAndPrimary);
            TrackLayout.AddTrack(1, 3, Engine.SingleTrackDirection.Vertical);
            TrackLayout.AddTrack(2, 2, Engine.SingleTrackDirection.Horizontal);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingDown, 2, 1, 1, 2)]
        [InlineData(TrainAngleHelper.TrainFacingUp, 2, 3, 1, 2)]
        [InlineData(TrainAngleHelper.TrainFacingRight, 1, 2, 2, 3)]
        public void LeftUpDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            TrackLayout.AddTrack(2, 1, Engine.SingleTrackDirection.Vertical);
            TrackLayout.AddTIntersectionTrack(2, 2, Engine.TIntersectionDirection.LeftDown_LeftUp, Engine.TIntersectionStyle.CornerAndPrimary);
            TrackLayout.AddTrack(2, 3, Engine.SingleTrackDirection.Vertical);
            TrackLayout.AddTrack(1, 2, Engine.SingleTrackDirection.Horizontal);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingRight, 1, 1, 2, 2)]
        [InlineData(TrainAngleHelper.TrainFacingLeft, 3, 1, 2, 2)]
        [InlineData(270, 2, 2, 3, 1)]
        public void LeftRightDown_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            TrackLayout.AddTrack(1, 1, Engine.SingleTrackDirection.Horizontal);
            TrackLayout.AddTIntersectionTrack(2, 1, Engine.TIntersectionDirection.RightDown_LeftDown, Engine.TIntersectionStyle.CornerAndPrimary);
            TrackLayout.AddTrack(3, 1, Engine.SingleTrackDirection.Horizontal);
            TrackLayout.AddTrack(2, 2, Engine.SingleTrackDirection.Vertical);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }

        [Theory]
        [InlineData(TrainAngleHelper.TrainFacingRight, 1, 2, 2, 1)]
        [InlineData(TrainAngleHelper.TrainFacingLeft, 3, 2, 2, 1)]
        [InlineData(TrainAngleHelper.TrainFacingDown, 2, 1, 1, 2)]
        public void LeftRightUp_TrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
        {
            TrackLayout.AddTrack(1, 2, Engine.SingleTrackDirection.Horizontal);
            TrackLayout.AddTIntersectionTrack(2, 2, Engine.TIntersectionDirection.LeftUp_RightUp, Engine.TIntersectionStyle.CornerAndPrimary);
            TrackLayout.AddTrack(3, 2, Engine.SingleTrackDirection.Horizontal);
            TrackLayout.AddTrack(2, 1, Engine.SingleTrackDirection.Vertical);

            AssertTrainMovement(startAngle, startColumn, startRow, endColumn, endRow);
        }
    }
}
