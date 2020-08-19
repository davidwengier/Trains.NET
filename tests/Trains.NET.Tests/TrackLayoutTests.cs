using Trains.NET.Engine;
using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests
{
    public class TrackLayoutTests : TestBase
    {
        public TrackLayoutTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void FishHook()
        {
            TrackLayout.AddTrack(5, 2);
            TrackLayout.AddTrack(4, 2);
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(3, 3);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(1, 3);
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(3, 1);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(5, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(4, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, TrackLayout.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightUp, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 1).Direction);
        }

        [Fact]
        public void VerticalHairpin()
        {
            TrackLayout.AddTrack(1, 3);
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void SidewaysHairpin()
        {
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 2);
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(3, 1);
            TrackLayout.AddTrack(2, 1);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void RightAngleWithCircleOnTop()
        {
            TrackLayout.AddTrack(1, 4);
            TrackLayout.AddTrack(1, 3);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftUp, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftAngleWithCircleOnTop()
        {
            TrackLayout.AddTrack(1, 3);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(2, 4);
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(2, 2);
            TrackLayout.AddTrack(1, 2);

            Assert.Equal(TrackDirection.RightUp, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 4).Direction);
            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.LeftUp, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.RightUp_RightDown, TrackLayout.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void CrossVerticalFirst()
        {
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(2, 2);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(3, 2);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void CrossHortizontalFirst()
        {
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 2);
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(2, 3);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void CrossMiddleLast()
        {
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void TwoCrosses()
        {
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 2);
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(2, 1);

            TrackLayout.AddTrack(1, 4);
            TrackLayout.AddTrack(2, 4);
            TrackLayout.AddTrack(3, 4);
            TrackLayout.AddTrack(2, 5);

            TrackLayout.AddTrack(2, 3);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);

            Assert.Equal(TrackDirection.Cross, TrackLayout.GetTrackAt(2, 4).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 4).Direction);

            Assert.True(TrackLayout.GetTrackAt(2, 3).Happy);
        }

        [Fact]
        public void Horizontal()
        {
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(2, 1);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void Vertical()
        {
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(1, 2);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void Two_Verticals()
        {
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(1, 3);
            TrackLayout.AddTrack(1, 4);

            TrackLayout.AddTrack(2, 2);
            TrackLayout.AddTrack(2, 3);


            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 4).Direction);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
        }

        [Fact]
        public void Three_Verticals()
        {
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(1, 3);
            TrackLayout.AddTrack(1, 4);
            TrackLayout.AddTrack(1, 5);

            TrackLayout.AddTrack(2, 2);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(2, 4);

            TrackLayout.AddTrack(3, 3);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 5).Direction);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 4).Direction);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 3).Direction);
        }

        [Fact]
        public void LeftUp()
        {
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.LeftUp, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftUpDown()
        {
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt(2, 2).Direction);
        }


        [Fact]
        public void RightUpDown()
        {
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightUp_RightDown, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightUp()
        {
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp_RightUp, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightDown()
        {
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.RightDown_LeftDown, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftDown()
        {
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void RightUp()
        {
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, TrackLayout.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void RightDown()
        {
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(1, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 1).Direction);
        }

        [Fact]
        public void RightUpDown_DrawOver()
        {
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(1, 3);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);

            TrackLayout.AddTrack(1, 2);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp_RightDown, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);
        }


        [Fact]
        public void LeftUpDown_DrawOver()
        {
            TrackLayout.AddTrack(3, 1);
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(3, 3);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);

            TrackLayout.AddTrack(3, 2);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightDown_DrawOver()
        {
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(2, 1);
            TrackLayout.AddTrack(3, 1);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);

            TrackLayout.AddTrack(2, 1);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightDown_LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightUp_DrawOver()
        {
            TrackLayout.AddTrack(1, 3);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(3, 3);
            TrackLayout.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);

            TrackLayout.AddTrack(2, 3);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftUp_RightUp, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void Happiness()
        {
            TrackLayout.AddTrack(1, 1);

            Assert.False(TrackLayout.GetTrackAt(1, 1).Happy);

            TrackLayout.AddTrack(1, 2);

            Assert.False(TrackLayout.GetTrackAt(1, 1).Happy);
            Assert.False(TrackLayout.GetTrackAt(1, 2).Happy);

            TrackLayout.AddTrack(1, 3);

            Assert.False(TrackLayout.GetTrackAt(1, 1).Happy);
            Assert.True(TrackLayout.GetTrackAt(1, 2).Happy);
            Assert.False(TrackLayout.GetTrackAt(1, 3).Happy);
        }

        [Fact]
        public void Trident()
        {
            TrackLayout.AddTrack(1, 1);
            TrackLayout.AddTrack(1, 2);
            TrackLayout.AddTrack(2, 2);
            TrackLayout.AddTrack(3, 2);
            TrackLayout.AddTrack(3, 1);
            TrackLayout.AddTrack(2, 3);
            TrackLayout.AddTrack(2, 2);

            TrackLayout.AddTrack(2, 1);

            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Cross, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);

            Assert.Equal(TrackDirection.RightDown_LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
        }
    }
}
