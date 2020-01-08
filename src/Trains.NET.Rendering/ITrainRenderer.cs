using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public interface ITrainRenderer
    {
        void Render(SKCanvas canvas, Train train);
    }
}