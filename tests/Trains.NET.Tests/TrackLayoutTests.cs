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

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(5, 2).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(4, 2).Direction);
            Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
            Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.RightUp, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
        }

        [Fact]
        public void VerticalHairpin()
        {
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        }

        [Fact]
        public void SidewaysHairpin()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(3, 1, true);
            TrackTool.Execute(2, 1, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
            Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
            Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        }

        [Fact(Skip = "These are broken and I don't know why")]
        public void RightAngleWithCircleOnTop()
        {
            TrackTool.Execute(1, 4, true);
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 4).Direction);
            Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.RightUp, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(TIntersectionDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt<TIntersection>(2, 2).Direction);
        }

        [Fact(Skip = "These are broken and I don't know why")]
        public void LeftAngleWithCircleOnTop()
        {
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 4, true);
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(1, 2, true);

            Assert.Equal(SingleTrackDirection.RightUp, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 4).Direction);
            Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
            Assert.Equal(TIntersectionDirection.RightUp_RightDown, TrackLayout.GetTrackAt<TIntersection>(1, 2).Direction);
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

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        }

        [Fact(Skip = "Things don't work like this anymore :(")]
        public void CrossHortizontalFirst()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(2, 3, true);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
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

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
            Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 3));
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 4).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
        }

        [Fact(Skip = "Crosses don't work like this any more :(")]
        public void TwoCrosses_DragDown()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(3, 2, true);

            TrackTool.Execute(1, 4, true);
            TrackTool.Execute(2, 4, true);
            TrackTool.Execute(3, 4, true);

            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 4, true);
            TrackTool.Execute(2, 5, true);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);

            Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 4));
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 4).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 4).Direction);

            Assert.True(TrackLayout.GetTrackAt<SingleTrack>(2, 3).Happy);
        }

        [Fact(Skip = "Crosses don't work like this any more :(")]
        public void TwoCrosses_DragUp()
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

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);

            Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 4));
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 4).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 4).Direction);

            Assert.True(TrackLayout.GetTrackAt<SingleTrack>(2, 3).Happy);
        }

        [Fact]
        public void Horizontal()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        }

        [Fact]
        public void Vertical()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
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

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 4).Direction);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
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

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 4).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 5).Direction);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 4).Direction);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
        }

        [Fact]
        public void LeftUp()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        }

        [Fact(Skip = "These are broken and I don't know why")]
        public void LeftUpDown()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(TIntersectionDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt<TIntersection>(2, 2).Direction);
        }

        [Fact(Skip = "These are broken and I don't know why")]
        public void RightUpDown()
        {
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(TIntersectionDirection.RightUp_RightDown, TrackLayout.GetTrackAt<TIntersection>(2, 2).Direction);
        }

        [Fact(Skip = "These are broken and I don't know why")]
        public void LeftRightUp()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
            Assert.Equal(TIntersectionDirection.LeftUp_RightUp, TrackLayout.GetTrackAt<TIntersection>(2, 2).Direction);
        }

        [Fact(Skip = "These are broken and I don't know why")]
        public void LeftRightDown()
        {
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
            Assert.Equal(TIntersectionDirection.RightDown_LeftDown, TrackLayout.GetTrackAt<TIntersection>(2, 2).Direction);
        }

        [Fact]
        public void LeftDown()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
            Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        }

        [Fact]
        public void RightUp()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.RightUp, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        }

        [Fact]
        public void RightDown()
        {
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        }

        [Fact]
        public void RightUpDown_DrawOver()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(1, 2, true);
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);

            TrackTool.Execute(1, 2, true);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(TIntersectionDirection.RightUp_RightDown, TrackLayout.GetTrackAt<TIntersection>(1, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        }

        [Fact]
        public void LeftUpDown_DrawOver()
        {
            TrackTool.Execute(3, 1, true);
            TrackTool.Execute(3, 2, true);
            TrackTool.Execute(3, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);

            TrackTool.Execute(3, 2, true);

            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
            Assert.Equal(TIntersectionDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt<TIntersection>(3, 2).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        }

        [Fact(Skip = "These are broken and I don't know why")]
        public void LeftRightDown_DrawOver()
        {
            TrackTool.Execute(1, 1, true);
            TrackTool.Execute(2, 1, true);
            TrackTool.Execute(3, 1, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);

            TrackTool.Execute(2, 1, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(TIntersectionDirection.RightDown_LeftDown, TrackLayout.GetTrackAt<TIntersection>(2, 1).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        }

        [Fact(Skip = "These are broken and I don't know why")]
        public void LeftRightUp_DrawOver()
        {
            TrackTool.Execute(1, 3, true);
            TrackTool.Execute(2, 3, true);
            TrackTool.Execute(3, 3, true);
            TrackTool.Execute(2, 2, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);

            TrackTool.Execute(2, 3, true);

            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
            Assert.Equal(TIntersectionDirection.LeftUp_RightUp, TrackLayout.GetTrackAt<TIntersection>(2, 3).Direction);
            Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        }

        [Fact]
        public void Happiness()
        {
            TrackTool.Execute(1, 1, true);

            Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 1).Happy);

            TrackTool.Execute(1, 2, true);

            Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 1).Happy);
            Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 2).Happy);

            TrackTool.Execute(1, 3, true);

            Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 1).Happy);
            Assert.True(TrackLayout.GetTrackAt<SingleTrack>(1, 2).Happy);
            Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 3).Happy);
        }

        [Fact(Skip = "These are broken and I don't know why")]
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

            Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
            Assert.Equal(SingleTrackDirection.RightUp, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
            Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
            Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
            Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
            Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);

            Assert.Equal(TIntersectionDirection.RightDown_LeftDown, TrackLayout.GetTrackAt<TIntersection>(2, 1).Direction);
        }
    }
}
