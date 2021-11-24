namespace Trains.NET.Rendering;

public interface ICanvas : IDisposable
{
    void DrawRect(float x, float y, float width, float height, PaintBrush paint);
    void Save();
    void Translate(float x, float y);
    void DrawCircle(float x, float y, float radius, PaintBrush paint);
    void Restore();
    void DrawText(string text, float x, float y, PaintBrush paint);
    void DrawLine(float x1, float y1, float x2, float y2, PaintBrush grid);
    void ClipRect(Rectangle sKRect, bool antialias, bool exclude);
    void DrawPicture(Picture picture, float x, float y, float size);
    void RotateDegrees(float degrees, float x, float y);
    void DrawPath(IPath trackPath, PaintBrush straightTrackPaint);
    void RotateDegrees(float degrees);
    void Clear(Color color);
    void DrawGradientRect(float x, float y, float width, float height, Color start, Color end);
    void Scale(float scaleX, float scaleY);
    void Scale(float scaleX, float scaleY, float x, float y);
    void DrawImage(IImage cachedImage, int x, int y);
    float MeasureText(string text, PaintBrush paint);
    void DrawImage(IImage image, Rectangle sourceRectangle, Rectangle destinationRectangle);
    void DrawGradientCircle(float x, float y, float width, float height, float circleX, float circleY, float radius, IEnumerable<Color> colours);
    void DrawVerticalGradientRect(float x, float y, float width, float height, IEnumerable<Color> colours);
    void DrawHorizontalGradientRect(float x, float y, float width, float height, IEnumerable<Color> colours);
    void DrawRoundRect(float x, float y, float width, float height, float radiusX, float radiusY, PaintBrush paint);
}
