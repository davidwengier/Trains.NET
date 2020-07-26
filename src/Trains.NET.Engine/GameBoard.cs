using System;
using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine.Tracks;
using Trains.NET.Instrumentation;

namespace Trains.NET.Engine
{
    internal class GameBoard : IGameBoard
    {
        private readonly ElapsedMillisecondsTimedStat _gameUpdateTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Game-LoopStepTime");

        private const int GameLoopInterval = 16;

        private readonly Dictionary<Track, (Train, float)> _takenTracks = new();
        private readonly List<IMovable> _movables = new();
        private readonly ITrackLayout _trackLayout;
        private readonly ITimer? _gameLoopTimer;

        public int Columns { get; set; }
        public int Rows { get; set; }
        public bool Enabled { get; set; } = true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public GameBoard(ITrackLayout trackLayout, IGameStorage? storage, ITimer? timer)
        {
            _trackLayout = trackLayout;
            _gameLoopTimer = timer;
            if (_gameLoopTimer != null)
            {
                _gameLoopTimer.Interval = GameLoopInterval;
                _gameLoopTimer.Elapsed += GameLoopTimerElapsed;
                _gameLoopTimer.Start();
            }

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
                _trackLayout.SetTracks(tracks);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public void GameLoopStep()
        {
            if (!this.Enabled) return;

            _gameUpdateTime.Start();
            try
            {
                DoGameLoopStep();
            }
            catch (Exception)
            {
            }
            _gameUpdateTime.Stop();
        }

        private void DoGameLoopStep()
        {
            _takenTracks.Clear();
            foreach (Train train in _movables.ToArray())
            {
                // Claim the track we are currently on, distance of 0
                if (_trackLayout.TryGet(train.Column, train.Row, out Track? myTrack))
                {
                    _takenTracks.Add(myTrack, (train, 0));
                }

                // If we are stopped, nothing more for this train to do

                if (train.Stopped && train.CurrentSpeed <= 0.0f)
                {
                    continue;
                }

                // Adjust our speed ahead of moving
                train.AdjustSpeed();

                // Clone our train for look-behind purposes ahead of moving
                //  so we always look the same distance behind irrespective of speed
                Train dummyTrain = train.Clone();
                dummyTrain.SetAngle(dummyTrain.Angle - 180);

                // Move the actual train by the required distance
                MoveTrain(train, train, train.CurrentSpeed, _takenTracks);

                // If we can't even move the required ammount, we have hit an edge case
                //  we should deal with it here! Maybe call stop?

                // Claim behind us & set our parent as the dummy 
                //  to abuse the fact no one can pause it
                MoveTrain(dummyTrain, dummyTrain, 1.0f, _takenTracks);

                // Clone our train for look-ahead purposes
                dummyTrain = train.Clone();

                // Move our lookahead train clone, claiming the tracks we cover
                if (MoveTrain(dummyTrain, train, train.LookaheadDistance - train.CurrentSpeed, _takenTracks))
                {
                    train.Resume();
                }
                else
                {
                    train.Pause();
                }
            }
        }

        private bool MoveTrain(Train train, Train parentTrain, float distanceToMove, Dictionary<Track, (Train train, float timeAway)> takenTracks)
        {
            if (distanceToMove <= 0) return true;

            float speedModifier = 0.005f * ((_gameLoopTimer?.TimeSinceLastTick / 16f) ?? 1);
            distanceToMove = distanceToMove * speedModifier;

            List<TrainPosition>? steps = GetNextSteps(train, distanceToMove);

            TrainPosition? lastPosition = null;

            int numSteps = 0;
            foreach (TrainPosition newPosition in steps)
            {
                numSteps++;

                float timeAway = numSteps / train.DesiredSpeed;

                if (!_trackLayout.TryGet(train.Column, train.Row, out Track? track))
                {
                    return false;
                }

                IMovable? otherTrain = GetMovableAt(newPosition.Column, newPosition.Row);

                if (!_trackLayout.TryGet(newPosition.Column, newPosition.Row, out Track? nextTrack))
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
                            takenTracks[nextTrack] = (parentTrain, timeAway);
                        }
                    }
                }
                // No claim, we take it!
                else
                {
                    takenTracks[nextTrack] = (parentTrain, timeAway);
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

        private void GameLoopTimerElapsed(object sender, EventArgs e) => GameLoopStep();

        private TrainPosition? GetNextPosition(TrainPosition currentPosition, float distance)
        {
            if (!_trackLayout.TryGet(currentPosition.Column, currentPosition.Row, out Track? track))
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

            if (!_trackLayout.TryGet(train.Column, train.Row, out _))
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

        public IEnumerable<IMovable> GetMovables()
        {
            foreach (IMovable movable in _movables.ToArray())
            {
                yield return movable;
            }
        }

        public IEnumerable<T> GetMovables<T>() where T : IMovable
        {
            foreach (T movable in _movables.OfType<T>().ToArray())
            {
                yield return movable;
            }
        }

        public void ClearAll()
        {
            _movables.Clear();
            _trackLayout.Clear();
        }

        public void Dispose()
        {
            _gameLoopTimer?.Dispose();
        }

        public IMovable? GetMovableAt(int column, int row) => _movables.FirstOrDefault(t => t is not null && t.Column == column && t.Row == row);
    }
}
