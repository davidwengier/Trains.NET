using Trains.NET.Engine;

namespace Trains.NET.Comet
{
    [Order(50)]
    public class TrainTool : ITool
    {
        private readonly IGameBoard _gameBoard;
        private readonly ILayout<Track> _trackLayout;
        private readonly ITrainController _gameState;

        public ToolMode Mode => ToolMode.Play;
        public string Name => "Train";

        public TrainTool(IGameBoard gameBoard, ILayout<Track> trackLayout, ITrainController gameState)
        {
            _gameBoard = gameBoard;
            _trackLayout = trackLayout;
            _gameState = gameState;
        }

        public void Execute(int column, int row)
        {
            if (_gameBoard.AddTrain(column, row) is Train train)
            {
                _gameState.SetCurrentTrain(train);
            }
        }

        public bool IsValid(int column, int row) => _trackLayout.TryGet(column, row, out _) &&
            _gameBoard.GetMovableAt(column, row) == null;
    }
}
