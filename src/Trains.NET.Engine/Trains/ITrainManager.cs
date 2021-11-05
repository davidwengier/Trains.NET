
using System;
using System.ComponentModel;

namespace Trains.NET.Engine;

public interface ITrainManager
{
    event EventHandler? Changed;
    event PropertyChangedEventHandler? CurrentTrainPropertyChanged;

    Train? CurrentTrain { get; set; }

    void ToggleFollow(Train train);
    void PreviousTrain();
    void NextTrain();
    IMovable? AddTrain(int column, int row);
    bool TryGetFollowTrainPosition(out int col, out int row);
}
