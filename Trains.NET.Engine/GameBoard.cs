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
            if (_tracks.ContainsKey((column, row - 1)))
            {
                if (GetTrackAt(column, row - 1)?.Direction != TrackDirection.Horizontal ||
                    !(_tracks.ContainsKey((column - 1, row - 1)) ||
                    _tracks.ContainsKey((column + 1, row - 1))))
                {
                    track.Direction = TrackDirection.Vertical;
                }
            }
            else if (_tracks.ContainsKey((column, row + 1)))
            {
                track.Direction = TrackDirection.Vertical;
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
    }
}
