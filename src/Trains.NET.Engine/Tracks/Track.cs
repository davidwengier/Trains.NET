using System;

namespace Trains.NET.Engine
{
    public abstract class Track : IStaticEntity
    {
        private ILayout? _trackLayout;

        public int Column { get; set; }
        public int Row { get; set; }
        public bool Happy { get; set; }
        public virtual bool HasMultipleStates => false;

        public virtual string Identifier => "";
        protected ILayout? TrackLayout => _trackLayout;

        public abstract void Move(TrainPosition position);
        public abstract bool IsConnectedRight();
        public abstract bool IsConnectedDown();
        public abstract bool IsConnectedLeft();
        public abstract bool IsConnectedUp();

        public virtual bool IsBlocked() => false;

        public virtual void NextState()
        {
        }

        public TrackNeighbors GetAllNeighbors()
        {
            _ = _trackLayout ?? throw new InvalidOperationException("Game board can't be null");

            return new TrackNeighbors(
                _trackLayout.TryGet(this.Column - 1, this.Row, out Track? left) ? left : null,
                _trackLayout.TryGet(this.Column, this.Row - 1, out Track? up) ? up : null,
                _trackLayout.TryGet(this.Column + 1, this.Row, out Track? right) ? right : null,
                _trackLayout.TryGet(this.Column, this.Row + 1, out Track? down) ? down : null
                );
        }

        protected void OnChanged()
        {
            _trackLayout?.RaiseCollectionChanged();
        }

        public void ReevaluateHappiness()
        {
            _ = _trackLayout ?? throw new InvalidOperationException("Game board can't be null");

            this.Happy = TrackNeighbors.GetConnectedNeighbours(_trackLayout, this.Column, this.Row).Count > 1;
        }

        public void Stored(ILayout? collection)
        {
            _trackLayout = collection;
        }

        public virtual void Replaced()
        {
            ReevaluateHappiness();
            var neighbours = GetAllNeighbors();
            foreach (var n in neighbours.All)
            {
                n.ReevaluateHappiness();
            }
        }

        public virtual void Created()
        {
        }

        public virtual void Updated()
        {
        }

        public virtual void Removed()
        {
        }

        public virtual void EnterTrack(Train train)
        {
        }

        public bool CanConnectRight()
            => !this.Happy || IsConnectedRight();
        public bool CanConnectDown()
            => !this.Happy || IsConnectedDown();
        public bool CanConnectLeft()
            => !this.Happy || IsConnectedLeft();
        public bool CanConnectUp()
            => !this.Happy || IsConnectedUp();
    }
}
