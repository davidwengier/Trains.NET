using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Engine
{
    internal class GameBoard : IGameBoard, IDisposable
    {
        private const int GameLoopInterval = 16;

        private readonly Dictionary<(int, int), Track> _tracks = new Dictionary<(int, int), Track>();
        private readonly List<IMovable> _movables = new List<IMovable>();
        private readonly ITimer? _gameLoopTimer;
        private readonly IGameStorage? _storage;

        public int Columns { get; set; }
        public int Rows { get; set; }
        public int SpeedAdjustmentFactor { get; set; } = 10;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public GameBoard(IGameStorage? storage, ITimer? timer)
        {
            _gameLoopTimer = timer;
            if (_gameLoopTimer != null)
            {
                _gameLoopTimer.Interval = GameLoopInterval;
                _gameLoopTimer.Elapsed += GameLoopStep;
                _gameLoopTimer.Start();
            }

            _storage = storage;

            if (storage == null)
            {
                return;
            }

            IEnumerable<Track>? tracks = null;
            try
            {
                tracks = storage.ReadTracks();
            }
            catch { }

            if (tracks != null)
            {
                LoadTracks(tracks);
            }
        }

        public void LoadTracks(IEnumerable<Track> tracks)
        {
            ClearAll();

            foreach (Track track in tracks)
            {
                _tracks[(track.Column, track.Row)] = new Track(this)
                {
                    Column = track.Column,
                    Row = track.Row,
                    Direction = track.Direction,
                };
            }

            foreach (Track track in _tracks.Values)
            {
                track.ReevaluateHappiness();
            }

            _storage?.WriteTracks(_tracks.Values);
        }

        private void GameLoopStep(object sender, EventArgs e)
        {
            _gameLoopTimer?.Stop();
            foreach (Train train in _movables)
            {
                float distance = 0.005f * this.SpeedAdjustmentFactor;
                while (distance > 0.0f)
                {
                    Track? track = GetTrackForTrain(train);
                    if (track != null)
                    {
                        (var newPosition, var newColumn, var newRow) = train.GetNextPosition(distance, track);

                        IMovable? otherTrain = GetMovableAt(newColumn, newRow);
                        if (GetTrackAt(newColumn, newRow) != null &&
                            (otherTrain == null || otherTrain == train))
                        {
                            train.Column = newColumn;
                            train.Row = newRow;
                            train.Angle = newPosition.Angle;
                            train.RelativeLeft = newPosition.RelativeLeft;
                            train.RelativeTop = newPosition.RelativeTop;
                            distance = newPosition.Distance;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            _gameLoopTimer?.Start();
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

            _storage?.WriteTracks(_tracks.Values);
        }

        public void RemoveTrack(int column, int row)
        {
            if (_tracks.TryGetValue((column, row), out Track track))
            {
                _tracks.Remove((column, row));
                track.RefreshNeighbors(true);
            }

            _storage?.WriteTracks(_tracks.Values);
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

        public void RemoveMovable(IMovable movable)
        {
            _movables.Remove(movable);
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

        public void ClearAll()
        {
            _tracks.Clear();
            _movables.Clear();

            _storage?.WriteTracks(_tracks.Values);
        }

        public void Dispose()
        {
            _gameLoopTimer?.Dispose();
        }

        public IMovable? GetMovableAt(int column, int row) => _movables.FirstOrDefault(t => t.Column == column && t.Row == row);
    }
}
