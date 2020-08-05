using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(400)]
    internal class HappinessRenderer : ILayerRenderer
    {
        private readonly ILayout<Track> _trackLayout;
        private readonly ITrackParameters _parameters;
        private readonly PaintBrush _paint = new PaintBrush
        {
            Color = Colors.Cyan,
            Style = PaintStyle.Fill
        };

        public bool Enabled { get; set; }
        public string Name => "Happiness";

        public HappinessRenderer(ILayout<Track> trackLayout, ITrackParameters parameters)
        {
            _trackLayout = trackLayout;
            _parameters = parameters;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Track track in _trackLayout)
            {
                if (!track.Happy)
                {
                    continue;
                }

                (int x, int y) = pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);

                canvas.DrawRect(x, y, _parameters.CellSize, _parameters.CellSize, _paint);
            }
        }
    }
}
