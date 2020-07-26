using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;
using Trains.NET.Rendering;

namespace Trains.NET.Comet
{
    [Order(1)]
    internal class PointerTool : IDraggableTool
    {
        private readonly ITrainController _gameState;
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackLayout _trackLayout;
        private int _lastX;
        private int _lastY;

        public ToolMode Mode => ToolMode.All;
        public PointerTool(ITrainController gameState, IGameBoard gameBoard, IPixelMapper pixelMapper, ITrackLayout trackLayout)
        {
            _gameState = gameState;
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
            _trackLayout = trackLayout;
        }

        public string Name => "Pointer";

        public void StartDrag(int x, int y)
        {
            _lastX = x;
            _lastY = y;
        }

        public void ContinueDrag(int x, int y)
        {
            _pixelMapper.AdjustViewPort(x - _lastX, y - _lastY);
            _lastX = x;
            _lastY = y;
        }

        public void Execute(int column, int row)
        {
            if (_gameBoard.GetMovableAt(column, row) is Train train)
            {
                _gameState.SetCurrentTrain(train);
            }
            else 
            {
                _trackLayout.ToggleTrack(column, row);
            }
        }

        public bool IsValid(int column, int row)
            => _gameBoard.GetMovableAt(column, row) is Train
                || (_trackLayout.TryGet(column, row, out Track? track) && track.HasAlternateState());
    }
}
