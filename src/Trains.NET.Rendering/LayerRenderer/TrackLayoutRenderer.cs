using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    internal class TrackLayoutRenderer : ICachableLayerRenderer
    {
        private readonly ILayout<Track> _trackLayout;
        private readonly IRenderer<Track> _trackRenderer;
        private readonly IGameParameters _gameParameters;
        private bool _dirty;

        public bool Enabled { get; set; } = true;
        public string Name => "Tracks";

        public bool IsDirty => _dirty;

        public TrackLayoutRenderer(ILayout<Track> trackLayout, IRenderer<Track> trackRenderer, IGameParameters gameParameters)
        {
            _trackLayout = trackLayout;
            _trackRenderer = trackRenderer;
            _gameParameters = gameParameters;

            _trackLayout.CollectionChanged += (s, e) => _dirty = true;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Track track in _trackLayout.OfType<Track>())
            {
                (int x, int y) = pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);

                if (x < -_gameParameters.CellSize || y < -_gameParameters.CellSize || x > width || y > height) continue;

                canvas.Save();

                canvas.Translate(x, y);

                canvas.ClipRect(new Rectangle(0, 0, _gameParameters.CellSize, _gameParameters.CellSize), ClipOperation.Intersect, false);

                _trackRenderer.Render(canvas, track);

                canvas.Restore();
            }
            _dirty = false;
        }
    }
}
