using System;
using SkiaSharp;

namespace Trains.NET.Rendering.Skia;

public static class RenderingExtensions
{
    public static SKPicture ToSkia(this Picture picture)
        => picture switch
        {
            Picture.Left => Assets.Svg_caret_left.Picture,
            Picture.Right => Assets.Svg_caret_right.Picture,
            Picture.Backward => Assets.Svg_backward.Picture,
            Picture.Forward => Assets.Svg_forward.Picture,
            Picture.Eye => Assets.Svg_eye.Picture,
            Picture.Trash => Assets.Svg_trash_alt.Picture,
            Picture.Play => Assets.Svg_play.Picture,
            Picture.Pause => Assets.Svg_pause.Picture,
            Picture.Cross => Assets.Svg_times.Picture,
            Picture.Tools => Assets.Svg_tools.Picture,
            Picture.Eraser => Assets.Svg_eraser.Picture,
            Picture.Plus => Assets.Svg_plus_square.Picture,
            Picture.Minus => Assets.Svg_minus_square.Picture,
            _ => throw new InvalidOperationException($"No idea what picture you want me to draw: {picture}")
        };

    public static SKImage ToSkia(this IImage image) => ((SKImageWrapper)image).Image;

    public static SKColor ToSkia(this Color color) => color switch
    {
        Color c when c == Colors.Empty => SKColor.Empty,
        _ => new SKColor((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A)
    };

    public static SKPaintStyle ToSkia(this PaintStyle style) => style switch
    {
        PaintStyle.Fill => SKPaintStyle.Fill,
        PaintStyle.Stroke => SKPaintStyle.Stroke,
        _ => throw new NotImplementedException(),
    };

    public static SKRect ToSkia(this Rectangle rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);

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
