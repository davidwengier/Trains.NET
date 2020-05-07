using Trains.NET.Engine;

namespace Trains.NET.Comet
{
    [Order(1)]
    internal class PointerTool : ITool
    {
        private readonly ITrainController _gameState;
        private readonly IGameBoard _gameBoard;

        public PointerTool(ITrainController gameState, IGameBoard gameBoard)
        {
            _gameState = gameState;
            _gameBoard = gameBoard;
        }

        public string Name => "Pointer";

        public void Execute(int column, int row)
        {
            if (_gameBoard.GetMovableAt(column, row) is Train train)
            {
                _gameState.SetCurrentTrain(train);
            }
        }

        public bool IsValid(int column, int row) => true;
    }
}
