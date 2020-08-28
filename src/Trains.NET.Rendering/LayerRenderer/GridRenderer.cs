using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(0)]
    public class GridRenderer : ILayerRenderer, ICachableLayerRenderer
    {
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

            for (int x = pixelMapper.ViewPortX; x < pixelMapper.ViewPortWidth + 1; x += pixelMapper.CellSize)
            {
                canvas.DrawLine(x, 0, x, height, grid);
            }

            for (int y = pixelMapper.ViewPortY; y < pixelMapper.ViewPortHeight + 1; y += pixelMapper.CellSize)
            {
                canvas.DrawLine(0, y, width, y, grid);
            }
        }
    }
}
