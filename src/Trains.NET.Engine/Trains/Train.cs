using System;
using System.ComponentModel;

namespace Trains.NET.Engine
{
    public class Train : IMovable, INotifyPropertyChanged
    {
        // only used for tests??
        internal const float SpeedScaleModifier = 0.005f;

        private const int MaximumSpeed = 200;
        private const float MinimumLookaheadSpeed = 5.0f;
        private const float DefaultSpeed = 20.0f;
        private readonly Random _random = new Random();
        private float? _lookaheadOverride;
        private bool _collisionAhead;

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
                return _lookaheadOverride ?? Math.Max(MinimumLookaheadSpeed, this.CurrentSpeed) * 30;
            }
            set
            {
                _lookaheadOverride = value;
            }
        }

        public Guid UniqueID { get; private set; } = Guid.NewGuid();

        public int Column { get; set; }
        public int Row { get; set; }
        public float Angle { get; set; }
        public float RelativeLeft { get; set; } = 0.5f;
        public float RelativeTop { get; set; } = 0.5f;

        public string Name { get; private set; }
        public float CurrentSpeed { get; private set; }
        public float DesiredSpeed { get; private set; }
        public bool Stopped { get; private set; }

        public bool Follow { get; set; }


        public void SetAngle(float angle)
        {
            while (angle < 0) angle += 360;
            while (angle > 360) angle -= 360;
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

        internal void Pause() => _collisionAhead = true;

        internal void Resume() => _collisionAhead = false;

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
            if (this.Stopped || _collisionAhead)
            {
                this.CurrentSpeed = Math.Max(this.CurrentSpeed - 1.0f, 0);
            }
            else if (this.DesiredSpeed > this.CurrentSpeed)
            {
                this.CurrentSpeed = Math.Min(this.CurrentSpeed + 1.0f, this.DesiredSpeed);
            }
            else if (this.DesiredSpeed < this.CurrentSpeed)
            {
                this.CurrentSpeed = Math.Max(this.CurrentSpeed - 1.0f, 0);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentSpeed)));
        }
    }
}
