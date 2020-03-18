using System.Linq;

namespace Trains.NET.Engine
{
    [Order(50)]
    internal class TrainTool : ITool
    {
        private readonly IGameBoard _gameBoard;

        public string Name => "Train";

        public TrainTool(IGameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public void Execute(int column, int row)
        {
            _gameBoard.AddTrain(column, row);
        }

        public bool IsValid(int column, int row) => _gameBoard.GetTrackAt(column, row) != null &&
            !_gameBoard.GetMovables().Any(t => t.Column == column && t.Row == row);
    }
}
