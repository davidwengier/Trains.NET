using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public interface ITrainRenderer
    {
        void Render(ICanvas canvas, Train train);
    }
}
