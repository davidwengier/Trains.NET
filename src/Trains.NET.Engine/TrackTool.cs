namespace Trains.NET.Engine
{
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
    }
}
