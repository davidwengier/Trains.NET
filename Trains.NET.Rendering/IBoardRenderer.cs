using SkiaSharp;

namespace Trains.NET.Rendering
{
    public interface IBoardRenderer
    {
        void Render(SKSurface surface, int width, int height);
    }
}
