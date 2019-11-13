using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Trains.NET.Rendering
{
    public class GridRenderer : IBoardRenderer
    {
        void IBoardRenderer.Render(SKSurface surface, int width, int height)
        {
            SKCanvas canvas = surface.Canvas;

            using var grid = new SKPaint
            {
                Color = SKColors.LightGray,
                StrokeWidth = 1,
                Style = SKPaintStyle.Stroke
            };

            for (int x = 0; x < width + 1; x += Game.CellSize)
            {
                canvas.DrawLine(x, 0, x, height, grid);
            }

            for (int y = 0; y < height + 1; y += Game.CellSize)
            {
                canvas.DrawLine(0, y, width, y, grid);
            }
        }
    }
}
