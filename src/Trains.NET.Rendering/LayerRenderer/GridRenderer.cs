using Trains.NET.Engine;
using Trains.NET.Rendering.LayerRenderer;

namespace Trains.NET.Rendering
{
    [Order(0)]
    internal class GridRenderer : ILayerRenderer, ICachableLayerRenderer
    {
        private bool _dirty = false;
        private readonly ITrackParameters _parameters;
        private readonly IPixelMapper _pixelMapper;

        public GridRenderer(ITrackParameters parameters, IPixelMapper pixelMapper)
        {
            _parameters = parameters;
            _pixelMapper = pixelMapper;
            _pixelMapper.ViewPortChanged += (s, e) => _dirty = true;
        }

        public bool Enabled { get; set; } = false;
        public string Name => "Grid";
        public bool IsDirty => _dirty;

        public void Render(ICanvas canvas, int width, int height)
        {
            var grid = new PaintBrush
            {
                Color = Colors.LightGray,
                StrokeWidth = 1,
                Style = PaintStyle.Stroke
            };

            for (int x = _pixelMapper.ViewPortX; x < _pixelMapper.ViewPortWidth + 1; x += _parameters.CellSize)
            {
                canvas.DrawLine(x, 0, x, height, grid);
            }

            for (int y = _pixelMapper.ViewPortY; y < _pixelMapper.ViewPortHeight + 1; y += _parameters.CellSize)
            {
                canvas.DrawLine(0, y, width, y, grid);
            }
        }
    }
}
