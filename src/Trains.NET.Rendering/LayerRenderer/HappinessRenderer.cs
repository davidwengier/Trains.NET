using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains.NET.Rendering
{
    [Order(400)]
    internal class HappinessRenderer : ILayerRenderer
    {
        private readonly ITrackLayout _trackLayout;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _parameters;
        private readonly PaintBrush _paint = new PaintBrush
        {
            Color = Colors.Cyan,
            Style = PaintStyle.Fill
        };

        public bool Enabled { get; set; }
        public string Name => "Happiness";

        public HappinessRenderer(ITrackLayout trackLayout, IPixelMapper pixelMapper, ITrackParameters parameters)
        {
            _trackLayout = trackLayout;
            _pixelMapper = pixelMapper;
            _parameters = parameters;
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            foreach ((int col, int row, Track track) in _trackLayout)
            {
                if (!track.Happy)
                {
                    continue;
                }

                (int x, int y) = _pixelMapper.CoordsToViewPortPixels(col, row);

                canvas.DrawRect(x, y, _parameters.CellSize, _parameters.CellSize, _paint);
            }
        }
    }
}
