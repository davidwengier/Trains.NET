using System;
using System.Collections.Generic;
using System.Text;

namespace Trains.NET.Engine
{
    public class GameBoard : IGameBoard
    {
        private readonly Dictionary<(int, int), Track> _tracks = new Dictionary<(int, int), Track>();

        public int Columns { get; set; }
        public int Rows { get; set; }

        public void AddTrack(int column, int row)
        {
            if (!_tracks.ContainsKey((column, row)))
            {
                _tracks.Add((column, row), new Track());
            }
        }

        public IEnumerable<(int, int, Track)> GetTracks()
        {
            foreach ((int col, int row, Track track) in _tracks)
            {
                yield return (col, row, track);
            }
        }
    }
}
