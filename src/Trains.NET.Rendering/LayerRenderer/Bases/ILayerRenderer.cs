
namespace Trains.NET.Rendering
{
    public interface ILayerRenderer
    {
        bool Enabled { get; set; }
        string Name { get; }
        void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper);
    }
}
