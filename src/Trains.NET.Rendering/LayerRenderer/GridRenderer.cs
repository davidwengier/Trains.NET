using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(1)]
public class GridRenderer : ICachableLayerRenderer
{
    public bool Enabled { get; set; }
    public string Name => "Grid";

    public event EventHandler? Changed { add { } remove { } } // wacky, but stops the compiler complaining its unused

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
