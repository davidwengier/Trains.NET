using Trains.NET.Engine;
using Trains.NET.Rendering.Trains;

namespace Trains.NET.Rendering.LayerRenderer
{
    [Order(470)]
    public class TrainLookaheadRenderer : ILayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrainPainter _painter;

        public TrainLookaheadRenderer(IGameBoard gameBoard, ITrainPainter painter)
        {
            _gameBoard = gameBoard;
            _painter = painter;
        }

        public bool Enabled { get; set; }

        public string Name => "Hitbox";

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (var (track, train, _) in _gameBoard.LastTrackLeases)
            {
                var _paint = new PaintBrush
                {
                    Color = _painter.GetPalette(train).FrontSectionEndColor with { A = 200 },
                    Style = PaintStyle.Fill
                };

                (int x, int y, _) = pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);

                canvas.DrawRect(x, y, pixelMapper.CellSize, pixelMapper.CellSize, _paint);
            }
        }
    }
}
