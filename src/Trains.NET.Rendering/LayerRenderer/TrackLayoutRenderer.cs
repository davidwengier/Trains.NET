using Trains.NET.Engine;
using Trains.NET.Rendering.LayerRenderer;

namespace Trains.NET.Rendering
{
    [Order(450)]
    internal class TrackLayoutRenderer : ILayerRenderer, ICachableLayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrackRenderer _trackRenderer;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _parameters;
        private bool _dirty;

        public bool Enabled { get; set; } = true;
        public string Name => "Tracks";

        public bool IsDirty => _dirty;

        public TrackLayoutRenderer(IGameBoard gameBoard, ITrackRenderer trackRenderer, IPixelMapper pixelMapper, ITrackParameters parameters)
        {
            _gameBoard = gameBoard;
            _trackRenderer = trackRenderer;
            _pixelMapper = pixelMapper;
            _parameters = parameters;

            _gameBoard.TracksChanged += (s, e) => _dirty = true;
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            foreach ((int col, int row, Track track) in _gameBoard.GetTracks())
            {
                canvas.Save();

                (int x, int y) = _pixelMapper.CoordsToPixels(col, row);

                canvas.Translate(x, y);

                canvas.ClipRect(new Rectangle(0, 0, _parameters.CellSize, _parameters.CellSize), ClipOperation.Intersect, false);

                _trackRenderer.Render(canvas, track, _parameters.CellSize);

                canvas.Restore();
            }
            _dirty = false;
        }
    }
}
