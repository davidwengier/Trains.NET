using SkiaSharp;

namespace Trains.NET.Rendering
{
    public interface IBoardRenderer
    {
        bool Enabled { get; set; }
        string Name { get; }
        void Render(SKSurface surface, int width, int height);
    }
}
