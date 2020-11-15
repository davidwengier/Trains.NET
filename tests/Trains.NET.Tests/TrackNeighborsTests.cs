using Trains.NET.Engine;
using Xunit;

namespace Trains.NET.Tests
{
    public class TrackNeighborsTests
    {
        [Theory]
        [InlineData(SingleTrackDirection.Horizontal, SingleTrackDirection.Vertical, SingleTrackDirection.Horizontal, SingleTrackDirection.Vertical, SingleTrackDirection.Horizontal, false, false, false, false)]
        [InlineData(SingleTrackDirection.Horizontal, SingleTrackDirection.Vertical, SingleTrackDirection.Vertical, SingleTrackDirection.Vertical, SingleTrackDirection.Vertical, false, false, false, false)]
        [InlineData(SingleTrackDirection.Vertical, SingleTrackDirection.Vertical, SingleTrackDirection.Vertical, SingleTrackDirection.Vertical, SingleTrackDirection.Vertical, false, true, false, true)]
        //[InlineData(TrackDirection.RightDown_LeftDown, TrackDirection.Horizontal, TrackDirection.Vertical, TrackDirection.Horizontal, TrackDirection.Vertical, true, false, true, true)]
        //[InlineData(TrackDirection.RightDown_LeftDown, TrackDirection.Horizontal, TrackDirection.Horizontal, TrackDirection.Horizontal, TrackDirection.Vertical, true, false, true, true)]
        public void GetConnectedNeighbors(SingleTrackDirection centerDir, SingleTrackDirection leftDir, SingleTrackDirection upDir, SingleTrackDirection rightDir, SingleTrackDirection downDir, bool left, bool up, bool right, bool down)
        {
            // The real layout evaluates the best direction, so we can't use it. We just want storage
            var layout = new TestLayout();

            layout.AddTrack(5, 5, centerDir);

            layout.AddTrack(4, 5, leftDir);
            layout.AddTrack(5, 4, upDir);
            layout.AddTrack(6, 5, rightDir);
            layout.AddTrack(5, 6, downDir);

            var track = layout.GetTrackAt<SingleTrack>(5, 5);
            var neighbors = TrackNeighbors.GetConnectedNeighbours(layout, track.Column, track.Row, false, false);

            Assert.Equal(left, neighbors.Left != null);
            Assert.Equal(up, neighbors.Up != null);
            Assert.Equal(right, neighbors.Right != null);
            Assert.Equal(down, neighbors.Down != null);
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(false, false, false, true)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, true, true)]
        [InlineData(false, true, false, false)]
        [InlineData(false, true, false, true)]
        [InlineData(false, true, true, false)]
        [InlineData(false, true, true, true)]
        [InlineData(true, false, false, false)]
        [InlineData(true, false, false, true)]
        [InlineData(true, false, true, false)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, false, false)]
        [InlineData(true, true, false, true)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, true, true)]
        public void GetAllNeighbors(bool left, bool up, bool right, bool down)
        {
            var layout = new Layout();
            layout.AddTrack(5, 5);

            int expected = 0;
            if (left)
            {
                expected++;
                layout.AddTrack(4, 5);
            }
            if (up)
            {
                expected++;
                layout.AddTrack(5, 4);
            }
            if (right)
            {
                expected++;
                layout.AddTrack(6, 5);
            }
            if (down)
            {
                expected++;
                layout.AddTrack(5, 6);
            }

            var track = layout.GetTrackAt<SingleTrack>(5, 5);
            var neighbors = track.GetAllNeighbors();

            Assert.Equal(expected, neighbors.Count);

            Assert.Equal(left, neighbors.Left != null);
            Assert.Equal(up, neighbors.Up != null);
            Assert.Equal(right, neighbors.Right != null);
            Assert.Equal(down, neighbors.Down != null);
        }
    }
}
