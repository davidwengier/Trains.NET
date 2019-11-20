using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trains.NET.Engine;
using Xunit;

namespace Trains.NET.Tests
{
    public class GameBoardTests
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        public void SetBestTrackDirection(TrackDirection expectedDirection, params (int,int)[] tracks)
        {
            var board = new GameBoard();
            foreach (var track in tracks)
            {
                board.AddTrack(track.Item1, track.Item2);
            }

            Track? lastTrack = board.GetTrackAt(tracks.Last().Item1, tracks.Last().Item2);
            Assert.NotNull(lastTrack);
            Assert.Equal(expectedDirection, lastTrack!.Direction);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[] {
                TrackDirection.Vertical,
                (1, 1),
                (1, 2),
                };
            yield return new object[] {
                TrackDirection.Horizontal,
                (1, 1),
                (2, 1),
                };
            yield return new object[] {
                TrackDirection.Horizontal,
                (1, 1),
                (2, 1),
                (1, 2),
                };
            yield return new object[] {
                TrackDirection.Vertical,
                (1, 2),
                (1, 1),
                };
        }
    }
}
