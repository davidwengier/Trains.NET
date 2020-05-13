using System;

namespace Trains.NET.Engine
{
    public class Train : IMovable
    {
        public const float SpeedScaleModifier = 0.005f;

        private readonly Random _random = new Random();
        private float _previousSpeed;
        private float? _lookaheadOverride;

        public Train()
        {
            this.Name = TrainNames.Names[_random.Next(0, TrainNames.Names.Length)];
        }

        public float LookaheadDistance
        {
            get
            {
                return _lookaheadOverride ?? 0.8f + SpeedScaleModifier * this.Speed;
            }
            set
            {
                _lookaheadOverride = value;
            }
        }

        public float DistanceToMove => SpeedScaleModifier * this.Speed;

        public Guid UniqueID { get; internal set; } = Guid.NewGuid();
        public int Column { get; internal set; }
        public int Row { get; internal set; }
        public float Angle { get; internal set; }
        public float RelativeLeft { get; internal set; } = 0.5f;
        public float RelativeTop { get; internal set; } = 0.5f;
        public string Name { get; set; }
        public float Speed { get; set; } = 20;

        public void SetAngle(float angle)
        {
            this.Angle = angle;
        }

        internal Train Clone()
        {
            var result = new Train()
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
            result._lookaheadOverride = _lookaheadOverride;

            return result;
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

        public override string ToString() => $"Train {this.UniqueID} [Column: {this.Column} | Row: {this.Row} | Left: {this.RelativeLeft} | Top: {this.RelativeTop} | Angle: {this.Angle} | Speed: {this.Speed}]";

        internal TrainPosition GetPosition() => new TrainPosition(this.Column, this.Row, this.RelativeLeft, this.RelativeTop, this.Angle, 0);
    }
}
