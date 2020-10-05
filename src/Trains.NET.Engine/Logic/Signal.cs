namespace Trains.NET.Engine
{
    public class Signal : Track
    {
        public SignalState SignalState { get; private set; }

        public Signal() : base()
        {
            this.SignalState = SignalState.Go;
        }

        public void Switch()
        {
            this.SignalState = this.SignalState == SignalState.Go ? SignalState.Stop : SignalState.Go;
        }
    }
}
