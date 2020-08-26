using System;
using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    public static class RenderingExtensions
    {
        public static SKImage ToSkia(this IImage image) => ((SKImageWrapper)image).Image;

        public static SKColor ToSkia(this Color color) => color switch
        {
            Color c when c == Colors.Empty => SKColor.Empty,
            _ => SKColor.Parse(color.HexCode)
        };

        public static SKPaintStyle ToSkia(this PaintStyle style) => style switch
        {
            PaintStyle.Fill => SKPaintStyle.Fill,
            PaintStyle.Stroke => SKPaintStyle.Stroke,
            _ => throw new NotImplementedException(),
        };

        public static SKRect ToSkia(this Rectangle rect) => new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

        public static SKPaint ToSkia(this PaintBrush brush)
        {
            var paint = new SKPaint();

            if (brush.Color != null)
            {
                paint.Color = brush.Color.ToSkia();
            }
            if (brush.IsAntialias != null)
            {
                paint.IsAntialias = brush.IsAntialias.Value;
            }
            if (brush.Style != null)
            {
                paint.Style = brush.Style.Value.ToSkia();
            }
            if (brush.StrokeWidth != null)
            {
                paint.StrokeWidth = brush.StrokeWidth.Value;
            }
            if (brush.TextSize != null)
            {
                paint.TextSize = brush.TextSize.Value;
            }

            return paint;
        }

        public static SKPath ToSkia(this IPath path) => ((SKPathWrapper)path).Path;
    }
}
