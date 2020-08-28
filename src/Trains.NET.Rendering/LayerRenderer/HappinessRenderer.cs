using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(400)]
    public class HappinessRenderer : ILayerRenderer
    {
        private readonly ILayout<Track> _trackLayout;
        private readonly PaintBrush _paint = new PaintBrush
        {
            Color = Colors.Cyan,
            Style = PaintStyle.Fill
        };

        public bool Enabled { get; set; }
        public string Name => "Happiness";

        public HappinessRenderer(ILayout<Track> trackLayout)
        {
            _trackLayout = trackLayout;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Track track in _trackLayout)
            {
                if (!track.Happy)
                {
                    continue;
                }

                (int x, int y, bool onScreen) = pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);

                if (!onScreen) continue;

                canvas.DrawRect(x, y, pixelMapper.CellSize, pixelMapper.CellSize, _paint);
            }
        }
    }
}
