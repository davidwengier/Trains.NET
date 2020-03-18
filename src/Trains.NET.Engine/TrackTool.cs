namespace Trains.NET.Engine
{
    [Order(10)]
    internal class TrackTool : ITool
    {
        private readonly IGameBoard _gameBoard;

        public string Name => "Track";

        public TrackTool(IGameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public void Execute(int column, int row)
        {
            _gameBoard.AddTrack(column, row);
        }

        public bool IsValid(int column, int row) => true;
    }
}
