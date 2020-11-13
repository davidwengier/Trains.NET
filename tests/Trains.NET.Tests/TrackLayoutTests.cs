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
            TrackTool.Execute(5, 2, true);
            TrackTool.Execute(4, 2, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(3, 3, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(3, 1, true);

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
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void SidewaysHairpin()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(3, 1, true);
            TrackTool.Execute(2, 1, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void RightAngleWithCircleOnTop()
        {
            TrackTool.Execute(1, 4, true);
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);

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
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 4, true);
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(1, 2, true);

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
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 2, true);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.IsType<CrossTrack>(TrackLayout.GetTrackAt(2, 2));
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void CrossHortizontalFirst()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(2, 3, true);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.IsType<CrossTrack>(TrackLayout.GetTrackAt(2, 2));
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void CrossMiddleLast()
        {
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 3, true);
            TrackTool.Execute(2, 4, true);
            TrackTool.Execute(2, 5, true);

            TrackTool.Execute(2, 3, false);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.IsType<CrossTrack>(TrackLayout.GetTrackAt(2, 3));
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 4).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 3).Direction);
        }

        [Fact]
        public void TwoCrosses()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 2, true);

            TrackTool.Execute(1, 4, true);
            TrackTool.Execute(2, 4, true);
            TrackTool.Execute(3, 4, true);

            TrackTool.Execute(2, 5, true);
            TrackTool.Execute(2, 4, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(2, 1, true);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.IsType<CrossTrack>(TrackLayout.GetTrackAt(2, 2));
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);

            Assert.IsType<CrossTrack>(TrackLayout.GetTrackAt(2, 4));
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 4).Direction);

            Assert.True(TrackLayout.GetTrackAt(2, 3).Happy);
        }

        [Fact]
        public void Horizontal()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void Vertical()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void Two_Verticals()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(1, 4, true);

            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(2, 3, true);


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
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(1, 4, true);
            TrackTool.Execute(1, 5, true);

            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 4, true);

            TrackTool.Execute(3, 3, true);

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
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.LeftUp, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftUpDown()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt(2, 2).Direction);
        }


        [Fact]
        public void RightUpDown()
        {
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightUp_RightDown, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightUp()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp_RightUp, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightDown()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.RightDown_LeftDown, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftDown()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void RightUp()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, TrackLayout.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void RightDown()
        {
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 1).Direction);
        }

        [Fact]
        public void RightUpDown_DrawOver()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);

            TrackTool.Execute(1, 2, true);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp_RightDown, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);
        }


        [Fact]
        public void LeftUpDown_DrawOver()
        {
            TrackTool.Execute(3, 1, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(3, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);

            TrackTool.Execute(3, 2, true);

            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightDown_DrawOver()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(3, 1, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);

            TrackTool.Execute(2, 1, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightDown_LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightUp_DrawOver()
        {
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(3, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(2, 2).Direction);

            TrackTool.Execute(2, 3, true);

            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftUp_RightUp, TrackLayout.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, TrackLayout.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void Happiness()
        {
            TrackTool.Execute(1, 1, true);

            Assert.False(TrackLayout.GetTrackAt(1, 1).Happy);

            TrackTool.Execute(1, 2, true);

            Assert.False(TrackLayout.GetTrackAt(1, 1).Happy);
            Assert.False(TrackLayout.GetTrackAt(1, 2).Happy);

            TrackTool.Execute(1, 3, true);

            Assert.False(TrackLayout.GetTrackAt(1, 1).Happy);
            Assert.True(TrackLayout.GetTrackAt(1, 2).Happy);
            Assert.False(TrackLayout.GetTrackAt(1, 3).Happy);
        }

        [Fact]
        public void Trident()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(3, 1, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 2, true);

            TrackTool.Execute(2, 1, true);

            Assert.Equal(TrackDirection.RightDown, TrackLayout.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, TrackLayout.GetTrackAt(1, 2).Direction);
            Assert.IsType<CrossTrack>(TrackLayout.GetTrackAt(2, 2));
            Assert.Equal(TrackDirection.LeftUp, TrackLayout.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, TrackLayout.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, TrackLayout.GetTrackAt(2, 3).Direction);

            Assert.Equal(TrackDirection.RightDown_LeftDown, TrackLayout.GetTrackAt(2, 1).Direction);
        }
    }
}
