using Trains.NET.Engine;
using Trains.NET.Rendering;

namespace Trains.NET.Comet
{
    [Order(1)]
    internal class PointerTool : ITool, IDraggableTool, ICustomCursor
    {
        private readonly ITrainController _gameState;
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private int _lastX;
        private int _lastY;

        public PointerTool(ITrainController gameState, IGameBoard gameBoard, IPixelMapper pixelMapper)
        {
            _gameState = gameState;
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
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
        }

        public bool IsValid(int column, int row) => _gameBoard.GetMovableAt(column, row) is Train;

        public void Render(ICanvas canvas)
        {
            var paint = new PaintBrush
            {
                Style = PaintStyle.Fill,
                Color = Colors.LightPurple
            };
            canvas.DrawCircle(0, 0, 10, paint);
        }
    }
}
