namespace Trains.NET.Rendering
{
    public interface ICanvas
    {
        public void DrawRect(float x, float y, float width, float height, PaintBrush paint);
        public void Save();
        public void Translate(float x, float y);
        public void DrawCircle(float x, float y, float radius, PaintBrush paint);
        public void Restore();
        public void DrawText(string text, float x, float y, PaintBrush paint);
        public void DrawLine(float x1, float y1, float x2, float y2, PaintBrush grid);
        public void ClipRect(Rectangle sKRect, ClipOperation intersect, bool v);
        public void RotateDegrees(float degrees, float x, float y);
        public void DrawPath(IPath trackPath, PaintBrush straightTrackPaint);
        public void RotateDegrees(float degrees);
        void Clear(Colors white);
    }
}
