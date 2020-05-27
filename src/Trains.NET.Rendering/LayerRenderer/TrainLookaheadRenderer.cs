using Trains.NET.Engine;
using Trains.NET.Rendering.Trains;

namespace Trains.NET.Rendering.LayerRenderer
{
    [Order(425)]
    internal class TrainLookaheadRenderer : ILayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _parameters;
        private readonly ITrainPainter _painter;

        public TrainLookaheadRenderer(IGameBoard gameBoard, IPixelMapper pixelMapper, ITrackParameters parameters, ITrainPainter painter)
        {
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
            _parameters = parameters;
            _painter = painter;
        }

        public bool Enabled { get; set; } = false;

        public string Name => "Hitbox";

        public void Render(ICanvas canvas, int width, int height)
        {
            foreach (Train train in _gameBoard.GetMovables())
            {
                PaintBrush _paint = new PaintBrush
                {
                    Color = _painter.GetPalette(train).FrontSectionEndColor,
                    Style = PaintStyle.Fill
                };

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
