using System;
using System.Collections.Generic;
using System.Linq;
using Trains.NET.Instrumentation;

namespace Trains.NET.Engine
{
    internal class GameBoard : IGameBoard, IDisposable
    {
        private readonly ElapsedMillisecondsTimedStat _gameUpdateTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("GameLoopStepTime");

        private const int GameLoopInterval = 16;

        private readonly Dictionary<(int, int), Track> _tracks = new Dictionary<(int, int), Track>();
        private readonly List<IMovable> _movables = new List<IMovable>();
        private readonly ITimer? _gameLoopTimer;
        private readonly IGameStorage? _storage;

        public int Columns { get; set; }
        public int Rows { get; set; }
        public bool Enabled { get; set; } = true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public GameBoard(IGameStorage? storage, ITimer? timer)
        {
            _gameLoopTimer = timer;
            if (_gameLoopTimer != null)
            {
                _gameLoopTimer.Interval = GameLoopInterval;
                _gameLoopTimer.Elapsed += GameLoopTimerElapsed;
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

        public void GameLoopStep()
        {
            _gameUpdateTime.Start();
            try
            {
                Dictionary<Track,(Train, int)> takenTracks = new Dictionary<Track, (Train, int)>();
                foreach (Train train in _movables)
                {
                    Train dummyTrain = train.Clone();

                    MoveTrain(dummyTrain, train.LookaheadDistance, takenTracks);
                }

                foreach (Train train in _movables)
                {
                    if (train.CurrentSpeed == 0 && train.DesiredSpeed == 0) continue;

                    train.AdjustSpeed();

                    Train dummyTrain = train.Clone();

                    if (MoveTrain(dummyTrain, train.LookaheadDistance, takenTracks))
                    {
                        if (this.Enabled)
                        {
                            train.Resume();
                        }
                    }
                    else
                    {
                        train.Pause();
                    }
                    MoveTrain(train, train.DistanceToMove, null);
                }
            }
            catch (Exception)
            {
            }
            _gameUpdateTime.Stop();
        }

        private bool MoveTrain(Train train, float distanceToMove, Dictionary<Track, (Train, int)>? takenTracks)
        {
            if (distanceToMove <= 0) return true;

            List<TrainPosition>? steps = GetNextSteps(train, distanceToMove);

            TrainPosition? lastPosition = null;

            int numSteps = 0;
            foreach (TrainPosition newPosition in steps)
            {
                numSteps++;

                Track? track = GetTrackForTrain(train);
                if (track == null) return false;

                IMovable? otherTrain = GetMovableAt(newPosition.Column, newPosition.Row);
                Track? nextTrack = GetTrackAt(newPosition.Column, newPosition.Row);

                if (nextTrack == null)   // No track to move to
                {
                    break;
                }

                if (track != nextTrack && !track.GetNeighbors().Contains(nextTrack)) // next track is not connected
                {
                    break;
                }
                if (otherTrain != null && otherTrain.UniqueID != train.UniqueID) // There is a train that isn't us
                {
                    break;
                }

                if (takenTracks != null &&
                    takenTracks.TryGetValue(nextTrack, out (Train Train, int NumSteps) otherTrains) && 
                    otherTrains.Train.UniqueID != train.UniqueID &&
                    otherTrains.NumSteps < numSteps)
                {
                    break;
                }

                if (takenTracks != null)
                {
                    takenTracks[nextTrack] = (train, numSteps);
                }

                lastPosition = newPosition;

                train.Column = newPosition.Column;
                train.Row = newPosition.Row;
                train.Angle = newPosition.Angle;
                train.RelativeLeft = newPosition.RelativeLeft;
                train.RelativeTop = newPosition.RelativeTop;
            }

            return lastPosition?.Distance <= 0.0f;
        }

        public List<TrainPosition> GetNextSteps(Train train, float distanceToMove)
        {
            List<TrainPosition> result = new List<TrainPosition>();

            float distance = distanceToMove;
            while (distance > 0.0f)
            {
                TrainPosition position = result.LastOrDefault() ?? train.GetPosition();

                TrainPosition? newPosition = GetNextPosition(position, distance);
                if (newPosition != null)
                {
                    result.Add(newPosition);
                    distance = newPosition.Distance;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private void GameLoopTimerElapsed(object sender, EventArgs e) => GameLoopStep();

        private TrainPosition? GetNextPosition(TrainPosition currentPosition, float distance)
        {
            Track? track = GetTrackAt(currentPosition.Column, currentPosition.Row);

            if (track == null) return null;

            TrainPosition position = currentPosition.Clone();
            position.Distance = distance;

            track.Move(position);

            if (position.RelativeLeft < 0.0f)
            {
                position.Column--;
                position.RelativeLeft = 1.0f;
            }
            if (position.RelativeLeft > 1.0f)
            {
                position.Column++;
                position.RelativeLeft = 0.0f;
            }
            if (position.RelativeTop < 0.0f)
            {
                position.Row--;
                position.RelativeTop = 1.0f;
            }
            if (position.RelativeTop > 1.0f)
            {
                position.Row++;
                position.RelativeTop = 0.0f;
            }

            return position;
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

        public IMovable? AddTrain(int column, int row)
        {
            var train = new Train()
            {
                Column = column,
                Row = row
            };

            Track? track = GetTrackForTrain(train);
            if (track == null)
            {
                return null;
            }

            _movables.Add(train);

            return train;
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
