using Trains.NET.Engine;
using Xunit;
using Xunit.Abstractions;

#nullable disable

namespace Trains.NET.Tests
{
    public class TrackLayoutTests : TestBase
    {
        public TrackLayoutTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void FishHook()
        {
            GameBoard.AddTrack(5, 2);
            GameBoard.AddTrack(4, 2);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(3, 3);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(1, 3);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(3, 1);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(5, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(4, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, GameBoard.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, GameBoard.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightUp, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 1).Direction);
        }

        [Fact]
        public void VerticalHairpin()
        {
            GameBoard.AddTrack(1, 3);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void SidewaysHairpin()
        {
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(3, 1);
            GameBoard.AddTrack(2, 1);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, GameBoard.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, GameBoard.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void RightAngleWithCircleOnTop()
        {
            GameBoard.AddTrack(1, 4);
            GameBoard.AddTrack(1, 3);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.RightDown, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftUp, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightDown, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.LeftUpDown, GameBoard.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftAngleWithCircleOnTop()
        {
            GameBoard.AddTrack(1, 3);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(2, 4);
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(1, 2);

            Assert.Equal(TrackDirection.RightUp, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftDown, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 4).Direction);
            Assert.Equal(TrackDirection.RightDown, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.LeftUp, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.RightUpDown, GameBoard.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void CrossVerticalFirst()
        {
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(3, 2);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void CrossHortizontalFirst()
        {
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 3);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void CrossMiddleLast()
        {
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void TwoCrosses()
        {
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(2, 1);

            GameBoard.AddTrack(1, 4);
            GameBoard.AddTrack(2, 4);
            GameBoard.AddTrack(3, 4);
            GameBoard.AddTrack(2, 5);

            GameBoard.AddTrack(2, 3);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 2).Direction);

            Assert.Equal(TrackDirection.Cross, GameBoard.GetTrackAt(2, 4).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 4).Direction);

            Assert.True(GameBoard.GetTrackAt(2, 3).Happy);
        }

        [Fact]
        public void Horizontal()
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void Vertical()
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void Two_Verticals()
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(1, 3);
            GameBoard.AddTrack(1, 4);

            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(2, 3);


            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 4).Direction);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);
        }

        [Fact]
        public void Three_Verticals()
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(1, 3);
            GameBoard.AddTrack(1, 4);
            GameBoard.AddTrack(1, 5);

            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(2, 4);

            GameBoard.AddTrack(3, 3);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 5).Direction);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 4).Direction);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 3).Direction);
        }

        [Fact]
        public void LeftUp()
        {
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.LeftUp, GameBoard.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftUpDown()
        {
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.LeftUpDown, GameBoard.GetTrackAt(2, 2).Direction);
        }


        [Fact]
        public void RightUpDown()
        {
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightUpDown, GameBoard.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightUp()
        {
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftRightUp, GameBoard.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightDown()
        {
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftRightDown, GameBoard.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftDown()
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, GameBoard.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void RightUp()
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, GameBoard.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void RightDown()
        {
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, GameBoard.GetTrackAt(1, 1).Direction);
        }

        [Fact]
        public void RightUpDown_DrawOver()
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(1, 3);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 2).Direction);

            GameBoard.AddTrack(1, 2);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUpDown, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 2).Direction);
        }


        [Fact]
        public void LeftUpDown_DrawOver()
        {
            GameBoard.AddTrack(3, 1);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(3, 3);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 2).Direction);

            GameBoard.AddTrack(3, 2);

            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.LeftUpDown, GameBoard.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightDown_DrawOver()
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(2, 1);
            GameBoard.AddTrack(3, 1);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 2).Direction);

            GameBoard.AddTrack(2, 1);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftRightDown, GameBoard.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightUp_DrawOver()
        {
            GameBoard.AddTrack(1, 3);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(3, 3);
            GameBoard.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(2, 2).Direction);

            GameBoard.AddTrack(2, 3);

            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftRightUp, GameBoard.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, GameBoard.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void Happiness()
        {
            GameBoard.AddTrack(1, 1);

            Assert.False(GameBoard.GetTrackAt(1, 1).Happy);

            GameBoard.AddTrack(1, 2);

            Assert.False(GameBoard.GetTrackAt(1, 1).Happy);
            Assert.False(GameBoard.GetTrackAt(1, 2).Happy);

            GameBoard.AddTrack(1, 3);

            Assert.False(GameBoard.GetTrackAt(1, 1).Happy);
            Assert.True(GameBoard.GetTrackAt(1, 2).Happy);
            Assert.False(GameBoard.GetTrackAt(1, 3).Happy);
        }

        [Fact]
        public void Trident()
        {
            GameBoard.AddTrack(1, 1);
            GameBoard.AddTrack(1, 2);
            GameBoard.AddTrack(2, 2);
            GameBoard.AddTrack(3, 2);
            GameBoard.AddTrack(3, 1);
            GameBoard.AddTrack(2, 3);
            GameBoard.AddTrack(2, 2);

            GameBoard.AddTrack(2, 1);

            Assert.Equal(TrackDirection.RightDown, GameBoard.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, GameBoard.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Cross, GameBoard.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, GameBoard.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, GameBoard.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, GameBoard.GetTrackAt(2, 3).Direction);

            Assert.Equal(TrackDirection.LeftRightDown, GameBoard.GetTrackAt(2, 1).Direction);
        }
    }
}
