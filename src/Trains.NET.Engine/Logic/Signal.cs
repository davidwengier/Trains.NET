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

        public override void TryToggle()
        {
            this.SignalState = this.SignalState == SignalState.Go ? SignalState.Stop : SignalState.Go;

            OnChanged();
        }

        public override bool CanToggle() => true;

        public override bool IsBlocked()
            => this.SignalState == SignalState.Stop;
    }
}
