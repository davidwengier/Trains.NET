using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Trains.NET.Engine;

public class MovableLayout : IMovableLayout, IGameState, IGameStep
{
    private ImmutableList<IMovable> _movables = ImmutableList<IMovable>.Empty;
    private Dictionary<Track, (Train, float)> _lastTrackLeases = new();
    private readonly ILayout _layout;
    private readonly IEntityCollectionSerializer _gameSerializer;
    private readonly Train _reservedTrain;

    public int Count => _movables.Count;

    public IMovable this[int index] => _movables[index];

    public MovableLayout(ILayout layout, IEntityCollectionSerializer gameSerializer)
    {
        _layout = layout;
        _gameSerializer = gameSerializer;
        _reservedTrain = new Train(0);
    }

    public int IndexOf(IMovable movable)
        => _movables.IndexOf(movable);

    public IEnumerable<(Track, Train, float)> LastTrackLeases => _lastTrackLeases.Select(kvp => (kvp.Key, kvp.Value.Item1, kvp.Value.Item2));

    public void Add(IMovable movable)
        => _movables = _movables.Add(movable);

    public void Remove(IMovable movable)
        => _movables = _movables.Remove(movable);

    public bool Load(IGameStorage storage)
    {
        var entitiesString = storage.Read(nameof(IMovableLayout));
        if (entitiesString is null)
            return false;

        var entities = _gameSerializer.Deserialize(entitiesString);

        var movables = entities.OfType<IMovable>();

        if (movables is null)
            return false;

        _movables = ImmutableList.CreateRange(movables);

        return true;
    }

    public void Save(IGameStorage storage)
    {
        var entities = _gameSerializer.Serialize(_movables);
        storage.Write(nameof(IMovableLayout), entities);
    }

    public void Reset()
        => _movables = _movables.Clear();

    public void Update(long timeSinceLastTick)
    {
        MoveTrains(timeSinceLastTick);
    }

    private void MoveTrains(long timeSinceLastTick)
    {
        Dictionary<Track, (Train, float)> takenTracks = new();

        // Reserve any pre-taken tracks
        foreach (Track track in _layout.OfType<Track>())
        {
            if (track.IsBlocked())
            {
                takenTracks.Add(track, (_reservedTrain, -1));
            }
        }

        float speedModifier = 0.005f * (timeSinceLastTick / 16f);

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

        var steps = TrainMovement.GetNextSteps(_layout, train, distanceToMove);

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

            IMovable? otherTrain = GetAt(newPosition.Column, newPosition.Row);

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

    public IMovable? GetAt(int column, int row)
    {
        foreach (var movable in _movables)
        {
            if (movable is not Train train)
            {
                continue;
            }

            if (train.Column == column && train.Row == row)
            {
                return train;
            }

            var fakeTrain = train.Clone();

            for (var i = 0; i < train.Carriages; i++)
            {
                var steps = TrainMovement.GetNextSteps(_layout, fakeTrain, 1.0f);
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

    public IEnumerator<IMovable> GetEnumerator()
        => _movables.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => _movables.GetEnumerator();
}
