using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;
using Trains.NET.Rendering.LayerRenderer;

namespace Trains.NET.Rendering
{
    [Order(450)]
    internal class TrackLayoutRenderer : ILayerRenderer, ICachableLayerRenderer
    {
        private readonly ITrackLayout _trackLayout;
        private readonly ITrackRenderer _trackRenderer;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _parameters;
        private bool _dirty;

        public bool Enabled { get; set; } = true;
        public string Name => "Tracks";

        public bool IsDirty => _dirty;

        public TrackLayoutRenderer(ITrackLayout trackLayout, ITrackRenderer trackRenderer, IPixelMapper pixelMapper, ITrackParameters parameters)
        {
            _trackLayout = trackLayout;
            _trackRenderer = trackRenderer;
            _pixelMapper = pixelMapper;
            _parameters = parameters;

            _trackLayout.TracksChanged += (s, e) => _dirty = true;
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            foreach ((int col, int row, Track track) in _trackLayout)
            {
                (int x, int y) = _pixelMapper.CoordsToViewPortPixels(col, row);

                if (x < -_parameters.CellSize || y < -_parameters.CellSize || x > width || y > height) continue;

                canvas.Save();

                canvas.Translate(x, y);

                canvas.ClipRect(new Rectangle(0, 0, _parameters.CellSize, _parameters.CellSize), ClipOperation.Intersect, false);

                _trackRenderer.Render(canvas, track, _parameters.CellSize);

                canvas.Restore();
            }
            _dirty = false;
        }
    }
}
