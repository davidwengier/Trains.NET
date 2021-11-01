using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public interface ILayerRenderer : ITogglable
{
    void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper);
}
