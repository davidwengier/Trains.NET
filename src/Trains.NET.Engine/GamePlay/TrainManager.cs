using System;
using System.ComponentModel;

namespace Trains.NET.Engine
{
    public class TrainManager : ITrainManager
    {
        private Train? _currentTrain;
        private readonly IGameBoard _gameBoard;

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

        public TrainManager(IGameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public void ToggleFollow(Train train)
        {
            // if we're already following the train specified, toggle it off
            Train? trainToSet = train.Follow ? null : train;

            foreach (Train t in _gameBoard.GetMovables<Train>())
            {
                t.Follow = (t == trainToSet);
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void Train_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            CurrentTrainPropertyChanged?.Invoke(sender, e);
        }
    }
}
