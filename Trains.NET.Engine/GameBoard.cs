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
                var track = new Track(this);
                track.Column = column;
                track.Row = row;
                _tracks.Add((column, row), track);

                track.SetBestTrackDirection();
            }
        }
        public void RemoveTrack(int column, int row)
        {
            if (_tracks.TryGetValue((column, row), out Track track))
            {
                _tracks.Remove((column, row));
                track.RefreshNeighbors(false);
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
