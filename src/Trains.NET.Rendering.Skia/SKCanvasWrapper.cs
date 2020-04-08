namespace Trains.NET.Rendering.Skia
{
    public class SKCanvasWrapper : Trains.NET.Rendering.ICanvas
    {
        private readonly SkiaSharp.SKCanvas _canvas;

        public SKCanvasWrapper(SkiaSharp.SKCanvas canvas)
        {
            _canvas = canvas;
        }

        public void Clear(Colors color)
            => _canvas.Clear(color.ToSkia());

        public void ClipRect(Rectangle rect, ClipOperation operation, bool antialias)
            => _canvas.ClipRect(rect.ToSkia(), operation.ToSkia(), antialias);

        public void DrawCircle(float x, float y, float radius, PaintBrush paint)
            => _canvas.DrawCircle(x, y, radius, paint.ToSkia());

        public void DrawLine(float x1, float y1, float x2, float y2, PaintBrush grid)
            => _canvas.DrawLine(x1, y1, x2, y2, grid.ToSkia());

        public void DrawPath(IPath trackPath, PaintBrush straightTrackPaint)
            => _canvas.DrawPath(trackPath.ToSkia(), straightTrackPaint.ToSkia());

        public void DrawRect(float x, float y, float width, float height, PaintBrush paint)
            => _canvas.DrawRect(x, y, width, height, paint.ToSkia());

        public void DrawText(string text, float x, float y, PaintBrush paint)
            => _canvas.DrawText(text, x, y, paint.ToSkia());

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
