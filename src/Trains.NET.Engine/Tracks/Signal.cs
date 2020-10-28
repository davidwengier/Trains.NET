namespace Trains.NET.Engine
{
    public class Signal : Track, IUpdatableEntity
    {
        private const int TemporaryStopTime = 5 * 60;

        public SignalState SignalState { get; set; }

        public int TemporaryStopCounter { get; set; }

        public override string Identifier
            => $"{base.Identifier}.{this.SignalState}";

        public Signal() : base()
        {
            this.SignalState = SignalState.Go;
        }

        public void Update()
        {
            if (this.SignalState == SignalState.TemporaryStop &&
                ++this.TemporaryStopCounter >= TemporaryStopTime)
            {
                this.TemporaryStopCounter = 0;
                this.SignalState = SignalState.Go;

                OnChanged();
            }
        }

        public override bool IsBlocked()
            => this.SignalState != SignalState.Go;
    }
}
