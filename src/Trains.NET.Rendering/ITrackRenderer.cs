using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public interface ITrackRenderer
    {
        void Render(ICanvas canvas, Track track, int width);
    }
}
