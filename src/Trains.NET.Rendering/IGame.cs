using SkiaSharp;

namespace Trains.NET.Rendering
{
    public interface IGame
    {
        void Render(SKCanvas canvas);
        void SetSize(int width, int height);
    }
}
