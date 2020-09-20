namespace Trains.NET.Engine
{
    [Order(10)]
    public class ClearAllCommand : ICommand
    {
        private readonly ITrainManager _trainManager;
        private readonly IGameBoard _gameBoard;

        public ClearAllCommand(ITrainManager trainManager, IGameBoard gameBoard)
        {
            _trainManager = trainManager;
            _gameBoard = gameBoard;
        }

        public string Name => "Clear All";

        public void Execute()
        {
            _trainManager.CurrentTrain = null;
            _gameBoard.ClearAll();
        }
    }
}
