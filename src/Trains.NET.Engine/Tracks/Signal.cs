namespace Trains.NET.Engine
{
    public class Signal : Track, IUpdatableEntity
    {
        private const int _temporaryStopTime = 5 * 60;

        public SignalState SignalState { get; set; }

        public int TemporaryStopCounter { get; set; }

        public override string Identifier
            => $"{base.Identifier}.{this.SignalState}";

        public Signal() : base()
        {
            this.SignalState = SignalState.Go;
        }

        public override void TryToggle()
        {
            this.SignalState = this.SignalState switch
            {
                SignalState.Go => SignalState.TemporaryStop,
                SignalState.TemporaryStop => SignalState.Stop,
                _ => SignalState.Go
            };

            if (this.SignalState == SignalState.TemporaryStop)
            {
                this.TemporaryStopCounter = 0;
            }

            OnChanged();
        }

        public void Update()
        {
            if (this.SignalState == SignalState.TemporaryStop &&
                ++this.TemporaryStopCounter >= _temporaryStopTime)
            {
                this.SignalState = SignalState.Go;

                OnChanged();
            }
        }

        public override bool CanToggle() => true;

        public override bool IsBlocked()
            => this.SignalState != SignalState.Go;
    }
}
