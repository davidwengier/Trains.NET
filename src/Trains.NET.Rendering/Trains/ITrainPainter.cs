using Trains.NET.Engine;

namespace Trains.NET.Rendering.Trains
{
    public interface ITrainPainter
    {
        ITrainPalette GetPalette(Train train);
    }
}
