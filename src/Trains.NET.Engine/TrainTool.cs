namespace Trains.NET.Engine
{
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
            //_gameBoard.AddTrack(column, row);
        }
    }
}
