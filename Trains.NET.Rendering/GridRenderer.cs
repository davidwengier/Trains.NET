using SkiaSharp;

namespace Trains.NET.Rendering
{
    internal class GridRenderer : IBoardRenderer
    {
        private readonly ITrackParameters _parameters;

        public GridRenderer(ITrackParameters parameters)
        {
            _parameters = parameters;
        }

        public bool Enabled { get; set; } = true;
        public string Name => "Grid";

        public void Render(SKSurface surface, int width, int height)
        {
            SKCanvas canvas = surface.Canvas;

            using var grid = new SKPaint
            {
                Color = SKColors.LightGray,
                StrokeWidth = 1,
                Style = SKPaintStyle.Stroke
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
