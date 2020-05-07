using System;

namespace Trains.NET.Engine
{
    public class Train : IMovable
    {
        private readonly Random _random = new Random();
        private float _previousSpeed;

        public Train()
        {
            this.Name = TrainNames.Names[_random.Next(0, TrainNames.Names.Length)];
        }

        public float LookaheadDistance { get; set; } = 1.5f;

        public Guid UniqueID { get; internal set; } = Guid.NewGuid();
        public int Column { get; internal set; }
        public int Row { get; internal set; }
        public float Angle { get; internal set; }
        public float RelativeLeft { get; internal set; } = 0.5f;
        public float RelativeTop { get; internal set; } = 0.5f;
        public string Name { get; set; }
        public float Speed { get; set; } = 10;

        public void SetAngle(float angle)
        {
            this.Angle = angle;
        }

        internal Train Clone()
        {
            return new Train()
            {
                UniqueID = this.UniqueID,
                Column = this.Column,
                Name = this.Name,
                Row = this.Row,
                Angle = this.Angle,
                RelativeLeft = this.RelativeLeft,
                RelativeTop = this.RelativeTop,
                Speed = this.Speed
            };
        }

        public void Start()
        {
            this.Speed = _previousSpeed;
        }

        public void Stop()
        {
            _previousSpeed = this.Speed;
            this.Speed = 0;
        }
    }
}
