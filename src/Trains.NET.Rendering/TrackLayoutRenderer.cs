using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    internal class TrackLayoutRenderer : ILayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrackRenderer _trackRenderer;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _parameters;

        public bool Enabled { get; set; } = true;
        public string Name => "Tracks";

        public TrackLayoutRenderer(IGameBoard gameBoard, ITrackRenderer trackRenderer, IPixelMapper pixelMapper, ITrackParameters parameters)
        {
            _gameBoard = gameBoard;
            _trackRenderer = trackRenderer;
            _pixelMapper = pixelMapper;
            _parameters = parameters;
        }

        public void Render(SKCanvas canvas, int width, int height)
        {
            foreach ((int col, int row, Track track) in _gameBoard.GetTracks())
            {
                canvas.Save();

                (int x, int y) = _pixelMapper.CoordsToPixels(col, row);

                canvas.Translate(x, y);

                canvas.ClipRect(new SKRect(0, 0, _parameters.CellSize, _parameters.CellSize), SKClipOperation.Intersect, false);

                _trackRenderer.Render(canvas, track, _parameters.CellSize);

                canvas.Restore();
            }
        }
    }
}
