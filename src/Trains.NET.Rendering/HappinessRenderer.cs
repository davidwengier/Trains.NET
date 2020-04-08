using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(449)]
    internal class HappinessRenderer : ILayerRenderer//, IDisposable
    {
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _parameters;
        private readonly PaintBrush _paint = new PaintBrush
        {
            Color = Colors.Cyan,
            Style = PaintStyle.Fill
        };

        public bool Enabled { get; set; }
        public string Name => "Happiness";

        public HappinessRenderer(IGameBoard gameBoard, IPixelMapper pixelMapper, ITrackParameters parameters)
        {
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
            _parameters = parameters;
        }

        //public void Dispose()
        //{
        //    _paint.Dispose();
        //}

        public void Render(ICanvas canvas, int width, int height)
        {
            foreach ((int col, int row, Track track) in _gameBoard.GetTracks())
            {
                if (!track.Happy)
                {
                    continue;
                }

                (int x, int y) = _pixelMapper.CoordsToPixels(col, row);

                canvas.DrawRect(x, y, _parameters.CellSize, _parameters.CellSize, _paint);
            }
        }
    }
}
