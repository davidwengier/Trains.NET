namespace Trains.NET.Engine
{
    internal class EraserTool : ITool
    {
        private readonly IGameBoard _gameBoard;

        public string Name => "Eraser";

        public EraserTool(IGameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public void Execute(int column, int row)
        {
            _gameBoard.RemoveTrack(column, row);
        }

        public bool IsValid(int column, int row) => _gameBoard.GetTrackAt(column, row) != null;
    }
}
