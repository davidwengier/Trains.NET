namespace Trains.NET.Engine
{
    public class Signal : Track
    {
        public SignalState SignalState { get; set; }

        public override string Identifier
            => $"{base.Identifier}.{this.SignalState}";

        public Signal() : base()
        {
            this.SignalState = SignalState.Go;
        }

        public void SwitchSignal()
        {
            this.SignalState = this.SignalState == SignalState.Go ? SignalState.Stop : SignalState.Go;
        }

        public bool IsValidSignalDirection()
            => this.Direction == TrackDirection.Horizontal ||
                this.Direction == TrackDirection.Vertical;

        public bool ShouldBlockTrain()
            => IsValidSignalDirection() &&
                this.SignalState == SignalState.Stop;
    }
}
