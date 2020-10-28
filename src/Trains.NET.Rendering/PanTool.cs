using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(1)]
    public class PanTool : IDraggableTool
    {
        private readonly IPixelMapper _pixelMapper;
        private int _lastX;
        private int _lastY;

        public ToolMode Mode => ToolMode.All;

        public string Name => "Pan";

        public PanTool(IPixelMapper pixelMapper)
        {
            _pixelMapper = pixelMapper;
        }

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
        }

        public bool IsValid(int column, int row)
            => false;
    }
}
