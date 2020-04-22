using System;
using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    public static class RenderingExtensions
    {
        public static SKColor ToSkia(this Color color) => SKColor.Parse(color.HexCode);

        public static SKClipOperation ToSkia(this ClipOperation operation) => operation switch
        {
            ClipOperation.Intersect => SKClipOperation.Intersect,
            _ => throw new NotImplementedException(),
        };

        public static SKTextAlign ToSkia(this TextAlign align) => align switch
        {
            TextAlign.Left => SKTextAlign.Left,
            _ => throw new NotImplementedException(),
        };

        public static SKPaintStyle ToSkia(this PaintStyle style) => style switch
        {
            PaintStyle.Fill => SKPaintStyle.Fill,
            PaintStyle.Stroke => SKPaintStyle.Stroke,
            _ => throw new NotImplementedException(),
        };

        public static SKPathArcSize ToSkia(this PathArcSize arcSize) => arcSize switch
        {
            PathArcSize.Small => SKPathArcSize.Small,
            _ => throw new NotImplementedException(),
        };

        public static SKPathDirection ToSkia(this PathDirection arcSize) => arcSize switch
        {
            PathDirection.CounterClockwise => SKPathDirection.CounterClockwise,
            _ => throw new NotImplementedException(),
        };

        public static SKRect ToSkia(this Rectangle rect) => new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

        public static SKPaint ToSkia(this PaintBrush brush)
        {
            SKPaint paint = new SKPaint();

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
            if (brush.TextAlign != null)
            {
                paint.TextAlign = brush.TextAlign.Value.ToSkia();
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
