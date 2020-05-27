using System;
using System.ComponentModel;

namespace Trains.NET.Engine
{
    public class Train : IMovable, INotifyPropertyChanged
    {
        public const float SpeedScaleModifier = 0.005f;
        private const int MaximumSpeed = 200;
        private const float MinimumLookaheadSpeed = 5.0f;
        private const float DefaultSpeed = 20.0f;
        private readonly Random _random = new Random();
        private float? _lookaheadOverride;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Train()
        {
            this.Name = TrainNames.Names[_random.Next(0, TrainNames.Names.Length)];
            this.DesiredSpeed = DefaultSpeed;
        }

        public float LookaheadDistance
        {
            get
            {
                return _lookaheadOverride ?? 0.8f + SpeedScaleModifier * Math.Max(MinimumLookaheadSpeed, this.CurrentSpeed) * 20;
            }
            set
            {
                _lookaheadOverride = value;
            }
        }

        public float DistanceToMove => SpeedScaleModifier * this.CurrentSpeed;

        public Guid UniqueID { get; internal set; } = Guid.NewGuid();
        public int Column { get; internal set; }
        public int Row { get; internal set; }
        public float Angle { get; internal set; }
        public float RelativeLeft { get; internal set; } = 0.5f;
        public float RelativeTop { get; internal set; } = 0.5f;
        public string Name { get; set; }
        public float CurrentSpeed { get; private set; }
        public float DesiredSpeed { get; private set; }
        public bool Stopped { get; private set; }
        public bool CollisionAhead { get; private set; }

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
                CurrentSpeed = this.CurrentSpeed,
                DesiredSpeed = this.DesiredSpeed
            };
            result._lookaheadOverride = _lookaheadOverride;

            return result;
        }

        internal void ForceSpeed(float speed)
        {
            this.CurrentSpeed = speed;
            this.DesiredSpeed = speed;
        }

        public void Start() => this.Stopped = false;

        public void Stop() => this.Stopped = true;

        internal void Pause() => this.CollisionAhead = true;

        internal void Resume() => this.CollisionAhead = false;

        public void Slower()
        {
            if (this.DesiredSpeed > 5)
            {
                this.DesiredSpeed -= 5;
            }
        }
        public void Faster()
        {
            if (this.DesiredSpeed < MaximumSpeed)
            {
                this.DesiredSpeed += 5;
            }
        }

        public override string ToString() => $"Train {this.UniqueID} [Column: {this.Column} | Row: {this.Row} | Left: {this.RelativeLeft} | Top: {this.RelativeTop} | Angle: {this.Angle} | Speed: {this.CurrentSpeed}]";

        internal TrainPosition GetPosition() => new TrainPosition(this.Column, this.Row, this.RelativeLeft, this.RelativeTop, this.Angle, 0);

        internal void AdjustSpeed()
        {
            if (this.Stopped || this.CollisionAhead)
            {
                this.CurrentSpeed = Math.Max(this.CurrentSpeed - 1, 0);
            }
            else if (this.DesiredSpeed > this.CurrentSpeed)
            {
                this.CurrentSpeed++;
            }
            else if (this.DesiredSpeed < this.CurrentSpeed)
            {
                this.CurrentSpeed--;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentSpeed)));
        }
    }
}
