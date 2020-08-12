using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(0)]
    internal class GridRenderer : ILayerRenderer, ICachableLayerRenderer
    {
        private readonly IGameParameters _gameParameters;

        public GridRenderer(IGameParameters parameters)
        {
            _gameParameters = parameters;
        }

        public bool Enabled { get; set; }
        public string Name => "Grid";
        public bool IsDirty => false;

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            var grid = new PaintBrush
            {
                Color = Colors.LightGray,
                StrokeWidth = 1,
                Style = PaintStyle.Stroke
            };

            for (int x = pixelMapper.ViewPortX; x < pixelMapper.ViewPortWidth + 1; x += _gameParameters.CellSize)
            {
                canvas.DrawLine(x, 0, x, height, grid);
            }

            for (int y = pixelMapper.ViewPortY; y < pixelMapper.ViewPortHeight + 1; y += _gameParameters.CellSize)
            {
                canvas.DrawLine(0, y, width, y, grid);
            }
        }
    }
}
