using Comet;
using Trains.NET.Engine;

namespace Trains.NET.Comet
{
    internal class GameState : IGameState
    {
        public State<Train> CurrentTrain { get; } = new State<Train>();

        public void SetCurrentTrain(Train train)
        {
            this.CurrentTrain.Value = train;
        }
    }
}
