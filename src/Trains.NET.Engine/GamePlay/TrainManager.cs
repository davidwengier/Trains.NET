using System;

namespace Trains.NET.Engine
{
    public class TrainManager : ITrainManager
    {
        private Train? _currentTrain;

        public event EventHandler? Changed;

        public Train? CurrentTrain
        {
            get { return _currentTrain; }
            set
            {
                _currentTrain = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
