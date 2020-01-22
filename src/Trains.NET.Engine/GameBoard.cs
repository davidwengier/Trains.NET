using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Trains.NET.Engine
{
    internal class GameBoard : IGameBoard, IDisposable
    {
        private const int GameLoopInterval = 500;
        private const int SpeedAdjustmentFactor = 1;

        private readonly Dictionary<(int, int), Track> _tracks = new Dictionary<(int, int), Track>();
        private readonly List<Train> _trains = new List<Train>();
        private readonly Timer _gameLoopTimer;

        public int Columns { get; set; }
        public int Rows { get; set; }

        public GameBoard()
        {
            _gameLoopTimer = new Timer(GameLoopInterval);
            _gameLoopTimer.Elapsed += GameLoopStep;
            _gameLoopTimer.Start();
        }

        private void GameLoopStep(object sender, ElapsedEventArgs e)
        {
            foreach (Train train in _trains)
            {
                if (_tracks.TryGetValue((train.Column, train.Row), out Track track))
                {
                    train.Move(SpeedAdjustmentFactor, track);
                }
            }
        }

        public void AddTrack(int column, int row)
        {
            if (_tracks.TryGetValue((column, row), out Track track))
            {
                track.SetBestTrackDirection(true);
            }
            else
            {
                track = new Track(this)
                {
                    Column = column,
                    Row = row
                };
                _tracks.Add((column, row), track);

                track.SetBestTrackDirection(false);
            }
        }

        public void RemoveTrack(int column, int row)
        {
            if (_tracks.TryGetValue((column, row), out Track track))
            {
                _tracks.Remove((column, row));
                track.RefreshNeighbors(true);
            }
        }

        public void AddTrain(int column, int row)
        {
            if (_tracks.ContainsKey((column, row)) && !_trains.Any(t => t.Column == column && t.Row == row))
            {
                _trains.Add(new Train()
                {
                    Column = column,
                    Row = row
                });
            }
        }

        public IEnumerable<(int, int, Track)> GetTracks()
        {
            foreach ((int col, int row, Track track) in _tracks)
            {
                yield return (col, row, track);
            }
        }

        public IEnumerable<Train> GetTrains()
        {
            foreach (Train train in _trains)
            {
                yield return train;
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

        public void Dispose()
        {
            _gameLoopTimer.Dispose();
        }
    }
}
