using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Trains.NET.Engine
{
    internal class GameBoard : IGameBoard, IDisposable
    {
        private const int GameLoopInterval = 16;
        private const int SpeedAdjustmentFactor = 10;

        private readonly Dictionary<(int, int), Track> _tracks = new Dictionary<(int, int), Track>();
        private readonly List<IMovable> _movables = new List<IMovable>();
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
            _gameLoopTimer.Stop();
            foreach (Train train in _movables)
            {
                float distance = 0.005f * SpeedAdjustmentFactor;
                while (distance > 0.0f)
                {
                    Track? track = GetTrackForTrain(train);
                    if (track != null)
                    {
                        distance = train.Move(distance, track);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            _gameLoopTimer.Start();
        }

        public Track? GetTrackForTrain(Train train)
        {
            if (_tracks.TryGetValue((train.Column, train.Row), out Track track))
            {
                return track;
            }
            return null;
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
            var train = new Train()
            {
                Column = column,
                Row = row
            };

            Track? track = GetTrackForTrain(train);
            if (track == null)
            {
                return;
            }

            _movables.Add(train);
        }

        public IEnumerable<(int, int, Track)> GetTracks()
        {
            foreach ((int col, int row, Track track) in _tracks)
            {
                yield return (col, row, track);
            }
        }

        public IEnumerable<IMovable> GetMovables()
        {
            foreach (IMovable movable in _movables)
            {
                yield return movable;
            }
        }

        public IEnumerable<T> GetMovables<T>() where T : IMovable
        {
            foreach (T movable in _movables.OfType<T>())
            {
                yield return movable;
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
