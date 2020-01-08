using SkiaSharp;

namespace Trains.NET.Rendering
{
    public interface ILayerRenderer
    {
        bool Enabled { get; set; }
        string Name { get; }
        void Render(SKCanvas canvas, int width, int height);
    }
}
