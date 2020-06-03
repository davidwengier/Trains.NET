using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    public class SKCanvasWrapper : ICanvas
    {
        private static readonly Dictionary<PaintBrush, SKPaint> s_paintCache = new Dictionary<PaintBrush, SKPaint>();

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
                s_paintCache.Add(paint, skPaint);
            }
            return skPaint;
        }

        public void Clear(Color color)
            => _canvas.Clear(color.ToSkia());

        public void ClipRect(Rectangle rect, ClipOperation operation, bool antialias)
            => _canvas.ClipRect(rect.ToSkia(), operation.ToSkia(), antialias);

        public void Dispose()
        {
            ((IDisposable)_canvas).Dispose();
        }

        public void DrawBitmap(IBitmap bitmap, int width, int height)
            => _canvas.DrawBitmap(bitmap.ToSkia(), width, height);


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

        public void RotateDegrees(float degrees, float x, float y)
            => _canvas.RotateDegrees(degrees, x, y);

        public void RotateDegrees(float degrees)
            => _canvas.RotateDegrees(degrees);

        public void Save()
            => _canvas.Save();

        public void Translate(float x, float y)
            => _canvas.Translate(x, y);
    }
}
