using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(1000)]
    internal class BoundingBoxRenderer : ILayerRenderer //, IDisposable
    {
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _trackParameters;
        private readonly PaintBrush _focalPoint = new PaintBrush
        {
            Color = Colors.Green,
            Style = PaintStyle.Fill
        };

        public bool Enabled { get; set; } = false;

        public string Name => "Bounding Boxes";

        public BoundingBoxRenderer(IGameBoard gameBoard, IPixelMapper pixelMapper, ITrackParameters trackParameters)
        {
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
            _trackParameters = trackParameters;
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            foreach (IMovable movable in _gameBoard.GetMovables())
            {
                canvas.Save();

                (int x, int y) = _pixelMapper.CoordsToPixels(movable.Column, movable.Row);

                canvas.Translate(x, y);

                TrainRenderer.SetupCanvasToDrawTrain(canvas, movable, _trackParameters);

                canvas.DrawCircle(movable.FrontEdgeDistance * _trackParameters.CellSize, 0, 3, _focalPoint);

                canvas.Restore();
            }
        }

        //public void Dispose()
        //{
        //    _focalPoint.Dispose();
        //    _highlight.Dispose();
        //}
    }
}
