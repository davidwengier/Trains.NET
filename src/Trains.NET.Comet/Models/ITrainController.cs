using Comet;
using Trains.NET.Engine;

namespace Trains.NET.Comet
{
    public interface ITrainController
    {
        State<Train?> CurrentTrain { get; }
        State<string> Display { get; }
        State<string> SpeedDisplay { get; }

        void Delete();
        void Faster();
        void SetCurrentTrain(Train? train);
        void Slower();
        void Start();
        void Stop();
        void ToggleGlobalStop();
    }
}
