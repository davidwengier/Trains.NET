namespace Trains.NET.Engine
{
    [Order(20)]
    public class EraserTool : ITool
    {
        private readonly ILayout _collection;
        private readonly IGameBoard _gameBoard;
        private readonly ITrainManager _trainManager;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Eraser";

        public EraserTool(ILayout trackLayout, IGameBoard gameBoard, ITrainManager trainManager)
        {
            _collection = trackLayout;
            _gameBoard = gameBoard;
            _trainManager = trainManager;
        }

        public void Execute(int column, int row, bool isPartOfDrag)
        {
            _collection.Remove(column, row);
            if (_gameBoard.GetMovableAt(column, row) is { } moveable)
            {
                _gameBoard.RemoveMovable(moveable);
                if (_trainManager.CurrentTrain == moveable)
                {
                    _trainManager.CurrentTrain = null;
                }
            }
        }

        public bool IsValid(int column, int row) => true;
    }
}
