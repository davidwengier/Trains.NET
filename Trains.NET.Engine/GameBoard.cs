using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Trains.NET.Engine
{
    internal class GameBoard : IGameBoard
    {
        private readonly Dictionary<(int, int), Track> _tracks = new Dictionary<(int, int), Track>();

        public int Columns { get; set; }
        public int Rows { get; set; }

        public void AddTrack(int column, int row)
        {
            if (!_tracks.ContainsKey((column, row)))
            {
                var track = new Track();
                _tracks.Add((column, row), track);

                SetBestTrackDirection(track, column, row);
            }
        }

        private void SetBestTrackDirection(Track track, int column, int row)
        {
            var neighbors = GetNeighbors(column, row);

            TrackDirection newDirection;
            if (neighbors.Up != null || neighbors.Down != null)
            {
                if (neighbors.Left != null)
                {
                    newDirection = neighbors.Up == null ? TrackDirection.LeftDown : TrackDirection.LeftUp;
                }
                else if (neighbors.Right != null)
                {
                    newDirection = neighbors.Up == null ? TrackDirection.RightDown : TrackDirection.RightUp;
                }
                else
                {
                    newDirection = TrackDirection.Vertical;
                }
            }
            else if (neighbors.Left != null || neighbors.Right != null)
            {
                if (neighbors.Up != null)
                {
                    newDirection = neighbors.Left == null ? TrackDirection.RightUp : TrackDirection.LeftUp;
                }
                else if (neighbors.Down != null)
                {
                    newDirection = neighbors.Left == null ? TrackDirection.RightDown : TrackDirection.LeftDown;
                }
                else
                {
                    newDirection = TrackDirection.Horizontal;
                }
            }
            else
            {
                newDirection = TrackDirection.Horizontal;
            }

            if (newDirection != track.Direction)
            {
                track.Direction = newDirection;
                foreach ((int c, int r) in GetNeighborPositions(column, row))
                {
                    var nextTrack = GetTrackAt(c, r);
                    if (nextTrack != null)
                    {
                        SetBestTrackDirection(nextTrack, c, r);
                    }
                }
            }
        }

        public IEnumerable<(int, int, Track)> GetTracks()
        {
            foreach ((int col, int row, Track track) in _tracks)
            {
                yield return (col, row, track);
            }
        }

        public Track? GetTrackAt(int column, int row)
        {
            if (_tracks.TryGetValue((column, row), out Track track))
            {
                return track;
            }
            return null;
        }

        public (Track? Left, Track? Up, Track? Right, Track? Down) GetNeighbors(int column, int row)
        {
            return (
                GetTrackAt(column - 1, row),
                GetTrackAt(column, row - 1),
                GetTrackAt(column + 1, row),
                GetTrackAt(column, row + 1)
               );
        }

        public IEnumerable<(int col, int row)> GetNeighborPositions(int column, int row)
        {
            yield return (column - 1, row);
            yield return (column, row - 1);
            yield return (column + 1, row);
            yield return (column, row + 1);
        }
    }
}
