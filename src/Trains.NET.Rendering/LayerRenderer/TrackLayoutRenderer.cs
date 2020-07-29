using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    internal class TrackLayoutRenderer : ICachableLayerRenderer
    {
        private readonly IStaticEntityCollection _trackLayout;
        private readonly ITrackRenderer _trackRenderer;
        private readonly ITrackParameters _parameters;
        private bool _dirty;

        public bool Enabled { get; set; } = true;
        public string Name => "Tracks";

        public bool IsDirty => _dirty;

        public TrackLayoutRenderer(IStaticEntityCollection trackLayout, ITrackRenderer trackRenderer, ITrackParameters parameters)
        {
            _trackLayout = trackLayout;
            _trackRenderer = trackRenderer;
            _parameters = parameters;

            _trackLayout.CollectionChanged += (s, e) => _dirty = true;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Track track in _trackLayout.OfType<Track>())
            {
                (int x, int y) = pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);

                if (x < -_parameters.CellSize || y < -_parameters.CellSize || x > width || y > height) continue;

                canvas.Save();

                canvas.Translate(x, y);

                canvas.ClipRect(new Rectangle(0, 0, _parameters.CellSize, _parameters.CellSize), ClipOperation.Intersect, false);

                _trackRenderer.Render(canvas, track);

                canvas.Restore();
            }
            _dirty = false;
        }
    }
}
