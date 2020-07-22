using System.Linq;
using Comet;
using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains.NET.Comet
{
    internal class TrainController : ITrainController
    {
        private readonly IGameBoard _gameBoard;

        public TrainController(IGameBoard gameBoard, ITrackLayout layout)
        {
            _gameBoard = gameBoard;

            this.BuildMode = !layout.Any();
        }

        public State<Train?> CurrentTrain { get; } = new State<Train?>();

        public State<string> Display { get; } = new State<string>("< No train selected >");

        public State<string> SpeedDisplay { get; } = new State<string>("--");

        public State<bool> BuildMode { get; } = new State<bool>(true);

        public void SetCurrentTrain(Train? train)
        {
            if (this.CurrentTrain.Value != null)
            {
                this.CurrentTrain.Value.PropertyChanged -= Train_PropertyChanged;
            }
            this.CurrentTrain.Value = train;
            if (this.CurrentTrain.Value != null)
            {
                this.CurrentTrain.Value.PropertyChanged += Train_PropertyChanged;
            }
            Update();
        }

        private void Train_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Update();
        }

        public void Start()
        {
            this.CurrentTrain.Value?.Start();
            Update();
        }

        public void Stop()
        {
            this.CurrentTrain.Value?.Stop();
            Update();
        }

        public void Slower()
        {
            if (this.CurrentTrain.Value != null)
            {
                this.CurrentTrain.Value.Slower();
            }
            Update();
        }

        public void Faster()
        {
            if (this.CurrentTrain.Value != null)
            {
                this.CurrentTrain.Value.Faster();
            }
            Update();
        }

        public void Delete()
        {
            if (this.CurrentTrain.Value != null)
            {
                _gameBoard.RemoveMovable(this.CurrentTrain.Value);
                SetCurrentTrain(null);
            }
        }

        private void Update()
        {
            if (this.CurrentTrain.Value == null)
            {
                this.Display.Value = "< No train selected >";
                this.SpeedDisplay.Value = "--";
            }
            else
            {
                this.Display.Value = this.CurrentTrain.Value.Name;
                this.SpeedDisplay.Value = $"{this.CurrentTrain.Value.CurrentSpeed} km/h";
            }
        }

        public void ToggleFollowMode()
        {
            if (this.CurrentTrain.Value != null)
            {
                this.CurrentTrain.Value.Follow = !this.CurrentTrain.Value.Follow;
            }
        }

        public void ToggleBuildMode()
        {
            this.BuildMode.Value = !this.BuildMode.Value;

            _gameBoard.Enabled = !this.BuildMode.Value;
        }
    }
}
