using Comet;
using Trains.NET.Engine;

namespace Trains.NET.Comet
{
    public interface IGameState
    {
        State<Train?> CurrentTrain { get; }

        void SetCurrentTrain(Train? train);
    }
}
