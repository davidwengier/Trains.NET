using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    public class SKCanvasWrapper : ICanvas
    {
        private static readonly Dictionary<PaintBrush, SKPaint> s_paintCache = new();

        private static readonly SKPaint s_noAntialiasPaint = new SKPaint
        {
            IsAntialias = false,
            IsDither = false
        };

        private readonly SkiaSharp.SKCanvas _canvas;

        public SKCanvasWrapper(SkiaSharp.SKCanvas canvas)
        {
            _canvas = canvas;
        }
        private static SKPaint GetSKPaint(PaintBrush paint)
        {
            if (!s_paintCache.TryGetValue(paint, out SKPaint skPaint))
            {
                skPaint = paint.ToSkia();
                s_paintCache[paint] = skPaint;
            }
            return skPaint;
        }

        public void Clear(Color color)
            => _canvas.Clear(color.ToSkia());

        public void ClipRect(Rectangle rect, bool antialias)
            => _canvas.ClipRect(rect.ToSkia(), antialias: antialias);

        public void Dispose()
        {
            ((IDisposable)_canvas).Dispose();
        }

        public void DrawImage(IImage image, int x, int y)
            => _canvas.DrawImage(image.ToSkia(), x, y);

        public void DrawImage(IImage image, Rectangle sourceRectangle, Rectangle destinationRectangle)
            => _canvas.DrawImage(image.ToSkia(), sourceRectangle.ToSkia(), destinationRectangle.ToSkia(), s_noAntialiasPaint);


        public void DrawCircle(float x, float y, float radius, PaintBrush paint)
            => _canvas.DrawCircle(x, y, radius, GetSKPaint(paint));

        public void DrawLine(float x1, float y1, float x2, float y2, PaintBrush paint)
            => _canvas.DrawLine(x1, y1, x2, y2, GetSKPaint(paint));

        public void DrawPath(IPath trackPath, PaintBrush paint)
            => _canvas.DrawPath(trackPath.ToSkia(), GetSKPaint(paint));

        public void DrawRect(float x, float y, float width, float height, PaintBrush paint)
            => _canvas.DrawRect(x, y, width, height, GetSKPaint(paint));

        public void DrawText(string text, float x, float y, PaintBrush paint)
            => _canvas.DrawText(text, x, y, GetSKPaint(paint));

        public void GradientRect(float x, float y, float width, float height, Color start, Color end)
        {
            var shader = SKShader.CreateLinearGradient(new SKPoint(x, y),
                                                       new SKPoint(x, y + height),
                                                       new SKColor[] { start.ToSkia(), end.ToSkia(), start.ToSkia() },
                                                       SKShaderTileMode.Clamp);
            using var paint = new SKPaint
            {
                Shader = shader
            };
            _canvas.DrawRect(x, y, width, height, paint);
        }

        public void Restore()
            => _canvas.Restore();

        public void Scale(float scaleX, float scaleY)
            => _canvas.Scale(scaleX, scaleY);

        public void Scale(float scaleX, float scaleY, float x, float y)
            => _canvas.Scale(scaleX, scaleY, x, y);

        public void RotateDegrees(float degrees, float x, float y)
            => _canvas.RotateDegrees(degrees, x, y);

        public void RotateDegrees(float degrees)
            => _canvas.RotateDegrees(degrees);

        public void Save()
            => _canvas.Save();

        public void Translate(float x, float y)
            => _canvas.Translate(x, y);

        public float MeasureText(string text, PaintBrush paint)
            => GetSKPaint(paint).MeasureText(text);
    }
}
