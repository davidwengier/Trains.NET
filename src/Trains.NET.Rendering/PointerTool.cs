using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(2)]
    public class PointerTool : ITool
    {
        private readonly ITrainManager _trainManager;
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly ILayout<Track> _trackLayout;

        public ToolMode Mode => ToolMode.All;
        public PointerTool(ITrainManager trainManager, IGameBoard gameBoard, IPixelMapper pixelMapper, ILayout<Track> trackLayout)
        {
            _trainManager = trainManager;
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
            _trackLayout = trackLayout;
        }

        public string Name => "Pointer";

        public void Execute(int column, int row)
        {
            if (_gameBoard.GetMovableAt(column, row) is Train train)
            {
                _trainManager.CurrentTrain = train;
            }
            else
            {
                if (_trackLayout.TryGet(column, row, out Track? track))
                {
                    _trackLayout.SelectedEntity = track;
                }
                else
                {
                    _trackLayout.SelectedEntity = null;
                }
            }
        }

        public bool IsValid(int column, int row)
            => _gameBoard.GetMovableAt(column, row) is Train
               || _trackLayout.TryGet(column, row, out _);
    }
}
