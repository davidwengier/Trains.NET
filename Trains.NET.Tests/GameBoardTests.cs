using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests
{
    public class GameBoardTests
    {
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
            Assert.Equal(TrackDirection.Horizontal, board.GetTrackAt(2, 3).Direction);
            Assert.Equal(TrackDirection.RightDown, board.GetTrackAt(1, 1).Direction);
            Assert.Equal(TrackDirection.LeftDown, board.GetTrackAt(2, 1).Direction);
            Assert.Equal(TrackDirection.RightUp, board.GetTrackAt(1, 2).Direction);
            Assert.Equal(TrackDirection.LeftUp, board.GetTrackAt(2, 2).Direction);
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
    }
}
