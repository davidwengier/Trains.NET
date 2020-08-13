namespace Trains.NET.Engine
{
    [Order(10)]
    public class ClearAllCommand : ICommand
    {
        private readonly IGameBoard _gameBoard;

        public ClearAllCommand(IGameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public string Name => "Clear All";

        public void Execute()
        {
            _gameBoard.ClearAll();
        }
    }
}
