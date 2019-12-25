using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests
{
    public class GameBoardTests
    {
        [Fact]
        public void FishHook()
        {
            var board = new GameBoard();
            board.AddTrack(5, 2);
            board.AddTrack(4, 2);
            board.AddTrack(3, 2);
            board.AddTrack(3, 3);
            board.AddTrack(2, 3);
            board.AddTrack(1, 3);
            board.AddTrack(1, 2);
            board.AddTrack(1, 1);
            board.AddTrack(2, 1);
            board.AddTrack(3, 1);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(5, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(4, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, board.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, board.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightUp, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 1).Direction);
        }

        [Fact]
        public void VerticalHairpin()
        {
            var board = new GameBoard();
            board.AddTrack(1, 3);
            board.AddTrack(1, 2);
            board.AddTrack(1, 1);
            board.AddTrack(2, 1);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void SidewaysHairpin()
        {
            var board = new GameBoard();
            board.AddTrack(1, 2);
            board.AddTrack(2, 2);
            board.AddTrack(3, 2);
            board.AddTrack(3, 1);
            board.AddTrack(2, 1);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, board.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, board.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void RightAngleWithCircleOnTop()
        {
            var board = new GameBoard();
            board.AddTrack(1, 4);
            board.AddTrack(1, 3);
            board.AddTrack(2, 3);
            board.AddTrack(1, 1);
            board.AddTrack(2, 1);
            board.AddTrack(1, 2);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.RightDown, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftUp, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightDown, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.LeftUpDown, board.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftAngleWithCircleOnTop()
        {
            var board = new GameBoard();
            board.AddTrack(1, 3);
            board.AddTrack(2, 3);
            board.AddTrack(2, 4);
            board.AddTrack(1, 1);
            board.AddTrack(2, 1);
            board.AddTrack(2, 2);
            board.AddTrack(1, 2);

            Assert.Equal(TrackDirection.RightUp, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftDown, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 4).Direction);
            Assert.Equal(TrackDirection.RightDown, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.LeftUp, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.RightUpDown, board.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void CrossVerticalFirst()
        {
            var board = new GameBoard();
            board.AddTrack(2, 1);
            board.AddTrack(2, 2);
            board.AddTrack(2, 3);
            board.AddTrack(1, 2);
            board.AddTrack(3, 2);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void CrossHortizontalFirst()
        {
            var board = new GameBoard();
            board.AddTrack(1, 2);
            board.AddTrack(2, 2);
            board.AddTrack(3, 2);
            board.AddTrack(2, 1);
            board.AddTrack(2, 3);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void CrossMiddleLast()
        {
            var board = new GameBoard();
            board.AddTrack(1, 2);
            board.AddTrack(3, 2);
            board.AddTrack(2, 1);
            board.AddTrack(2, 3);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 2).Direction);
        }

        [Fact]
        public void TwoCrosses()
        {
            var board = new GameBoard();
            board.AddTrack(1, 2);
            board.AddTrack(2, 2);
            board.AddTrack(3, 2);
            board.AddTrack(2, 1);

            board.AddTrack(1, 4);
            board.AddTrack(2, 4);
            board.AddTrack(3, 4);
            board.AddTrack(2, 5);

            board.AddTrack(2, 3);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Cross, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 2).Direction);

            Assert.Equal(TrackDirection.Cross, board.GetTrackAt(2, 4).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 4).Direction);

            Assert.True(board.GetTrackAt(2, 3).Happy);
        }

        [Fact]
        public void Horizontal()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);
            board.AddTrack(2, 1);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void Vertical()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);
            board.AddTrack(1, 2);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void Two_Verticals()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);
            board.AddTrack(1, 2);
            board.AddTrack(1, 3);
            board.AddTrack(1, 4);

            board.AddTrack(2, 2);
            board.AddTrack(2, 3);


            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 4).Direction);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);
        }

        [Fact]
        public void Three_Verticals()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);
            board.AddTrack(1, 2);
            board.AddTrack(1, 3);
            board.AddTrack(1, 4);
            board.AddTrack(1, 5);

            board.AddTrack(2, 2);
            board.AddTrack(2, 3);
            board.AddTrack(2, 4);

            board.AddTrack(3, 3);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 4).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 5).Direction);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 4).Direction);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 3).Direction);
        }

        [Fact]
        public void LeftUp()
        {
            var board = new GameBoard();
            board.AddTrack(1, 2);
            board.AddTrack(2, 1);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.LeftUp, board.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftUpDown()
        {
            var board = new GameBoard();
            board.AddTrack(1, 2);
            board.AddTrack(2, 1);
            board.AddTrack(2, 3);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.LeftUpDown, board.GetTrackAt(2, 2).Direction);
        }


        [Fact]
        public void RightUpDown()
        {
            var board = new GameBoard();
            board.AddTrack(3, 2);
            board.AddTrack(2, 1);
            board.AddTrack(2, 3);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightUpDown, board.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightUp()
        {
            var board = new GameBoard();
            board.AddTrack(1, 2);
            board.AddTrack(2, 1);
            board.AddTrack(3, 2);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftRightUp, board.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightDown()
        {
            var board = new GameBoard();
            board.AddTrack(1, 2);
            board.AddTrack(2, 3);
            board.AddTrack(3, 2);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftRightDown, board.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftDown()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);
            board.AddTrack(2, 1);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, board.GetTrackAt(2, 1).Direction);
        }

        [Fact]
        public void RightUp()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);
            board.AddTrack(1, 2);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, board.GetTrackAt(1, 2).Direction);
        }

        [Fact]
        public void RightDown()
        {
            var board = new GameBoard();
            board.AddTrack(2, 1);
            board.AddTrack(1, 1);
            board.AddTrack(1, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.RightDown, board.GetTrackAt(1, 1).Direction);
        }

        [Fact]
        public void RightUpDown_DrawOver()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);
            board.AddTrack(1, 2);
            board.AddTrack(1, 3);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 2).Direction);

            board.AddTrack(1, 2);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUpDown, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 2).Direction);
        }


        [Fact]
        public void LeftUpDown_DrawOver()
        {
            var board = new GameBoard();
            board.AddTrack(3, 1);
            board.AddTrack(3, 2);
            board.AddTrack(3, 3);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 2).Direction);

            board.AddTrack(3, 2);

            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.LeftUpDown, board.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightDown_DrawOver()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);
            board.AddTrack(2, 1);
            board.AddTrack(3, 1);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 2).Direction);

            board.AddTrack(2, 1);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftRightDown, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void LeftRightUp_DrawOver()
        {
            var board = new GameBoard();
            board.AddTrack(1, 3);
            board.AddTrack(2, 3);
            board.AddTrack(3, 3);
            board.AddTrack(2, 2);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 2).Direction);

            board.AddTrack(2, 3);

            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(1, 3).Direction);
            Assert.Equal(TrackDirection.LeftRightUp, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(3, 3).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 2).Direction);
        }

        [Fact]
        public void Happiness()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);

            Assert.False(board.GetTrackAt(1, 1).Happy);

            board.AddTrack(1, 2);

            Assert.False(board.GetTrackAt(1, 1).Happy);
            Assert.False(board.GetTrackAt(1, 2).Happy);

            board.AddTrack(1, 3);

            Assert.False(board.GetTrackAt(1, 1).Happy);
            Assert.True(board.GetTrackAt(1, 2).Happy);
            Assert.False(board.GetTrackAt(1, 3).Happy);
        }

        [Fact]
        public void Trident()
        {
            var board = new GameBoard();
            board.AddTrack(1, 1);
            board.AddTrack(1, 2);
            board.AddTrack(2, 2);
            board.AddTrack(3, 2);
            board.AddTrack(3, 1);
            board.AddTrack(2, 3);
            board.AddTrack(2, 2);

            board.AddTrack(2, 1);

            Assert.Equal(TrackDirection.RightDown, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.Cross, board.GetTrackAt(2, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, board.GetTrackAt(3, 2).Direction);
            Assert.Equal(TrackDirection.LeftDown, board.GetTrackAt(3, 1).Direction);
            Assert.Equal(TrackDirection.Vertical, board.GetTrackAt(2, 3).Direction);

            Assert.Equal(TrackDirection.LeftRightDown, board.GetTrackAt(2, 1).Direction);
        }
    }
}
