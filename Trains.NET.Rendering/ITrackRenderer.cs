using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public interface ITrackRenderer
    {
        void Render(SKCanvas canvas, Track track, int width);
    }
}