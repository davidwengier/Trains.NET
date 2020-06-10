using Trains.NET.Engine;
using Trains.NET.Rendering.LayerRenderer;

namespace Trains.NET.Rendering
{
    [Order(0)]
    internal class GridRenderer : ILayerRenderer, ICachableLayerRenderer
    {
        private readonly ITrackParameters _parameters;

        public GridRenderer(ITrackParameters parameters)
        {
            _parameters = parameters;
        }

        public bool Enabled { get; set; } = false;
        public string Name => "Grid";
        public bool IsDirty => false;

        public void Render(ICanvas canvas, int width, int height)
        {
            var grid = new PaintBrush
            {
                Color = Colors.LightGray,
                StrokeWidth = 1,
                Style = PaintStyle.Stroke
            };

            for (int x = 0; x < width + 1; x += _parameters.CellSize)
            {
                canvas.DrawLine(x, 0, x, height, grid);
            }

            for (int y = 0; y < height + 1; y += _parameters.CellSize)
            {
                canvas.DrawLine(0, y, width, y, grid);
            }
        }
    }
}
