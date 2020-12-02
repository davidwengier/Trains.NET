using System;

namespace Trains.NET.Engine
{
    public class Carriage : Train
    {
        private readonly Train _train;

        public Carriage(Train train)
        {
            _train = train;
        }

        public override Guid UniqueID => _train.UniqueID;

        public override float DesiredSpeed
        {
            get { return _train.DesiredSpeed; }
            set { }
        }

        public override float CurrentSpeed
        {
            get { return _train.CurrentSpeed; }
            set { }
        }

        public override bool Stopped
        {
            get { return _train.Stopped; }
            set { }
        }

        public Train Train => _train;
    }
}
