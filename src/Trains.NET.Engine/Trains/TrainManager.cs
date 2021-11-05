using System;
using System.ComponentModel;
using System.Linq;

namespace Trains.NET.Engine;

public class TrainManager : ITrainManager
{
    private Train? _currentTrain;
    private readonly IMovableLayout _movableLayout;
    private readonly ILayout _layout;
    private readonly Random _trainSpawnRandom = new();

    public event EventHandler? Changed;
    public event PropertyChangedEventHandler? CurrentTrainPropertyChanged;

    public Train? CurrentTrain
    {
        get { return _currentTrain; }
        set
        {
            if (_currentTrain != null)
            {
                _currentTrain.PropertyChanged -= Train_PropertyChanged;
            }
            _currentTrain = value;
            if (_currentTrain != null)
            {
                _currentTrain.PropertyChanged += Train_PropertyChanged;
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public TrainManager(IMovableLayout movableLayout, ILayout layout)
    {
        _movableLayout = movableLayout;
        _layout = layout;
    }

    public IMovable? AddTrain(int column, int row)
    {
        var train = new Train(_trainSpawnRandom.Next())
        {
            Column = column,
            Row = row
        };

        if (!_layout.TryGet(train.Column, train.Row, out _))
        {
            return null;
        }

        _movableLayout.Add(train);

        return train;
    }

    public void ToggleFollow(Train train)
    {
        // if we're already following the train specified, toggle it off
        Train? trainToSet = train.Follow ? null : train;

        foreach (Train t in _movableLayout.Get<Train>())
        {
            t.Follow = (t == trainToSet);
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void Train_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        CurrentTrainPropertyChanged?.Invoke(sender, e);
    }

    public void PreviousTrain()
    {
        var trains = _movableLayout.Get();
        int index = _currentTrain == null ? -1 : trains.IndexOf(_currentTrain);
        if (index == -1 || index == 0)
        {
            this.CurrentTrain = trains[^1] as Train;
        }
        else
        {
            this.CurrentTrain = trains[index - 1] as Train;
        }
    }

    public void NextTrain()
    {
        var trains = _movableLayout.Get();
        int index = _currentTrain == null ? -1 : trains.IndexOf(_currentTrain);
        if (index == -1 || index == trains.Count - 1)
        {
            this.CurrentTrain = trains[0] as Train;
        }
        else
        {
            this.CurrentTrain = trains[index + 1] as Train;
        }
    }

    public bool TryGetFollowTrainPosition(out int col, out int row)
    {
        col = -1;
        row = -1;
        foreach (IMovable vehicle in _movableLayout.Get())
        {
            if (vehicle is Train { Follow: true } train)
            {
                if (train.Carriages == 0)
                {
                    col = train.Column;
                    row = train.Row;
                }
                else
                {
                    var locomotivePosition = TrainMovement.GetNextSteps(_layout, train, train.Carriages).Last();
                    col = locomotivePosition.Column;
                    row = locomotivePosition.Row;
                }
                return true;
            }
        }
        return false;
    }
}
