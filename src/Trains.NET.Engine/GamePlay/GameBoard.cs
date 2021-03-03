using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Trains.NET.Instrumentation;

namespace Trains.NET.Engine
{
    public class GameBoard : IGameBoard
    {
        private readonly ElapsedMillisecondsTimedStat _gameUpdateTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Game-LoopStepTime");

        private const int GameLoopInterval = 16;

        private ImmutableList<IMovable> _movables = ImmutableList<IMovable>.Empty;
        private readonly ILayout _layout;
        private readonly ITimer? _gameLoopTimer;
        private readonly ITerrainMap _terrainMap;
        private readonly IGameStorage? _storage;
        private readonly Train _reservedTrain = new();
        private int _terrainSeed;

        private int _columns;
        private int _rows;
        private Dictionary<Track, (Train, float)> _lastTrackLeases = new();

        public IEnumerable<(Track, Train, float)> LastTrackLeases => _lastTrackLeases.Select(kvp => (kvp.Key, kvp.Value.Item1, kvp.Value.Item2));
        public bool Enabled { get; set; } = true;

        public int TerrainSeed
        {
            get { return _terrainSeed; }
            set
            {
                _terrainSeed = value;
                _terrainMap.Reset(value, _columns, _rows);
            }
        }

        public GameBoard(ILayout trackLayout, ITerrainMap terrainMap, IGameStorage? storage, ITimer? timer)
        {
            _layout = trackLayout;
            _gameLoopTimer = timer;
            _terrainMap = terrainMap;
            _storage = storage;
            if (_gameLoopTimer != null)
            {
                _gameLoopTimer.Interval = GameLoopInterval;
                _gameLoopTimer.Elapsed += GameLoopTimerElapsed;
                _gameLoopTimer.Start();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public void Initialize(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;

            IEnumerable<IStaticEntity>? tracks = null;
            IEnumerable<Terrain>? terrain = null;
            IEnumerable<IMovable>? trains = null;
            try
            {
                var entities = _storage?.ReadEntities();
                tracks = entities!.OfType<IStaticEntity>();
                trains = entities.OfType<IMovable>();
                terrain = _storage?.ReadTerrain();
            }
            catch { }

            if (tracks is not null && terrain is not null && terrain.Any() && trains is not null)
            {
                _layout.Set(tracks);
                _terrainMap.Set(terrain);
                _movables = ImmutableList.CreateRange(trains);
            }
            else
            {
                ClearAll();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public void GameLoopStep()
        {
            if (!this.Enabled) return;

            using (_gameUpdateTime.Measure())
            {
                try
                {
                    DoGameLoopStep();
                }
                catch (Exception)
                {
                }
            }
        }

        private void DoGameLoopStep()
        {
            foreach (IUpdatableEntity entity in _layout.OfType<IUpdatableEntity>())
            {
                entity.Update();
            }

            Dictionary<Track, (Train, float)> takenTracks = new();

            // Reserve any pre-taken tracks
            foreach (Track track in _layout.OfType<Track>())
            {
                if (track.IsBlocked())
                {
                    takenTracks.Add(track, (_reservedTrain, -1));
                }
            }

            var timeSinceLastGameTick = (_gameLoopTimer?.TimeSinceLastTick / 16f);
            float speedModifier = 0.005f * (timeSinceLastGameTick ?? 1);

            foreach (Train train in _movables)
            {
                // Claim the track we are currently on, distance of 0
                if (_layout.TryGet(train.Column, train.Row, out Track? myTrack))
                {
                    takenTracks.Add(myTrack, (train, 0));
                }

                // If we are stopped, nothing more for this train to do

                if (train.Stopped && train.CurrentSpeed <= 0.0f)
                {
                    continue;
                }

                // Adjust our speed ahead of moving
                train.AdjustSpeed();

                // Move the actual train by the required distance
                MoveTrain(train, train, train.CurrentSpeed * speedModifier, takenTracks);

                // Claim in front of us for the number of carriages we have
                var dummyTrain = train.Clone();
                MoveTrain(dummyTrain, train, train.Carriages, takenTracks, 0);

                // Move ahead some more, claiming the tracks we cover. If we can't move
                // then pause the train
                if (MoveTrain(dummyTrain, train, (train.LookaheadDistance - train.CurrentSpeed) * speedModifier, takenTracks))
                {
                    train.Resume();
                }
                else
                {
                    train.Pause();
                }
            }

            _lastTrackLeases = takenTracks;
        }

        private bool MoveTrain(Train train, Train trainToLease, float distanceToMove, Dictionary<Track, (Train train, float timeAway)> takenTracks, int? timeAwayOverride = null)
        {
            if (distanceToMove <= 0) return true;

            List<TrainPosition>? steps = GetNextSteps(train, distanceToMove);

            TrainPosition? lastPosition = null;

            bool firstTimeInNewTrack = false;

            int numSteps = 0;
            foreach (TrainPosition newPosition in steps)
            {
                numSteps++;

                float timeAway = numSteps / train.DesiredSpeed;

                if (!_layout.TryGet(train.Column, train.Row, out Track? track))
                {
                    return false;
                }

                IMovable? otherTrain = GetMovableAt(newPosition.Column, newPosition.Row);

                if (!_layout.TryGet(newPosition.Column, newPosition.Row, out Track? nextTrack))
                {
                    break;
                }

                var neighbours = TrackNeighbors.GetConnectedNeighbours(_layout, track.Column, track.Row, false, false);
                if (track != nextTrack && !neighbours.Contains(nextTrack)) // next track is not connected
                {
                    break;
                }
                if (otherTrain != null && otherTrain.UniqueID != train.UniqueID) // There is a train that isn't us
                {
                    break;
                }

                // Check to see if there is another claim
                if (takenTracks.TryGetValue(nextTrack, out (Train Train, float TimeAway) otherTrains))
                {
                    // If it's not us
                    if (otherTrains.Train.UniqueID != train.UniqueID)
                    {
                        // and if they win, break
                        if (otherTrains.TimeAway < timeAway)
                        {
                            break;
                        }
                        // else we win, so pause them, and take the claim
                        else
                        {
                            otherTrains.Train.Pause();
                            takenTracks[nextTrack] = (trainToLease, timeAwayOverride ?? timeAway);
                        }
                    }
                }
                // No claim, we take it!
                else
                {
                    takenTracks[nextTrack] = (trainToLease, timeAwayOverride ?? timeAway);
                }

                lastPosition = newPosition;

                if (firstTimeInNewTrack)
                {
                    nextTrack.EnterTrack(train);
                }

                firstTimeInNewTrack = (train.Column != newPosition.Column || train.Row != newPosition.Row);

                train.ApplyStep(newPosition);
            }

            return lastPosition?.Distance <= 0.0f;
        }

        public List<TrainPosition> GetNextSteps(Train train, float distanceToMove)
        {
            var result = new List<TrainPosition>();

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

        private void GameLoopTimerElapsed(object? sender, EventArgs e) => GameLoopStep();

        private TrainPosition? GetNextPosition(TrainPosition currentPosition, float distance)
        {
            if (!_layout.TryGet(currentPosition.Column, currentPosition.Row, out Track? track))
            {
                return null;
            }

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

        public IMovable? AddTrain(int column, int row)
        {
            var train = new Train()
            {
                Column = column,
                Row = row
            };

            if (!_layout.TryGet(train.Column, train.Row, out _))
            {
                return null;
            }

            _movables = _movables.Add(train);

            return train;
        }

        public void RemoveMovable(IMovable movable)
        {
            _movables = _movables.Remove(movable);
        }

        public ImmutableList<IMovable> GetMovables()
        {
            return _movables;
        }

        public IEnumerable<T> GetMovables<T>() where T : IMovable
        {
            foreach (T movable in _movables.OfType<T>())
            {
                yield return movable;
            }
        }

        public void ClearAll()
        {
            _movables = _movables.Clear();
            _layout.Clear();

            _terrainSeed = new Random().Next();

            _terrainMap.Reset(_terrainSeed, _columns, _rows);
        }

        public void Dispose()
        {
            _gameLoopTimer?.Dispose();
            if (_storage is not null)
            {
                _storage.WriteEntities(_layout.OfType<IEntity>().Concat(_movables));
                _storage.WriteTerrain(_terrainMap);
            }
        }

        public IMovable? GetMovableAt(int column, int row)
        {
            foreach (var train in _movables.OfType<Train>())
            {
                if (train.Column == column && train.Row == row)
                {
                    return train;
                }

                var fakeTrain = train.Clone();

                for (var i = 0; i < train.Carriages; i++)
                {
                    var steps = GetNextSteps(fakeTrain, 1.0f);
                    foreach (var step in steps)
                    {
                        fakeTrain.ApplyStep(step);

                        if (fakeTrain.Column == column && fakeTrain.Row == row)
                        {
                            return train;
                        }
                    }
                }
            }
            return null;
        }
    }
}
