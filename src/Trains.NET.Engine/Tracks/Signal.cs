using System;

namespace Trains.NET.Engine
{
    public class Signal : Track, IUpdatableEntity
    {
        private const int TemporaryStopTime = 5 * 60;

        public SignalState SignalState { get; set; }

        public int TemporaryStopCounter { get; set; }

        public override string Identifier
            => $"{base.Identifier}.{this.SignalState}";

        public override bool HasMultipleStates => true;

        public Signal() : base()
        {
            this.SignalState = SignalState.Go;
        }

        public override void NextState()
        {
            this.SignalState = this.SignalState switch
            {
                SignalState.Go => SignalState.TemporaryStop,
                SignalState.TemporaryStop => SignalState.Stop,
                SignalState.Stop => SignalState.Go,
                _ => throw new InvalidOperationException()
            };

            OnChanged();
        }

        public void Update()
        {
            if (this.SignalState == SignalState.TemporaryStop &&
                ++this.TemporaryStopCounter >= TemporaryStopTime)
            {
                this.SignalState = SignalState.Go;
                this.TemporaryStopCounter = 0;

                OnChanged();
            }
        }

        public override bool IsBlocked()
            => this.SignalState != SignalState.Go;
    }
}
