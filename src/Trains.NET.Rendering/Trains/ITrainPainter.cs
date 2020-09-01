using Trains.NET.Engine;

namespace Trains.NET.Rendering.Trains
{
    public interface ITrainPainter
    {
        TrainPalette GetPalette(Train train);
    }
}
