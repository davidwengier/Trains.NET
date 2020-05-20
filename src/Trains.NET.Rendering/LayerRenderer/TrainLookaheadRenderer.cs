using Trains.NET.Engine;

namespace Trains.NET.Rendering.LayerRenderer
{
    [Order(425)]
    internal class TrainLookaheadRenderer : ILayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _parameters;
        private readonly PaintBrush _paint = new PaintBrush
        {
            Color = Colors.LightPurple,
            Style = PaintStyle.Fill
        };

        public TrainLookaheadRenderer(IGameBoard gameBoard, IPixelMapper pixelMapper, ITrackParameters parameters)
        {
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
            _parameters = parameters;
        }

        public bool Enabled { get; set; } = true;

        public string Name => "Hitbox";

        public void Render(ICanvas canvas, int width, int height)
        {
            foreach (Train train in _gameBoard.GetMovables())
            {
                (int x, int y) = _pixelMapper.CoordsToPixels(train.Column, train.Row);

                canvas.DrawRect(x, y, _parameters.CellSize, _parameters.CellSize, _paint);

                foreach (var position in _gameBoard.GetNextSteps(train, train.LookaheadDistance))
                {
                    (x, y) = _pixelMapper.CoordsToPixels(position.Column, position.Row);

                    canvas.DrawRect(x, y, _parameters.CellSize, _parameters.CellSize, _paint);
                }
            }
        }
    }
}
