using Trains.NET.Engine;

namespace Trains.NET.Comet
{
    [Order(50)]
    internal class TrainTool : ITool
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrainController _gameState;

        public string Name => "Train";

        public TrainTool(IGameBoard gameBoard, ITrainController gameState)
        {
            _gameBoard = gameBoard;
            _gameState = gameState;
        }

        public void Execute(int column, int row)
        {
            if (_gameBoard.AddTrain(column, row) is Train train)
            {
                _gameState.SetCurrentTrain(train);
            }
        }

        public bool IsValid(int column, int row) => _gameBoard.GetTrackAt(column, row) != null &&
            _gameBoard.GetMovableAt(column, row) == null;
    }
}
