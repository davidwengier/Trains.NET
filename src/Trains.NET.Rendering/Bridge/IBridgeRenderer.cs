using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public interface IBridgeRenderer
    {
        void Render(ICanvas canvas, TrackDirection direction);
    }
}
